using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Utils;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class EventController : BaseController
    {

        #region private members

        private readonly IEventService _eventService;

        #endregion

        #region Constructor

        public EventController(IEventService eventService)
        {           
            this._eventService = eventService;
        }

        #endregion
        
        #region Home

        public ActionResult Index()
        {
            vmAdmin_EventIndex vm = new vmAdmin_EventIndex 
            { 
                EventList = _eventService.GetAllUpcomingEventDetails(),                
                RegionList = _eventService.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId),
                TemplateList = _eventService.GetEventTemplates().Select(x => new EventTemplate { Name = x.Name, EventTemplateId = x.EventTemplateId }).ToList(),
                NewEvent = new CreateNewEvent()
            };

            return View(vm);
        }

        #endregion     
  
        #region AddEvent

        public ActionResult AddEvent()
        {
            return PartialView("Partial/AddEvent", new vmAdmin_AddEvent());
        }

        [HttpPost]
        public ActionResult AddEvent(vmAdmin_AddEvent model)
        {
            if (ModelState.IsValid)            
            {               
                ServiceResult result = _eventService.CreateEventByTemplate(model.NewEvent);
                    
                if (result.Success)
                    return RedirectToAction("EditEvent", new { id = model.NewEvent.EventId });
                else
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return View(model);            
        }

        #endregion        

        #region Edit Event

        [HttpGet]
        public ActionResult EditEvent(int? id)
        {
            vmAdmin_EditEvent vm = new vmAdmin_EditEvent();            

            if (id.HasValue)
                vm.Event = _eventService.GetEventById(id.Value) ?? new Event();
            else
                vm.Event = new Event();

            vm.RegionList = _eventService.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId);
            vm.FeeTypes = DirtyGirlExtensions.ConvertToSelectList<EventFeeType>();
            vm.EventLeadTypes = Utilities.CreateSelectList(_eventService.GetEventLeadTypes(), value => value.EventLeadTypeId, text => text.TypeName, false);
            vm.CouponTypeList = DirtyGirlExtensions.ConvertToSelectList<CouponType>();
            vm.DiscountTypeList = DirtyGirlExtensions.ConvertToSelectList<DiscountType>();

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditEvent(vmAdmin_EditEvent model)
        {
            
            if (ModelState.IsValid)
            {
                ServiceResult result;

                //The editor widget tries to keep encoding the html on the post, so decode it before it goes in making sure it stays as raw hmtl.
                model.Event.EventDetails = HttpUtility.HtmlDecode(model.Event.EventDetails);

                result = _eventService.UpdateEvent(model.Event);

                if (result.Success)
                {                  
                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "This event has been saved successfully"));
                    return RedirectToAction("EditEvent", new { id = model.Event.EventId });
                }
                else
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            } 


            // repopulate the grids..
            model.RegionList = _eventService.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId);
            model.FeeTypes = DirtyGirlExtensions.ConvertToSelectList<EventFeeType>();
            model.EventLeadTypes = Utilities.CreateSelectList(_eventService.GetEventLeadTypes(), value => value.EventLeadTypeId, text => text.TypeName, false);
            model.CouponTypeList = DirtyGirlExtensions.ConvertToSelectList<CouponType>();
            model.DiscountTypeList = DirtyGirlExtensions.ConvertToSelectList<DiscountType>();
            return View(model);
        }

        #endregion

        #region View Event

        [HttpGet]
        public ActionResult ViewEvent(int id)
        {
            Event e = _eventService.GetEventById(id);
            return View(e);
        }

        #endregion

        #region Ajax Methods

        #region Event Dates

        [HttpPost]
        public ActionResult Ajax_GetEventDates([DataSourceRequest] DataSourceRequest request, int masterEventId)
        {                     
            return Json(_eventService.GetDatesForEvent(masterEventId).Select(x => new {x.EventDateId, x.EventId, x.DateOfEvent, x.IsActive}).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_CreateEventDate([DataSourceRequest] DataSourceRequest request, EventDate eventDate, int masterEventId)
        {
            if (ModelState.IsValid)
            {
                eventDate.EventId = masterEventId;
                ServiceResult result = _eventService.CreateEventDate(eventDate);

                if (!result.Success)                   
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(new[] { eventDate }.ToDataSourceResult(request, ModelState));

        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventDate([DataSourceRequest] DataSourceRequest request, EventDate eventDate)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = _eventService.UpdateEventDate(eventDate);

                if (!result.Success)                   
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());                
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventDate([DataSourceRequest] DataSourceRequest request, EventDate eventDate)
        {
            ServiceResult result = _eventService.RemoveEventDate(eventDate.EventDateId);

            if (!result.Success)              
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());                

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion

        #region Generate Event Date

        public ActionResult GenerateEventDate(int eventId)
        {
            return View("Partial/GenerateEventDate", new vmAdmin_CreateEventDate(eventId));
        }

        [HttpPost]
        public ActionResult GenerateEventDate(vmAdmin_CreateEventDate model)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = _eventService.GenerateEventDate(model.EventId, model.EventDate, model.WaveStartTime, model.WaveEndTime, model.Duration, model.MaxRegistrants);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            if (ModelState.IsValid)
                return Json(new object[] { true, this.RenderPartialViewToString("Partial/GenerateEventDateForm", new vmAdmin_CreateEventDate()) });
            else
                return Json(new object[] { false, this.RenderPartialViewToString("Partial/GenerateEventDateForm", model) });

        }

        #endregion

        #region Event Waves

        [HttpPost]
        public ActionResult Ajax_GetEventWaves([DataSourceRequest] DataSourceRequest request, int masterEventDateId)
        {
            return Json( _eventService.GetWavesForEventDate(masterEventDateId).Select(x => new {x.EventWaveId, x.EventDateId, x.StartTime, x.EndTime, x.IsActive, x.MaxRegistrants }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_CreateEventWave([DataSourceRequest] DataSourceRequest request, EventWave eventWave, int masterEventDateId)
        {
            if (ModelState.IsValid)
            {
                eventWave.EventDateId = masterEventDateId;
                ServiceResult result = _eventService.CreateEventWave(eventWave);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventWave([DataSourceRequest] DataSourceRequest request, EventWave eventWave)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = _eventService.UpdateEventWave(eventWave);

                if (!result.Success)                   
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());                
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventWave([DataSourceRequest] DataSourceRequest request, EventWave eventWave)
        {
            ServiceResult result = _eventService.RemoveEventWave(eventWave.EventWaveId);

            if (!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }       

        #endregion

        #region Event Sponsors

        public ActionResult AddEventSponsor(int eventId)
        {
            return View("Partial/AddEventSponsor", eventId);
        }       

        [HttpPost]
        public ActionResult Ajax_AddLogo(IEnumerable<HttpPostedFileBase> logos, int masterEventId)
        {
            try
            {
                foreach (var logo in logos)
                {
                    Image img = Image.FromStream(logo.InputStream, true, true);
                    Image thumbnail = Utilities.ResizeImage(img, new Size{Height = DirtyGirlConfig.Settings.LogoHieght, Width =DirtyGirlConfig.Settings.LogoWidth});

                    var fileName = string.Format("{0}.png",Path.GetFileNameWithoutExtension(logo.FileName));
                    var thumbnailName = string.Format("thumbnail_{0}.png", Path.GetFileNameWithoutExtension(logo.FileName));

                    var physicalPath = Path.Combine(Server.MapPath(DirtyGirlConfig.Settings.EventImageFolder), masterEventId.ToString(), "sponsors");
                    var relativeUrl = string.Format("{0}/{1}/{2}/", DirtyGirlConfig.Settings.EventImageFolder, masterEventId, "sponsors");

                    var standardUrl = string.Format("{0}{1}", relativeUrl, fileName);
                    var thumbnailUrl = string.Format("{0}{1}", relativeUrl, thumbnailName);

                    var standardPath = Path.Combine(physicalPath, fileName);
                    var thumnailPath = Path.Combine(physicalPath, thumbnailName);

                    if (!Directory.Exists(physicalPath))
                        Directory.CreateDirectory(physicalPath);

                    img.Save(standardPath, ImageFormat.Png);
                    thumbnail.Save(thumnailPath, ImageFormat.Png);

                    EventSponsor sponsor = new EventSponsor();
                    sponsor.SponsorName = "Enter Sponsorship Name";
                    sponsor.EventId = masterEventId;
                    sponsor.FileName = fileName;
                    sponsor.Url = standardUrl;
                    sponsor.thumbnailUrl = thumbnailUrl;

                    ServiceResult result = _eventService.CreateEventSponsor(sponsor);

                    if (result.Success)
                        return Content("");                                       
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Content(ex.Message);
            }

            return Content("An Error Occured Uploading your logo."); 
        }

        [HttpPost]
        public ActionResult Ajax_RemoveLogo(string[] fileNames, int masterEventId)
        {
            foreach (var fileName in fileNames)
            {
                var fileNameToDelete = string.Format("{0}.png", Path.GetFileNameWithoutExtension(fileName));

                ServiceResult result = _eventService.RemoveEventSponsor(masterEventId, fileNameToDelete);

                if (result.Success)
                {

                    var physicalPath = Path.Combine(Server.MapPath(DirtyGirlConfig.Settings.EventImageFolder), masterEventId.ToString(), "sponsors");

                    FileInfo imageToDelete = new FileInfo(Path.Combine(physicalPath, fileNameToDelete));
                    FileInfo thumbnailToDelete = new FileInfo(Path.Combine(physicalPath, "thumbnail_" + fileNameToDelete));

                    try
                    {
                        imageToDelete.Delete();
                        thumbnailToDelete.Delete();
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }

            }

            return Content("");
        }

        public ActionResult Ajax_GetEventSponsors([DataSourceRequest] DataSourceRequest request, int masterEventId)
        {
            return Json(_eventService.GetSponsorsForEvent(masterEventId).Select(x => new {x.EventSponsorId, x.EventId, x.SponsorName, x.FileName, x.Description, x.Url, x.thumbnailUrl}).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventSponsor([DataSourceRequest] DataSourceRequest request, EventSponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = _eventService.UpdateEventSponsor(sponsor);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventSponsor([DataSourceRequest] DataSourceRequest request, EventSponsor sponsor)
        {
            ServiceResult result = _eventService.RemoveEventSponsor(sponsor.EventSponsorId);

            if (result.Success)
            {
                var physicalPath = Path.Combine(Server.MapPath(DirtyGirlConfig.Settings.EventImageFolder), sponsor.EventId.ToString(), "sponsors");
                FileInfo imageToDelete = new FileInfo(Path.Combine(physicalPath, sponsor.FileName));
                FileInfo thumbnailToDelete = new FileInfo(Path.Combine(physicalPath, "thumbnail_" + sponsor.FileName));

                try
                {
                    imageToDelete.Delete();
                    thumbnailToDelete.Delete();
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            else
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion

        #region registration Fees

        [HttpPost]
        public ActionResult Ajax_GetEventFees([DataSourceRequest] DataSourceRequest request, int masterEventId)
        {
            var feeList = _eventService.GetFeesForEvent(masterEventId).Select(x => new { x.PurchaseItemId, x.EventFeeType, x.EventId, x.EffectiveDate, x.Cost });
            return Json(feeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_CreateEventFee([DataSourceRequest] DataSourceRequest request, EventFee fee, int masterEventId)
        {
            if (ModelState.IsValid)
            {
                fee.EventId = masterEventId;
                ServiceResult result = _eventService.CreateEventFee(fee);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            }

            return Json(new[] { fee }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventFee([DataSourceRequest] DataSourceRequest request, EventFee fee)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = _eventService.UpdateEventFee(fee);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventFee([DataSourceRequest] DataSourceRequest request, EventFee fee)
        {
            ServiceResult result = _eventService.RemoveEventFee(fee.PurchaseItemId);

            if (!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion   
    
        #region Event Lead Ajax

        [HttpPost]
        public ActionResult Ajax_GetEventLeads([DataSourceRequest] DataSourceRequest request, int masterEventId)
        {
            var leadList = _eventService.GetEventLeads(masterEventId, true);
            var eventLeadList = leadList.Select(leadItem => new vmAdmin_EventLeadItem
                                                                {
                                                                    DisplayText = leadItem.DisplayText,
                                                                    EventId = leadItem.EventId,
                                                                    EventLeadId = leadItem.EventLeadId,
                                                                    EventLeadTypeId = leadItem.EventLeadTypeId,
                                                                    IsGlobal = (leadItem.EventId.HasValue ? 0 : 1),
                                                                    Title = leadItem.Title
                                                                }).ToList();
            return Json(eventLeadList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_CreateEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView, int masterEventId)
        {
            EventLead eventLead = null;
            if (ModelState.IsValid)
            {
                if (eventLeadView.IsGlobal == 0)
                    eventLeadView.EventId = masterEventId;
                else
                    eventLeadView.EventId = null;

                eventLead = new EventLead
                                          {
                                              DisplayText = eventLeadView.DisplayText,
                                              EventId = eventLeadView.EventId,
                                              EventLeadId = eventLeadView.EventLeadId,
                                              EventLeadTypeId = eventLeadView.EventLeadTypeId,
                                              Title = eventLeadView.Title
                                          };

                ServiceResult result = _eventService.CreateEventLead(eventLead);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            }

            return Json(new[] { eventLead }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView, int masterEventId)
        {
            if (ModelState.IsValid)
            {
                if (eventLeadView.IsGlobal == 0)
                    eventLeadView.EventId = masterEventId;
                else
                    eventLeadView.EventId = null;

                EventLead eventLead = new EventLead
                                          {
                                              DisplayText = eventLeadView.DisplayText,
                                              EventId = eventLeadView.EventId,
                                              EventLeadId = eventLeadView.EventLeadId,
                                              EventLeadTypeId = eventLeadView.EventLeadTypeId,
                                              Title = eventLeadView.Title
                                          };

                ServiceResult result = _eventService.UpdateEventLead(eventLead);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView)
        {
            ServiceResult result = _eventService.RemoveEventLead(eventLeadView.EventLeadId);

            if (!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion  

        #region EventCoupons



        #endregion

        public ActionResult Ajax_GetUpcomingEvents()
        {
            return Json(_eventService.GetAllUpcomingEvents().Select(e => new { EventId = e.EventId, EventName = e.EventDates.Min().DateOfEvent.ToShortDateString() + " - " + e.Locality + ", " + e.Region.Name }), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
