using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DirtyGirl.Services
{
    public class RegistrationService : ServiceBase, IRegistrationService
    {

        #region constructor

        public RegistrationService(IRepositoryGroup repository) : base(repository, false) { }
        public RegistrationService(IRepositoryGroup repository, bool isShared) : base(repository, isShared) { }

        #endregion

        #region Validation

        private bool ValidateRegistration(Registration registrationToValidate, ServiceResult result)
        {            
            return result.Success;
        }

        #endregion

        #region Registration

        public IList<Registration> GetRegistrationsAll()
        {
            return _repository.Registrations.All().ToList();
        }

        public IList<Registration> GetRegistrationByUserID(int userId)
        {
            var registrations = _repository.Registrations.Filter(r => r.UserId.Equals(userId)).ToList();
            return registrations;
        }        

        public IList<Registration> GetRegistrationsByEvent(int EventId)
        {
           return _repository.Registrations.Filter(x => x.EventWave.EventDate.EventId == EventId).ToList();
        }

        public int GetSurvivorRegistrationsCountByEventDate(int EventDateId)
        {
            return _repository.Registrations.Filter(x => x.EventWave.EventDateId == EventDateId && 
                                                    x.RegistrationStatus == RegistrationStatus.Active && 
                                                    x.RegistrationType == RegistrationType.CancerRegistration).Count();
        }

        public IList<Registration> GetRegistrationsByEventDate(int EventDateId)
        {
            return _repository.Registrations.Filter(x => x.EventWave.EventDateId == EventDateId).ToList();
        }

        public IList<Registration> GetRegistrationByEventWave(int EventWaveId)
        {
            return _repository.Registrations.Filter(x => x.EventWaveId == EventWaveId).ToList();
        }
        
        public IList<Registration> GetRegistrationsByTeam(int teamId)
        {
            return _repository.Registrations.Filter(r => r.TeamId == teamId).ToList();            
        }

        public bool IsDuplicateRegistration(int eventWaveId, int userId, string fname, string lname)
        {
            fname = fname.ToLower().Replace(" ", string.Empty);
            lname = lname.ToLower().Replace(" ", string.Empty);
            var existingRegistrations = _repository.Registrations.All().Where(x => x.EventWaveId == eventWaveId
                                                    && x.UserId == userId
                                                    && x.RegistrationStatus == RegistrationStatus.Active
                                                    && x.FirstName.ToLower().Replace(" ", string.Empty) == fname
                                                    && x.LastName.ToLower().Replace(" ", string.Empty) == lname );
            return existingRegistrations.Count() > 0;
        }
        
        public ServiceResult CreateNewRegistration(Registration r)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateRegistration(r, result))
                {
                    _repository.Registrations.Create(r);

                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult CreateNewRegistration(Registration r, int? redemtpionId)
        {
            var result = new ServiceResult();

            try
            {
                result = CreateNewRegistration(r);

                if (result.Success)
                {
                    //Check to see if the discount id being passed in is a redemption code, if it is, update it.
                    var code = _repository.RedemptionCodes.Find(x => x.DiscountItemId == redemtpionId);

                    if (code != null)
                    {
                        code.GeneratingRegistration.RegistrationStatus = code.RedemptionCodeType == RedemptionCodeType.Transfer ? RegistrationStatus.Transferred : RegistrationStatus.Cancelled;
                        code.ResultingRegistrationId = r.RegistrationId;
                        r.ParentRegistrationId = code.GeneratingRegistrationId;

                        if (!_sharedRepository)
                            _repository.SaveChanges();
                    }
                }

            }
            catch(Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult UpdateRegistration(Registration r)
        {
            var result = new ServiceResult();

            try
            {
                var target = _repository.Registrations.Find(x => x.RegistrationId == r.RegistrationId);

                if (ValidateRegistration(r, result))
                {
                    target.Address1 = r.Address1;
                    target.Address2 = r.Address2;
                    target.AgreeToTerms = r.AgreeToTerms;
                    target.AgreeTrademark = r.AgreeTrademark;
                    target.Birthday = r.Birthday;
                    target.CartItemId = r.CartItemId;                    
                    target.Email = r.Email;
                    target.EmergencyContact = r.EmergencyContact;
                    target.EmergencyPhone = r.EmergencyPhone;
                    target.EventLeadId = r.EventLeadId;
                    target.EventWaveId = r.EventWaveId;
                    target.FirstName = r.FirstName;
                    target.Gender = r.Gender;
                    target.IsFemale = r.IsFemale;
                    target.IsOfAge = r.IsOfAge;
                    target.IsThirdPartyRegistration = r.IsThirdPartyRegistration;
                    target.LastName = r.LastName;
                    target.Locality = r.Locality;
                    target.MedicalInformation = r.MedicalInformation;
                    target.ParentRegistrationId = r.ParentRegistrationId;
                    target.Phone = r.Phone;
                    target.PostalCode = r.PostalCode;
                    target.PacketDeliveryOption = r.PacketDeliveryOption;
                    target.ReferenceAnswer = r.ReferenceAnswer;
                    target.RegionId = r.RegionId;
                    target.RegistrationStatus = r.RegistrationStatus;
                    target.RegistrationType = r.RegistrationType;
                    target.SpecialNeeds = r.SpecialNeeds;
                    target.TShirtSize = r.TShirtSize;
                    target.TeamId = r.TeamId;
                    target.UserId = r.UserId;
                    target.DateUpdated = DateTime.Now;
                    target.ConfirmationCode = r.ConfirmationCode;

                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult TransferRegistration(int existingRegistrationId, string name, string email)
        {
            ServiceResult result = null;

            IEmailService emailService = new EmailService();
            var discountService = new DiscountService(this._repository, false);

            Registration transferReg = _repository.Registrations.Find(x => x.RegistrationId == existingRegistrationId);
            transferReg.RegistrationStatus = RegistrationStatus.Held;
            transferReg.DateUpdated = DateTime.Now;

            var newTransferCode = new RedemptionCode
            {
                GeneratingRegistrationId = existingRegistrationId,
                Code = discountService.GenerateDiscountCode(),
                RedemptionCodeType = RedemptionCodeType.Transfer,
                Value = GetRegistrationPathValue(existingRegistrationId),
                DiscountType = DiscountType.Dollars
            };

            result = discountService.CreateRedemptionCode(newTransferCode);

            if (!_sharedRepository)
            {
                _repository.SaveChanges();
                emailService.SendTransferEmail(newTransferCode.DiscountItemId, name, email);
            }            

            return result;
        }

        public ServiceResult CancelRegistration(int existingRegistrationId)
        {

            var result = new ServiceResult();
            IEmailService emailService = new EmailService();
            IDiscountService discountService = new DiscountService(this._repository, false);

            Registration cancelReg = _repository.Registrations.Find(x => x.RegistrationId == existingRegistrationId);
            cancelReg.RegistrationStatus = RegistrationStatus.Cancelled;
            cancelReg.DateUpdated = DateTime.Now;

            var newTransferCode = new RedemptionCode
            {
                GeneratingRegistrationId = existingRegistrationId,
                Code = discountService.GenerateDiscountCode(),
                RedemptionCodeType = RedemptionCodeType.StraightValue,
                Value = GetRegistrationPathValue(existingRegistrationId),
                DiscountType = DiscountType.Dollars
            };

            result = discountService.CreateRedemptionCode(newTransferCode);

            if (!_sharedRepository)
            {
                _repository.SaveChanges();
                emailService.SendCancellationEmail(newTransferCode.DiscountItemId);
            }            


            return result;
        }

        public ServiceResult ChangeWave(int registrationId, int eventWaveId)
        {
            var result = new ServiceResult();

            try
            {
                Registration existingReg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);
                int eventId = existingReg.EventWave.EventDate.EventId;    
                int newEventId = GetEventWaveById(eventWaveId).EventDate.EventId;

                if (existingReg.TeamId != null && eventId != newEventId)
                    existingReg.TeamId = null;

                existingReg.EventWaveId = eventWaveId;
                existingReg.DateUpdated = DateTime.Now;

                if (!_sharedRepository)
                    _repository.SaveChanges();

            }
            catch(Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;

        }

        public ServiceResult ChangeEvent(int registrationId, int eventWaveId, int? cartItemId, string confirmationCode)
        {
            var result = new ServiceResult();

            try
            {                
                Registration existingReg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);
                existingReg.RegistrationStatus = RegistrationStatus.Changed;
                existingReg.DateUpdated = DateTime.Now;
                
                var newReg = new Registration
                    {
                        EventWaveId = eventWaveId,
                        RegistrationStatus = RegistrationStatus.Active,
                        RegistrationType = existingReg.RegistrationType,
                        ParentRegistrationId = existingReg.RegistrationId,
                        FirstName = existingReg.FirstName,
                        LastName = existingReg.LastName,
                        Address1 = existingReg.Address1,
                        Address2 = existingReg.Address2,
                        Locality = existingReg.Locality,
                        RegionId = existingReg.RegionId,
                        PostalCode = existingReg.PostalCode,
                        PacketDeliveryOption = existingReg.PacketDeliveryOption,
                        Email = existingReg.Email,
                        Phone = existingReg.Phone,
                        EmergencyContact = existingReg.EmergencyContact,
                        EmergencyPhone = existingReg.EmergencyPhone,
                        SpecialNeeds = existingReg.SpecialNeeds,
                        TShirtSize = existingReg.TShirtSize,
                        Gender = existingReg.Gender,
                        UserId = existingReg.UserId,
                        CartItemId = cartItemId,
                        AgreeToTerms = existingReg.AgreeToTerms,
                        AgreeTrademark = existingReg.AgreeTrademark,
                        IsFemale = existingReg.IsFemale,
                        IsOfAge = existingReg.IsOfAge,
                        EventLeadId = existingReg.EventLeadId,
                        IsSignatureConsent = existingReg.IsSignatureConsent,
                        IsThirdPartyRegistration = existingReg.IsThirdPartyRegistration,
                        Signature = existingReg.Signature,
                        Birthday = existingReg.Birthday,
                        IsIAmTheParticipant = existingReg.IsIAmTheParticipant
                    };
                if (string.IsNullOrEmpty(confirmationCode))
                    newReg.ConfirmationCode = existingReg.ConfirmationCode;
                else
                    newReg.ConfirmationCode = confirmationCode;

                _repository.Registrations.Create(newReg);

                if (!_sharedRepository)
                    _repository.SaveChanges();

            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        public ServiceResult RedemptionCodeRegistration(string code, Registration r)
        {
            var result = new ServiceResult();


            return result;
        }

        public Registration GetRegistrationById(int registrationId)
        {
            return _repository.Registrations.Find(x => x.RegistrationId == registrationId);
        }

        public Registration GetBaseRegistration(int registrationId)
        {
            Registration reg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);

            if (reg.ParentRegistrationId.HasValue)
                return GetBaseRegistration(reg.ParentRegistrationId.Value);

            return reg;
        }

        public decimal GetRegistrationPathValue(int registrationId)
        {
            
            Registration reg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);
            decimal val = 0;

            while (reg != null)
            {
                if (reg.CartItemId != null)
                {
                    var feePaid = (EventFee)reg.CartItem.PurchaseItem;

                    if (feePaid.EventFeeType == EventFeeType.Registration)
                        val += feePaid.Cost;                    
                    else
                    {
                        foreach (var item in reg.CartItem.Cart.CartItems)
                        {
                            if (item.PurchaseItemId == 1)
                            {
                                val += item.Cost;
                                break;
                            }
                        }
                    }
                }

                reg = _repository.Registrations.Find(x => x.RegistrationId == reg.ParentRegistrationId);
            }
            
            return val;
        }

        public ActionItem CreateShippingFee(Guid regItemGuid, int eventWaveId, RegistrationMaterialsDeliveryOption? deliveryOption)
        {

            var newAction = new ShippingFeeAction {
                RegItemGuid = regItemGuid,
                EventWaveId = eventWaveId 
            };
            var newCartItem = new ActionItem
            {
                ActionType = CartActionType.ShippingFee,
                ActionObject = newAction,
                ItemReadyForCheckout = true
            };

            return newCartItem;
        }

        public ActionItem CreateProcessingFee(Guid regItemGuid, int eventWaveId, RegistrationMaterialsDeliveryOption? deliveryOption)
        {

            var newAction = new ProcessingFeeAction
            {
                RegItemGuid = regItemGuid,
                EventWaveId = eventWaveId
            };
            var newCartItem = new ActionItem
            {
                ActionType = CartActionType.ProcessingFee,
                ActionObject = newAction,
                ItemReadyForCheckout = true
            };

            return newCartItem;
        }

        #endregion

        #region Registration Leads

        public IList<EventLead> GetEventLeads()
        {
            return _repository.EventLeads.Filter(x => x.EventId.HasValue == false).ToList();
        }

        public IList<EventLead> GetEventLeads(int eventId, bool includeGlobalLeads)
        {
            return includeGlobalLeads
                       ? _repository.EventLeads.Filter(x => x.EventId.HasValue == false || (x.EventId.Value == eventId)).OrderBy(x => x.DisplayText).ToList()
                       : _repository.EventLeads.Filter(x => (x.EventId.HasValue && x.EventId == eventId)).OrderBy(x => x.DisplayText).ToList();
        }

        #endregion

        #region Region Items

        public IList<Region> GetRegionsByCountry(int countryId)
        {
            RegionService regionService = new RegionService(this._repository);
            return regionService.GetRegionsByCountryId(countryId).OrderBy(x => x.Name).ToList();
        }

        #endregion

        #region event
        public IList<EventBasics> GetSimpleActiveEventList()
        {
            IEventService eventService = new EventService(this._repository);
            return eventService.GetSimpleActiveEventList();
        }

        public IList<EventDetails> GetActiveUpcomingEvents()
        {
            IEventService eventService = new EventService(this._repository);
            return eventService.GetAllUpcomingEventDetails();
        }

        public Event GetEventById(int eventId)
        {
            return _repository.Events.Find(x => x.EventId == eventId);
        }

        public EventOverview GetEventOverviewById(int eventId)
        {
            EventService eventService = new EventService(this._repository);
            return eventService.GetEventOverviewById(eventId);
        }

        public EventWave GetEventWaveById(int eventWaveId)
        {
            return _repository.EventWaves.Find(x => x.EventWaveId == eventWaveId);
        }

        public EventDate GetEventDateById(int eventDateId)
        {
            return _repository.EventDates.Find(x => x.EventDateId == eventDateId);
        }

        public EventFee GetCurrentEventFeeForWave(int eventWaveId, EventFeeType feeType)
        {
            var eventService = new EventService(this._repository, false);
            return eventService.GetCurrentFeeForWave(eventWaveId, feeType);
        }

        public EventFee GetShippingFeeForEvent(int eventId)
        {
            var eventService = new EventService(this._repository, false);
            return eventService.GetCurrentFeeForEvent(eventId, EventFeeType.Shipping);
        }

        public IList<EventDateDetails> GetSimpleDateDetailsByEvent(int eventId)
        {
            var eventService = new EventService(this._repository);
            return eventService.GetSimpleDateDetailsByEvent(eventId);
        }

        public IList<EventDateDetails> GetActiveDateDetailsByEvent(int eventId)
        {
            var eventService = new EventService(this._repository);
            return eventService.GetActiveDateDetailsByEvent(eventId);
        }

        public IList<EventWaveDetails> GetWaveDetialsForEventDate(int eventDateId)
        {
            var eventService = new EventService(this._repository);
            return eventService.GetWaveDetailsForEventDate(eventDateId);
        }

        #endregion

        #region Team

        public Team GetTeamByCode(int eventId, string teamCode)
        {
            return _repository.Teams.Find(x => x.EventId == eventId && x.Code == teamCode);
        }

        public Team  GetTeamById(int teamId)
        {
            return _repository.Teams.Find(x => x.TeamId == teamId);
        }

        public ServiceResult GenerateTempTeam(Team newTeam)
        {
            var result = new ServiceResult();

            try
            {               
                ITeamService teamService = new TeamService(_repository);
                if (!teamService.CheckTeamNameForDirtyWords(newTeam.Name))
                {
                    result.AddServiceError("TeamName", "Team name contains a naughty word.");
                }
                else
                {
                    if (teamService.CheckTeamNameAvailability(newTeam.EventId, newTeam.Name))
                        newTeam.Code = teamService.GenerateTeamCode(newTeam.EventId);
                    else
                        result.AddServiceError("TeamName", "Team name is already taken for this event.");
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }


            return result;

        }

        #endregion

        #region Save

        public ServiceResult Save()
        {
            var result = new ServiceResult();

            try
            {
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        #endregion
       
    }
}
