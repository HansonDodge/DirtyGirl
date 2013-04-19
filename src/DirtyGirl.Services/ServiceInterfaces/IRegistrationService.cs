using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IRegistrationService
    {
        #region registrations

        IList<Registration> GetRegistrationByUserID(int userId);        
        IList<Registration> GetRegistrationsByEvent(int EventId);
        IList<Registration> GetRegistrationsByEventDate(int EventDateId);
        int GetSurvivorRegistrationsCountByEventDate(int EventDateId);
        IList<Registration> GetRegistrationByEventWave(int EventWaveId);        
        IList<Registration> GetRegistrationsByTeam(int teamId);
        IList<Registration> GetRegistrationsAll();

        bool IsDuplicateRegistration(int eventWaveId, int userId, string fname, string lname);

        ServiceResult CreateNewRegistration(Registration r);
        ServiceResult CreateNewRegistration(Registration r, int? redemptionId);
        ServiceResult UpdateRegistration(Registration r);        
        ServiceResult TransferRegistration(int existingRegistrationId, string name, string email);
        ServiceResult CancelRegistration(int registrationId);
        ServiceResult ChangeWave(int registrationId, int eventWaveId);
        ServiceResult ChangeEvent(int registrationId, int eventWaveId, int? cartItemId, string confirmationCode);
        ServiceResult RedemptionCodeRegistration(string redemptionCode, Registration r);

        Registration GetRegistrationById(int registrationId);
        Registration GetBaseRegistration(int registrationId);

        decimal GetRegistrationPathValue(int registrationId);
        ActionItem CreateShippingFee(Guid regItemGuid, int eventWaveId, RegistrationMaterialsDeliveryOption? deliveryOption);
        ActionItem CreateProcessingFee(Guid regItemGuid, int eventWaveId,RegistrationMaterialsDeliveryOption? deliveryOption);

        #endregion

        #region RegistrationLeads

        IList<EventLead> GetEventLeads();
        IList<EventLead> GetEventLeads(int eventId, bool includeGlobalLeads);

        #endregion

        #region Regions

        IList<Region> GetRegionsByCountry(int countryId);

        #endregion

        #region Event

        IList<EventBasics> GetSimpleActiveEventList();
        IList<EventDetails> GetActiveUpcomingEvents();
        IList<EventDateDetails> GetSimpleDateDetailsByEvent(int eventId);
        IList<EventDateDetails> GetActiveDateDetailsByEvent(int eventId);
        IList<EventWaveDetails> GetWaveDetialsForEventDate(int eventDateId);

        Event GetEventById(int eventId);
        EventOverview GetEventOverviewById(int eventId);
        EventDate GetEventDateById(int eventDateId);
        EventWave GetEventWaveById(int eventWaveId);

        EventFee GetCurrentEventFeeForWave(int eventWaveId, EventFeeType feeType);

        #endregion
        
        #region team

        Team GetTeamByCode(int eventId, string teamCode);
        Team GetTeamById(int teamId);
        ServiceResult GenerateTempTeam(Team newTeam);   

        #endregion

        #region Save

        ServiceResult Save();        

        #endregion

    }
}
