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

        public IList<Registration> GetRegistrationByUserID(int UserId)
        {
            var registrations = _repository.Registrations.Filter(r => r.UserId.Equals(UserId)).ToList();
            return registrations;
        }        

        public IList<Registration> GetRegistrationsByEvent(int EventId)
        {
           return _repository.Registrations.Filter(x => x.EventWave.EventDate.EventId == EventId).ToList();
        }

        public IList<Registration> GetRegistrationsByEventDate(int EventDateId)
        {
            return _repository.Registrations.Filter(x => x.EventWave.EventDateId == EventDateId).ToList();
        }

        public IList<Registration> GetRegistrationByEventWave(int EventWaveId)
        {
            return _repository.Registrations.Filter(x => x.EventWaveId == EventWaveId).ToList();
        }
        
        public IList<Registration> GetRegistrationsByTeam(int TeamId)
        {
            return _repository.Registrations.Filter(r => r.TeamId == TeamId).ToList();            
        }
        
        public ServiceResult CreateNewRegistration(Registration r)
        {
            ServiceResult result = new ServiceResult();

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
            ServiceResult result = new ServiceResult();

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
            ServiceResult result = new ServiceResult();

            try
            {
                var target = _repository.Registrations.Find(x => x.RegistrationId == r.RegistrationId);

                if (ValidateRegistration(r, result))
                {
                    target.Address1 = r.Address1;
                    target.Address2 = r.Address2;
                    target.AgreeToTerms = r.AgreeToTerms;
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
                    target.ReferenceAnswer = r.ReferenceAnswer;
                    target.RegionId = r.RegionId;
                    target.RegistrationStatus = r.RegistrationStatus;
                    target.RegistrationType = r.RegistrationType;
                    target.SpecialNeeds = r.SpecialNeeds;
                    target.TeamId = r.TeamId;
                    target.UserId = r.UserId;
                    target.DateUpdated = DateTime.Now;

                    if (!this._sharedRepository)
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
            ServiceResult result = new ServiceResult();
            IEmailService emailService = new EmailService();
            IDiscountService discountService = new DiscountService(this._repository, false);

            Registration transferReg = _repository.Registrations.Find(x => x.RegistrationId == existingRegistrationId);
            transferReg.RegistrationStatus = RegistrationStatus.Held;
            transferReg.DateUpdated = DateTime.Now;

            RedemptionCode newTransferCode = new RedemptionCode
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

            ServiceResult result = new ServiceResult();
            IEmailService emailService = new EmailService();
            IDiscountService discountService = new DiscountService(this._repository, false);

            Registration cancelReg = _repository.Registrations.Find(x => x.RegistrationId == existingRegistrationId);
            cancelReg.RegistrationStatus = RegistrationStatus.Cancelled;
            cancelReg.DateUpdated = DateTime.Now;

            RedemptionCode newTransferCode = new RedemptionCode
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
            ServiceResult result = new ServiceResult();

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

        public ServiceResult ChangeEvent(int registrationId, int eventWaveId, int? cartItemId)
        {
            ServiceResult result = new ServiceResult();

            try
            {                
                Registration existingReg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);
                existingReg.RegistrationStatus = RegistrationStatus.Changed;
                existingReg.DateUpdated = DateTime.Now;

                Registration newReg = new Registration();

                newReg.EventWaveId = eventWaveId;
                newReg.RegistrationStatus = RegistrationStatus.Active;
                newReg.RegistrationType = existingReg.RegistrationType;
                newReg.ParentRegistrationId = existingReg.RegistrationId;
                newReg.FirstName = existingReg.FirstName;
                newReg.LastName = existingReg.LastName;
                newReg.Address1 = existingReg.Address1;
                newReg.Address2 = existingReg.Address2;
                newReg.Locality = existingReg.Locality;
                newReg.RegionId = existingReg.RegionId;
                newReg.PostalCode = existingReg.PostalCode;
                newReg.Email = existingReg.Email;
                newReg.Phone = existingReg.Phone;
                newReg.EmergencyContact = existingReg.EmergencyContact;
                newReg.EmergencyPhone = existingReg.EmergencyPhone;
                newReg.SpecialNeeds = existingReg.SpecialNeeds;
                newReg.Gender = existingReg.Gender;                
                newReg.UserId = existingReg.UserId;
                newReg.CartItemId = cartItemId;
                newReg.AgreeToTerms = existingReg.AgreeToTerms;
                newReg.IsFemale = existingReg.IsFemale;
                newReg.IsOfAge = existingReg.IsOfAge;
                newReg.EventLeadId = existingReg.EventLeadId;
                newReg.IsThirdPartyRegistration = existingReg.IsThirdPartyRegistration;
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
            ServiceResult result = new ServiceResult();


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
            EventService eventService = new EventService(this._repository, false);
            return eventService.GetCurrentFeeForWave(eventWaveId, feeType);
        }       

        public IList<EventDateDetails> GetActiveDateDetailsByEvent(int eventId)
        {
            EventService eventService = new EventService(this._repository);
            return eventService.GetActiveDateDetailsByEvent(eventId);
        }

        public IList<EventWaveDetails> GetWaveDetialsForEventDate(int eventDateId)
        {
            EventService eventService = new EventService(this._repository);
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
            ServiceResult result = new ServiceResult();

            try
            {               
                ITeamService teamService = new TeamService(_repository);               

                if (teamService.CheckTeamNameAvailability(newTeam.EventId, newTeam.Name))
                    newTeam.Code = teamService.GenerateTeamCode(newTeam.EventId);
                else
                    result.AddServiceError("TeamName", "Team name is already taken for this event.");
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
            ServiceResult result = new ServiceResult();

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
