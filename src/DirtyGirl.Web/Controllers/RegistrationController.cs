using System.Diagnostics;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Utilities = DirtyGirl.Web.Utils.Utilities;

namespace DirtyGirl.Web.Controllers
{
    [Authorize(Roles = "Registrant, Admin, SuperUser")]
    public class RegistrationController : BaseController
    {

        #region Constructor

        IRegistrationService _service;

        public RegistrationController(IRegistrationService service)
        {
            this._service = service;
        }

        #endregion

        #region EventSelection

        public ActionResult EventSelection(Guid itemId, int? eventId, int? eventDateId, int? eventWaveId, bool? returnToRegDetails, bool? redemption)
        {

            int eId = (eventId.HasValue)? eventId.Value : 0;  
            
            var vm = new vmRegistration_EventSelection
            {
                ItemId = itemId,
                CartFocus = SessionManager.CurrentCart.CheckOutFocus,
                EventOverview = (eventId.HasValue && eventId > 0 ? _service.GetEventOverviewById(eId) : new EventOverview()),
                EventDateCount = (eventId.HasValue && eventId > 0 ? GetEventDateCount(eId) : 0),
                LockEvent = true,
                ReturnToRegistrationDetails = false
            };

            if (eventWaveId.HasValue)
            {
                EventWave selectedWave = _service.GetEventWaveById(eventWaveId.Value);
                vm.EventId = selectedWave.EventDate.EventId;
                vm.EventDateId = selectedWave.EventDateId;
                vm.EventWaveId = selectedWave.EventWaveId;  
            }
            else if(eventDateId.HasValue)
            {
                EventDate selectedDate = _service.GetEventDateById(eventDateId.Value);
                vm.EventId = selectedDate.EventId;
                vm.EventDateId = selectedDate.EventDateId;
            }
            else if (eventId.HasValue)
            {
                Event selectedEvent = _service.GetEventById(eventId.Value);
                vm.EventId = eventId.Value;
                vm.EventDateId = selectedEvent.EventDates.First().EventDateId;
                vm.EventName = selectedEvent.GeneralLocality;
            }
      

            // did the cart timeout?
            if (!SessionManager.CurrentCart.ActionItems.ContainsKey(itemId))
            {
                return RedirectToAction("Index", "home");
            }
            if (SessionManager.CurrentCart.ActionItems[itemId].ActionType == CartActionType.EventChange ||
                SessionManager.CurrentCart.CheckOutFocus == CartFocusType.Redemption)
                vm.LockEvent = false;

            if (returnToRegDetails.HasValue && returnToRegDetails.Value == true)
                vm.ReturnToRegistrationDetails = true;

            return View(vm);
        }

        [HttpPost]
        public ActionResult WaveSelected(int eventWaveId, Guid itemId, bool ReturnToRegistrationDetails, int? initialWaveId, int? initialEventId)
        {
            if (!Utilities.IsValidCart())
                return  RedirectToAction("Index", "home");

            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.Registration;

            //save event city for thank you page... 
            EventWave selectedWave = _service.GetEventWaveById(eventWaveId);
            SessionManager.CurrentCart.EventCity = selectedWave.EventDate.Event.GeneralLocality;

            var action = SessionManager.CurrentCart.ActionItems[itemId];
            
            switch (action.ActionType)
            {
                case CartActionType.NewRegistration:
                    
                    ((Registration)action.ActionObject).EventWaveId = eventWaveId;

                    if (ReturnToRegistrationDetails)
                    {
                        return RedirectToAction("RegistrationDetails", new { itemId });
                    }
                    return RedirectToAction("CreateTeam", new { itemId});

                case CartActionType.EventChange:
                    if (EventChanged(initialEventId, eventWaveId))
                    {
                        ((ChangeEventAction)action.ActionObject).UpdatedEventWaveId = eventWaveId;
                        action.ItemReadyForCheckout = true;
                        return RedirectToAction("checkout", "cart");
                    }
                    // remove it from the cart, we are not going to be charging the user


                    // is only a wave change, process it as such
                    var eventChangeResult = ChangeWave(((ChangeEventAction)action.ActionObject).RegistrationId, eventWaveId, itemId, initialWaveId);
                    if (eventChangeResult != null)
                        return eventChangeResult;
                   
                    break;

                case CartActionType.WaveChange:
                   
                    // ok, it changed, update 
                    var result = ChangeWave(((ChangeWaveAction)action.ActionObject).RegistrationId, eventWaveId, itemId, initialWaveId);
                    if (result != null)
                        return result;
                                                               
                    break;
            }

            return RedirectToAction("EventSelection", new {eventWaveId, itemId});
        }

