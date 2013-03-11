using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IEventService
    {

        #region Events

        IList<Event> GetAllEvents();

        IList<Event> GetAllUpcomingEvents();

        IList<Event> GetActiveUpcomingEvents();

        IList<EventDetails> GetActiveUpcomingEventDetails();

        IList<EventDetails> GetAllUpcomingEventDetails();

        IList<Event> GetEventsInDateRange(DateTime begin, DateTime end);

        IList<EventOverview> GetUpcomingEventOverviews();
        
     
        EventDetails GetEventDetails(int eventId);

        EventDetails SetEventDetail(Event e);

        Event GetEventById(int eventId);

        EventOverview GetEventOverviewById(int eventId);

        EventOverview SetEventOverview(Event e);

        ServiceResult CreateEvent(Event e);

        ServiceResult CreateEventByTemplate(CreateNewEvent e);

        ServiceResult UpdateEvent(Event e);

        ServiceResult RemoveEvent(Event e);


        #endregion

        #region Regions

        IList<Region> GetRegionsByCountry(int countryId);

        #endregion

        #region Event Dates

        IList<EventDate> GetDatesForEvent(int eventId);

        IList<EventDateDetails> GetActiveDateDetailsByEvent(int eventId);

        IList<EventDateOverview> GetActiveEventDateOverviews(int? regionId, int? month, int? year, string sort, string direction);    

        ServiceResult CreateEventDate(EventDate ed);

        ServiceResult UpdateEventDate(EventDate ed);

        ServiceResult RemoveEventDate(int eventDateId);

        ServiceResult GenerateEventDate(int eventId, DateTime eventDate, DateTime startTime, DateTime endTime, int duration, int maxRegistrants);

        #endregion

        #region Event Waves

        IList<EventWave> GetWavesForEventDate(int eventWaveId);

        IList<EventWaveDetails> GetWaveDetailsForEventDate(int eventDateId);

        EventWave GetEventWaveById(int eventWaveId);

        ServiceResult CreateEventWave(EventWave ew);

        ServiceResult UpdateEventWave(EventWave ew);

        ServiceResult RemoveEventWave(int eventWaveId);       

        #endregion

        #region Event Sponsors

        IList<EventSponsor> GetSponsorsForEvent(int eventId);

        ServiceResult CreateEventSponsor(EventSponsor sponsor);

        ServiceResult UpdateEventSponsor(EventSponsor sponsor);

        ServiceResult RemoveEventSponsor(int sponsorId);

        ServiceResult RemoveEventSponsor(int eventId, string fileName);

        #endregion

        #region Event Templates

        IList<EventTemplate> GetEventTemplates();

        EventTemplate GetEventTemplateById(int templateId);

        #endregion

        #region Event Fees

        IList<EventFee> GetFeesForEvent(int eventId);

        EventFee GetCurrentFeeForEvent(int eventId, EventFeeType feeType);

        EventFee GetCurrentFeeForWave(int eventWaveId, EventFeeType feeType);

        ServiceResult CreateEventFee(EventFee fee);

        ServiceResult UpdateEventFee(EventFee fee);

        ServiceResult RemoveEventFee(int purchaseItemId);

        EventFee GetFeeForEventByRegistrationDate(int eventId, EventFeeType feeType, DateTime registrationDate);

        #endregion

        #region Event Leads

        IList<EventLead> GetEventLeads();

        IList<EventLead> GetEventLeads(int eventId, bool includeGlobalLeads);

        IList<EventLeadType> GetEventLeadTypes();
            
        ServiceResult CreateEventLead(EventLead eventLead);

        ServiceResult UpdateEventLead(EventLead eventLead);

        ServiceResult RemoveEventLead(int eventLeadId);

        #endregion

        #region Event Coupons


        #endregion
    }
}
