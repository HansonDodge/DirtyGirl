using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using DirtyGirl.Web.Utils;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class EventLeadController : BaseController
    {
        #region private members

        private readonly IEventLeadService _eventLeadService;

        #endregion

        #region Constructor

        public EventLeadController(IEventLeadService service)
        {
            _eventLeadService = service;
        }

        #endregion

        public ActionResult Index()
        {
            var eventLeads = new vmAdmin_EventLead
                                 {
                                     EventLead = _eventLeadService.GetAllGlobalEventLeads(),
                                     EventLeadTypes =
                                         Utilities.CreateSelectList(
                                             _eventLeadService.GetAllEventLeadTypes(),
                                             value => value.EventLeadTypeId, text => text.TypeName, false)
                                 };
            return View(eventLeads);
        }

        #region Event Lead Ajax

        [HttpPost]
        public ActionResult Ajax_GetEventLeads([DataSourceRequest] DataSourceRequest request)
        {
            var leadList = _eventLeadService.GetAllGlobalEventLeads();
            var eventLeadList = leadList.Select(leadItem => new vmAdmin_EventLeadItem
            {
                DisplayText = leadItem.DisplayText,
                EventId = leadItem.EventId,
                EventLeadId = leadItem.EventLeadId,
                EventLeadTypeId = leadItem.EventLeadTypeId,
                IsGlobal = 1,
                Title = leadItem.Title
            }).ToList();
            return Json(eventLeadList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_CreateEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView)
        {
            EventLead eventLead = null;
            if (ModelState.IsValid)
            {
                eventLead = new EventLead
                {
                    DisplayText = eventLeadView.DisplayText,
                    EventId = null,
                    EventLeadId = eventLeadView.EventLeadId,
                    EventLeadTypeId = eventLeadView.EventLeadTypeId,
                    Title = eventLeadView.Title,
                };

                ServiceResult result = _eventLeadService.CreateEventLead(eventLead);

                if (!result.Success)
                    Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());

            }

            return Json(new[] { eventLead }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Ajax_UpdateEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView)
        {
            if (ModelState.IsValid)
            {
                var eventLead = new EventLead
                {
                    DisplayText = eventLeadView.DisplayText,
                    EventId = null,
                    EventLeadId = eventLeadView.EventLeadId,
                    EventLeadTypeId = eventLeadView.EventLeadTypeId,
                    Title = eventLeadView.Title,
                    
                };

                ServiceResult result = _eventLeadService.UpdateEventLead(eventLead);

                if (!result.Success)
                    Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteEventLead([DataSourceRequest] DataSourceRequest request, vmAdmin_EventLeadItem eventLeadView)
        {
            ServiceResult result = _eventLeadService.RemoveEventLead(eventLeadView.EventLeadId);

            if (!result.Success)
                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion  

    }
}