        private RedirectToRouteResult ChangeWave(int registrationId, int eventWaveId, Guid itemId, int ?initialWaveId)
        {
            // did they even change it?
            if (initialWaveId.HasValue && initialWaveId.Value == eventWaveId)
            {
                SessionManager.CurrentCart.ActionItems.Remove(itemId);
                DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "head-Event wave was not changed."));
                return RedirectToAction("viewuser", "user", new { userId = CurrentUser.UserId });
            }

            ServiceResult eventWaveChangeResult = _service.ChangeWave(registrationId, eventWaveId);

            if (eventWaveChangeResult.Success)
            {
                SessionManager.CurrentCart.ActionItems.Remove(itemId);
                DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "head-You have successfully updated your event wave."));
                return RedirectToAction("viewuser", "user", new { userId = CurrentUser.UserId });
            }
            return null;
        }

        private bool EventChanged(int? initialEventId, int? eventWaveId)
        {
            if (!eventWaveId.HasValue || !initialEventId.HasValue) 
                return true;

            var initialEvent = _service.GetEventWaveById(eventWaveId.Value);
            if (initialEvent == null)
                return true;

            if (initialEventId.Value == initialEvent.EventDate.EventId)
                return false;

            return true;
        }

        public JsonResult GetActiveEventList()
        {
            return Json(_service.GetSimpleActiveEventList().Select(x => new { EventId = x.EventId, Name = x.EndDate > x.StartDate ? string.Format("{0}, {1} : {2} - {3}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString(), x.EndDate.ToShortDateString()) : string.Format("{0}, {1} : {2}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString()) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventList()
        {
            return Json(_service.GetActiveUpcomingEvents().Select(x => new { EventId = x.EventId, Name = x.EndDate > x.StartDate ? string.Format("{0}, {1} : {2} - {3}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString(), x.EndDate.ToShortDateString()) : string.Format("{0}, {1} : {2}", x.GeneralLocality, x.StateCode, x.StartDate.ToShortDateString()) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventDateList(int eventId)
        {
            return Json(_service.GetSimpleDateDetailsByEvent(eventId).Select(x => new { EventDateId = x.EventDateId, DateOfEvent = x.DateOfEvent.ToString("D") }).ToList(), JsonRequestBehavior.AllowGet);
        }

        private int GetEventDateCount(int eventId)
        {            
            var dates = _service.GetActiveDateDetailsByEvent(eventId).Select(x => new { EventDateId = x.EventDateId, DateOfEvent = x.DateOfEvent.ToShortDateString() }).ToList();

            return dates.Count; 
        }

        public ActionResult GetWavesByEventDateId(int eventDateId)
        {
            var waves = _service.GetWaveDetialsForEventDate(eventDateId);

            var vm = new vmRegistration_WaveList();
            vm.MorningWaves = GetWaveItems(1, waves.Where(x => x.StartTime.ToString("tt") == "AM").ToList());
            vm.EveningWaves = GetWaveItems(vm.MorningWaves.Count()+1,
                                           waves.Where(x => x.StartTime.ToString("tt") == "PM").ToList());
            
            return PartialView("Partial/WaveList", vm);
        }

        private List<vmRegistration_WaveItem> GetWaveItems(int startingWaveNumber, List<EventWaveDetails> waveOverviewList)
        {

            var waveItemList = new List<vmRegistration_WaveItem>();
            int waveNumber = startingWaveNumber;

            foreach (var wave in waveOverviewList)
            {
                var newWave = new vmRegistration_WaveItem
                    {
                        EventWaveId = wave.EventWaveId,
                        WaveNumber = waveNumber++,
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

        #endregion

        #region Registration

        public ActionResult RegistrationDetails(Guid itemId) 
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            var regAction = SessionManager.CurrentCart.ActionItems[itemId];
            var reg = (Registration)regAction.ActionObject;

            // update address to current user address
            reg.Address1 = CurrentUser.Address1;
            reg.Address2 = CurrentUser.Address2;
            reg.Locality = CurrentUser.Locality;
            reg.RegionId = CurrentUser.RegionId;
            reg.PostalCode = CurrentUser.PostalCode;
            reg.Birthday = DateTime.Now.AddYears(-30).Date;
    
            var wave = _service.GetEventWaveById(reg.EventWaveId);
            int eventId = wave.EventDate.EventId;

            var tShirtSizeList = DirtyGirlExtensions.ConvertToSelectList<TShirtSize>();
            tShirtSizeList.RemoveAt(0);

            var allowSurvivors = (_service.GetSurvivorRegistrationsCountByEventDate(wave.EventDateId) < DirtyGirlServiceConfig.Settings.SurvivorSpots);

            var vm = new vmRegistration_Details 
                { 

                    EventWave = _service.GetEventWaveById(reg.EventWaveId),
                    EventOverview = _service.GetEventOverviewById(eventId),
                    RegionList = _service.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId),
                    RegistrationTypeList = DirtyGirlExtensions.ConvertToSelectList<RegistrationType>(),
                    TShirtSizeList = tShirtSizeList,
                    PacketDeliveryOptionList = DirtyGirlExtensions.ConvertToSelectList<RegistrationMaterialsDeliveryOption>(),
                    EventLeadList = _service.GetEventLeads(eventId, true),
                    RegistrationDetails = reg,
                    ItemId = itemId,
                    survivorSpotsAvailable = allowSurvivors
                };          

            return View(vm);
        }

        [HttpPost]
        public ActionResult RegistrationDetails(vmRegistration_Details model)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            var regAction = SessionManager.CurrentCart.ActionItems[model.ItemId];
            var reg = (Registration) regAction.ActionObject;

            if (_service.IsDuplicateRegistration(reg.EventWaveId, CurrentUser.UserId,
                                                 model.RegistrationDetails.FirstName, model.RegistrationDetails.LastName))
                ModelState.AddModelError("FirstName",
                                         "You have already registered for this event wave. You may select another wave above, or, if you would like to register another participant for this wave, please enter their name below.");

            var fullName = model.RegistrationDetails.FirstName + model.RegistrationDetails.LastName;
            if (fullName.Replace(" ","") == model.RegistrationDetails.EmergencyContact.Replace(" ", ""))
                ModelState.AddModelError("EmergencyContact", "Emergency contact cannot be the same as the registrant.");

            EventWave wave = _service.GetEventWaveById(reg.EventWaveId);

            if (model.RegistrationDetails.Birthday.HasValue)
            {
                if (model.RegistrationDetails.Birthday.Value.AddYears(14) > wave.EventDate.DateOfEvent )
                     ModelState.AddModelError("Birthday", "The participant must be 14 years or older to join the event..");
            }
            else
                ModelState.AddModelError("Birthday", "Registrants Birthday is required.");

            model.RegistrationDetails.Address1 = reg.Address1 = CurrentUser.Address1;
            model.RegistrationDetails.Address2 = reg.Address2 = CurrentUser.Address2;
            model.RegistrationDetails.Locality = reg.Locality = CurrentUser.Locality;
            model.RegistrationDetails.RegionId = reg.RegionId = CurrentUser.RegionId;
            model.RegistrationDetails.PostalCode = reg.PostalCode = CurrentUser.PostalCode;

            if (ModelState.IsValid)
            {
                reg.AgreeToTerms = model.RegistrationDetails.AgreeToTerms;
                reg.CartItemId = model.RegistrationDetails.CartItemId;
                reg.DateAdded = model.RegistrationDetails.DateAdded;
                reg.Email = model.RegistrationDetails.Email;
                reg.EmergencyContact = model.RegistrationDetails.EmergencyContact;
                reg.EmergencyPhone = model.RegistrationDetails.EmergencyPhone;
                reg.EventLeadId = model.RegistrationDetails.EventLeadId;
                reg.FirstName = model.RegistrationDetails.FirstName;
                reg.Gender = model.RegistrationDetails.Gender;
                reg.IsFemale = model.RegistrationDetails.IsFemale;
                reg.IsOfAge = model.RegistrationDetails.IsOfAge;
                reg.IsThirdPartyRegistration = model.RegistrationDetails.IsThirdPartyRegistration;
                reg.LastName = model.RegistrationDetails.LastName;
                reg.MedicalInformation = model.RegistrationDetails.MedicalInformation;
                reg.ParentRegistrationId = model.RegistrationDetails.ParentRegistrationId;
                reg.Phone = model.RegistrationDetails.Phone;
                reg.ReferenceAnswer = model.RegistrationDetails.ReferenceAnswer;
                reg.RegistrationStatus = RegistrationStatus.Active;
                reg.RegistrationType = model.RegistrationDetails.RegistrationType;
                reg.SpecialNeeds = model.RegistrationDetails.SpecialNeeds;
                reg.Birthday = model.RegistrationDetails.Birthday.Value.Date;
                reg.TShirtSize = model.RegistrationDetails.TShirtSize;
                reg.PacketDeliveryOption = (model.RegistrationDetails.PacketDeliveryOption.HasValue ? model.RegistrationDetails.PacketDeliveryOption : RegistrationMaterialsDeliveryOption.OnSitePickup );
                reg.UserId = CurrentUser.UserId;
                reg.Signature = model.RegistrationDetails.Signature;
                reg.IsIAmTheParticipant = model.RegistrationDetails.IsIAmTheParticipant;
                reg.IsSignatureConsent = model.RegistrationDetails.IsSignatureConsent;
                reg.AgreeTrademark = model.RegistrationDetails.AgreeTrademark;
                reg.ConfirmationCode = model.RegistrationDetails.ConfirmationCode;
                regAction.ActionObject = reg;
                regAction.ItemReadyForCheckout = true;

                
                // should check this better... 
                if (((int)reg.PacketDeliveryOption.Value == 1))
                {
                    ActionItem shippingFeeItem = _service.CreateShippingFee(model.ItemId, reg.EventWaveId, reg.PacketDeliveryOption);
                    SessionManager.CurrentCart.ActionItems.Add(Guid.NewGuid(), shippingFeeItem);
                }

                if (CheckAddProcessingFee(reg, model.ItemId) )
                {
                    ActionItem processingFeeItem = _service.CreateProcessingFee(model.ItemId, reg.EventWaveId,
                                                                                reg.PacketDeliveryOption);
                    SessionManager.CurrentCart.ActionItems.Add(Guid.NewGuid(), processingFeeItem);
                }
                else
                {
                    // check to see if the processing fee is already in the cart.  If so, we know we do not want it, so remove it.
                    RemoveProcessingFee(model.ItemId);
                }
                return RedirectToAction("checkout", "cart");
            }

           

            model.EventWave = wave;
            model.EventOverview = _service.GetEventOverviewById(wave.EventDate.EventId);
            model.RegionList = _service.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId);
            model.RegistrationTypeList = DirtyGirlExtensions.ConvertToSelectList<RegistrationType>();
            model.EventLeadList = _service.GetEventLeads(wave.EventDate.EventId, true);
            model.PacketDeliveryOptionList = DirtyGirlExtensions.ConvertToSelectList<RegistrationMaterialsDeliveryOption>();
            model.TShirtSizeList = DirtyGirlExtensions.ConvertToSelectList<TShirtSize>();
            model.TShirtSizeList.RemoveAt(0);

            return View(model);
        }

        private void RemoveProcessingFee(Guid itemId)
        {
            var processingActions = SessionManager.CurrentCart.ActionItems.Where(x => x.Value.ActionType == CartActionType.ProcessingFee).ToList();
            foreach (var proc in processingActions)
            {
                var processesAction = proc.Value.ActionObject as ProcessingFeeAction;
                if (processesAction != null && processesAction.RegItemGuid == itemId)
                {
                    SessionManager.CurrentCart.ActionItems.Remove(proc.Key);
                }
            }
        }

        private bool CheckAddProcessingFee(Registration reg, Guid itemId)
        {
            if (reg.RegistrationType == RegistrationType.CancerRegistration)
                return false;

            var processingActions = SessionManager.CurrentCart.ActionItems.Where(x => (x.Value as ActionItem).ActionType == CartActionType.ProcessingFee).ToList();
            return processingActions.Select(proc => proc.Value.ActionObject as ProcessingFeeAction).All(processesAction => processesAction.RegItemGuid != itemId);
        }

        #endregion

        #region Create Team

        public ActionResult CreateTeam(Guid itemId)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            ViewBag.showTeamCode = "false";
            var reg = (Registration)SessionManager.CurrentCart.ActionItems[itemId].ActionObject;
            var wave = _service.GetEventWaveById(reg.EventWaveId);

            var vm = new vmRegistration_CreateTeam
                {
                    ItemId = itemId,
                    EventId = wave.EventDate.EventId,
                    EventOverview = _service.GetEventOverviewById(wave.EventDate.EventId)
                };         

            if (reg.TeamId.HasValue)
            {
                vm.RegistrationType = "team";
                vm.TeamType = "existing";
                vm.TeamCode = _service.GetTeamById(reg.TeamId.Value).Code;
            }
            else if (reg.Team != null)
            {
                vm.RegistrationType = "team";
                vm.TeamType = "new";
                vm.TeamName = reg.Team.Name;
            }
            else
            {
                vm.RegistrationType = "individual";
                vm.TeamType = "new";
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateTeam(vmRegistration_CreateTeam model)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            ViewBag.showTeamCode = "false";
            if (ModelState.IsValid)
            {
                
                var reg = (Registration)SessionManager.CurrentCart.ActionItems[model.ItemId].ActionObject;

                if (model.RegistrationType.ToLower() == "team")
                {
                    switch (model.TeamType.ToLower())
                    {
                        case "existing":
                            JoinExistingTeam(model, reg);
                            break;

                        case "new":
                            CreateNewTeam(model, reg);
                            break;
                    }
                }
                else
                {
                    reg.Team = null;
                    reg.TeamId = null;
                }                

                if (ModelState.IsValid)
                    return RedirectToAction("RegistrationDetails", new { itemId = model.ItemId });
                
            }

            model.EventOverview = _service.GetEventOverviewById(model.EventId);
            return View(model);
        }

        private void JoinExistingTeam(vmRegistration_CreateTeam model, Registration reg)
        {
            if (string.IsNullOrEmpty(model.TeamCode))
            {
                ModelState.AddModelError("TeamCode", "Team Code is Required.");
                ViewBag.showTeamCode = "true";
                return;
            }

            var existingTeam = _service.GetTeamByCode(model.EventId, model.TeamCode);

            if (existingTeam != null)
                reg.TeamId = existingTeam.TeamId;
            else
                ModelState.AddModelError("TeamCode", "There is no team using this code for this event.");
        }

        private void CreateNewTeam(vmRegistration_CreateTeam model, Registration reg)
        {
            if (string.IsNullOrEmpty(model.TeamName))
            {
                ModelState.AddModelError("TeamName", "Team Name is Required");
                return;
            }

            Match match = Regex.Match(model.TeamName, @"([a-zA-Z].*?){3}", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                ModelState.AddModelError("TeamName", "Team Name must contain at least 3 letters.");
                return;
            }

            Team newTeam = new Team {EventId = model.EventId, Name = model.TeamName, CreatorID = CurrentUser.UserId};

            ServiceResult tempTeamResult = _service.GenerateTempTeam(newTeam);

            if (!tempTeamResult.Success)
                Utilities.AddModelStateErrors(ModelState, tempTeamResult.GetServiceErrors());
            else
                reg.Team = newTeam;
                
        }

        public JsonResult ValidateTeamCode(int eventId, string code)
        {
            var team = _service.GetTeamByCode(eventId, code);
            return team != null ? 
                Json(new { Name = team.Name }, JsonRequestBehavior.AllowGet) : 
                Json(new {Status = "Failure"}, JsonRequestBehavior.AllowGet);
        }

        #endregion        

        #region Transfer

        public ActionResult Transfer(Guid itemId)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            TransferAction action = (TransferAction)SessionManager.CurrentCart.ActionItems[itemId].ActionObject;            
            Registration existingReg = _service.GetRegistrationById(action.RegistrationId);

            var vm = new vmRegistration_Transfer
                {
                    ItemId = itemId,
                    FirstName = action.FirstName,
                    LastName = action.LastName,
                    Email = action.Email,
                    EventOverview = _service.GetEventOverviewById(existingReg.EventWave.EventDate.EventId)
                };           
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult Transfer(vmRegistration_Transfer model)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            var action = SessionManager.CurrentCart.ActionItems[model.ItemId];
            var transfer = (TransferAction)action.ActionObject;
            bool isValid = true; 

            // make sure to not transfer to self. 
            if (model.FirstName.Replace(" ", "") == CurrentUser.FirstName.Replace(" ", "") && model.LastName.Replace(" ", "") == CurrentUser.LastName.Replace(" ", ""))
            {
                List<ServiceError> errors = new List<ServiceError>();
                errors.Add(new ServiceError("Can't transfer run to yourself..."));
                Utilities.AddModelStateErrors(ModelState, errors);
                isValid = false; 
            }

            if (ModelState.IsValid && isValid)
            {                
                transfer.FirstName = model.FirstName;
                transfer.LastName = model.LastName;
                transfer.Email = model.Email;
                action.ItemReadyForCheckout = true;

                return RedirectToAction("checkout", "cart");
            }

            Registration existingReg = _service.GetRegistrationById(transfer.RegistrationId);
            model.EventOverview = _service.GetEventOverviewById(existingReg.EventWave.EventDate.EventId);          

            return View(model);
        }

        public ActionResult InvalidRedemption(string redemptionError)
        {
            ViewData["InvalidRedemption"] = redemptionError;
            return View();
        }
        #endregion
        
    }
}
