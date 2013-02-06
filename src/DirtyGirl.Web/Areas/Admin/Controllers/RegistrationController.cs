using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class RegistrationController : BaseController
    {
        #region Private Members

        private readonly IRegistrationService _registrationService;

        #endregion

        #region Constructor

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        #endregion

        public ActionResult EventSelection(int registrationId = 0)
        {
            var vm = new vmRegistration_EventSelection();
            var currentRegistration = _registrationService.GetRegistrationById(registrationId);

            vm.EventId = currentRegistration.EventWave.EventDate.EventId;
            vm.EventDateId = currentRegistration.EventWave.EventDateId;
            vm.EventWaveId = currentRegistration.EventWaveId;

            vm.LockEvent = false;
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            return View(vm);
        }

        [HttpPost]
        public ActionResult EventSelection(int eventWaveId, int registrationId)
        {
            var eventObj = _registrationService.GetRegistrationById(registrationId);
            var result = _registrationService.ChangeEvent(registrationId, eventWaveId, null);

            if(!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            return RedirectToAction("ViewUser", "User", new { id = eventObj.UserId });
        }


        public ActionResult CancelRegistration(int registrationId)
        {
            var eventObj = _registrationService.GetRegistrationById(registrationId);
            if (eventObj != null)
                return View(eventObj);
            else
            {
                return HttpNotFound("RegistrationId not found");
            }
        }

        [HttpPost]
        public ActionResult ConfirmCancelRegistration(int registrationId)
        {
            var eventObj = _registrationService.GetRegistrationById(registrationId);
            var result = _registrationService.CancelRegistration(registrationId);
            if (!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            return RedirectToAction("ViewUser", "User", new { id = eventObj.UserId });
        }

        public ActionResult GetWavesByEventDateId(int eventDateId)
        {
            var waves = _registrationService.GetWaveDetialsForEventDate(eventDateId).ToList();
            var vm = waves.Select(GetWaveItem);

            return Json(vm.Select(x => new { EventWaveId = x.EventWaveId, Description = string.Format("{0} / {1}", x.StartTime.ToShortTimeString(), x.WaveNotification)}), JsonRequestBehavior.AllowGet);
        }

        private vmRegistration_WaveItem GetWaveItem(EventWaveDetails wave)
        {
            var newWave = new vmRegistration_WaveItem
            {
                EventWaveId = wave.EventWaveId,
                StartTime = wave.StartTime,
                isFull = wave.SpotsLeft <= 0
            };

            if (wave.SpotsLeft <= 0)
            {
                newWave.WaveNotification = "SOLD OUT!";
                newWave.cssClassName = "full_wave";
            }
            else if (wave.SpotsLeft < DirtyGirlConfig.Settings.DisplaySpotsAvailableCount)
            {
                newWave.WaveNotification = string.Format("{0} spots left", wave.SpotsLeft);
                newWave.cssClassName = "half_wave";
            }
            else
            {
                newWave.WaveNotification = "spots open";
                newWave.cssClassName = "empty_wave";
            }

           return newWave;
        }

        private List<vmRegistration_WaveItem> GetWaveItems(List<EventWaveDetails> waveOverviewList)
        {

            var waveItemList = new List<vmRegistration_WaveItem>();

            foreach (var wave in waveOverviewList)
            {
                var newWave = new vmRegistration_WaveItem
                                  {
                                      EventWaveId = wave.EventWaveId,
                                      WaveNumber = waveOverviewList.IndexOf(wave) + 1,
                                      StartTime = wave.StartTime,
                                      isFull = wave.SpotsLeft <= 0
                                  };

                if (wave.SpotsLeft <= 0)
                {
                    newWave.WaveNotification = "SOLD OUT!";
                    newWave.cssClassName = "full_wave";
                }
                else if (wave.SpotsLeft < DirtyGirlConfig.Settings.DisplaySpotsAvailableCount)
                {
                    newWave.WaveNotification = string.Format("{0} spots left", wave.SpotsLeft);
                    newWave.cssClassName = "half_wave";
                }
                else
                {
                    newWave.WaveNotification = "spots open";
                    newWave.cssClassName = "empty_wave";
                }

                waveItemList.Add(newWave);
            }

            return waveItemList;

        }

        #region Ajax Methods

        public JsonResult GetEventList()
        {
            return Json(_registrationService.GetActiveUpcomingEvents().Select(x => new { EventId = x.EventId, Name = x.EndDate > x.StartDate ? string.Format("{0}, {1} : {2} - {3}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString(), x.EndDate.ToShortDateString()) : string.Format("{0}, {1} : {2}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString()) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventDateList(int eventId)
        {
            return Json(_registrationService.GetActiveDateDetailsByEvent(eventId).Select(x => new { EventDateId = x.EventDateId, DateOfEvent = x.DateOfEvent.ToShortDateString() }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_GetEventRegistrants([DataSourceRequest] DataSourceRequest request, int eventId)
        {
            return Json(_registrationService.GetRegistrationsByEvent(eventId).Select(r => new
            {
                r.RegistrationId,
                r.UserId,
                CreatedByUsername = r.User.UserName,
                r.IsThirdPartyRegistration,
                r.FirstName,
                r.LastName,
                r.TeamId,
                TeamName = (r.TeamId != null ? r.Team.Name : "Individual"),
                r.Address1,
                r.Address2,
                City = r.Locality,
                State = r.Region.Name,
                Zip = r.PostalCode,
                r.Phone,
                r.Email,
                r.EmergencyContact,
                r.EmergencyPhone,
                r.MedicalInformation,
                r.SpecialNeeds,
                r.DateAdded,
                WaveDateTime = r.EventWave.StartTime,
                r.RegistrationType,
            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Ajax_GetTeamRegistrants([DataSourceRequest] DataSourceRequest request, int teamId)
        {
            var registrants = _registrationService.GetRegistrationsByTeam(teamId).Select(r => new
            {
                r.RegistrationId,
                r.UserId,
                CreatedByUsername = r.User.UserName,
                r.IsThirdPartyRegistration,
                r.FirstName,
                r.LastName,
                r.TeamId,
                TeamName = (r.TeamId != null ? r.Team.Name : "Individual"),
                r.Address1,
                r.Address2,
                City = r.Locality,
                State = r.Region.Name,
                Zip = r.PostalCode,
                r.Phone,
                r.Email,
                r.EmergencyContact,
                r.EmergencyPhone,
                r.MedicalInformation,
                r.SpecialNeeds,
                r.DateAdded,
                WaveDateTime = r.EventWave.StartTime,
                r.RegistrationType,
            });
            return Json(registrants.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ajax_GetUserRegistrations([DataSourceRequest] DataSourceRequest request, int userId,
                                                      bool futureOnly)
        {
            var registrations =
                _registrationService.GetRegistrationByUserID(userId)
                                    .Where(r => (!futureOnly || r.EventWave.StartTime > DateTime.Now) &&
                                    r.RegistrationStatus == RegistrationStatus.Active);

            var result = registrations.Select(reg => new vmAdmin_RegistrationListItem
                                                         {
                                                             RegistrationId = reg.RegistrationId,
                                                             WaveDateTime = reg.EventWave.StartTime,
                                                             EventLocation =
                                                                 string.Format("{0}, {1}",
                                                                               reg.EventWave.EventDate.Event
                                                                                  .GeneralLocality,
                                                                               reg.EventWave.EventDate.Event.Region.Code),
                                                             EventId = reg.EventWave.EventDate.EventId,
                                                             IsThirdPartyRegistration = reg.IsThirdPartyRegistration,
                                                             FirstName = reg.FirstName,
                                                             LastName = reg.LastName,
                                                             TeamId = reg.TeamId,
                                                             TeamName = (reg.Team != null ? reg.Team.Name : "Individual"),
                                                             TeamCode = (reg.Team != null ? reg.Team.Code : "n/a"),
                                                             Address1 = reg.Address1,
                                                             Address2 = reg.Address2 ?? string.Empty,
                                                             City = reg.Locality,
                                                             State = reg.Region.Code,
                                                             Zip = reg.PostalCode,
                                                             Phone = reg.Phone,
                                                             Email = reg.Email,
                                                             EmergencyContact = reg.EmergencyContact ?? string.Empty,
                                                             EmergencyPhone = reg.EmergencyPhone ?? string.Empty,
                                                             MedicalInformation = reg.MedicalInformation ?? string.Empty,
                                                             SpecialNeeds = reg.SpecialNeeds ?? string.Empty,
                                                             DateAdded = reg.DateAdded,
                                                             RegistrationType = reg.RegistrationType.ToString(),
                                                             RegistrationStatus = reg.RegistrationStatus.ToString(),
                                                             IsOfAge = reg.IsOfAge,
                                                             IsFemale = reg.IsFemale,
                                                             AgreeToTerms = reg.AgreeToTerms
                                                         }).ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
