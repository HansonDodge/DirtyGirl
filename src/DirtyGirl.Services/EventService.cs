using System;
using System.Collections.Generic;
using System.Linq;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;

namespace DirtyGirl.Services
{
    public class EventService : ServiceBase, IEventService
    {
        #region Constructor

        public EventService(IRepositoryGroup repository) : base(repository, false)
        {
        }

        public EventService(IRepositoryGroup repository, bool isSharedRepository) : base(repository, isSharedRepository)
        {
        }

        #endregion

        #region Validation

        private bool ValidateEvent(Event eventToValidate)
        {
            const bool isValid = true;


            return isValid;
        }

        private bool ValidateEventDate(EventDate dateToValidate, ServiceResult result)
        {
            if (dateToValidate.DateOfEvent.Date < DateTime.Now.Date)
                result.AddServiceError("DateOfEvent", "You can not add an event date that is in the past");
            else
            {
                if (
                    _repository.EventDates.Find(
                        x =>
                        dateToValidate.EventDateId != x.EventDateId && dateToValidate.DateOfEvent == x.DateOfEvent &&
                        dateToValidate.EventId == x.EventId) != null)
                    result.AddServiceError("DateOfEvent", "This date is already attached to this event.");
            }

            return result.Success;
        }

        private bool ValidateEventWave(EventWave waveToValidate, ServiceResult result)
        {
            EventDate eventDate = _repository.EventDates.Find(x => x.EventDateId == waveToValidate.EventDateId);

            waveToValidate.StartTime = new DateTime(eventDate.DateOfEvent.Year, eventDate.DateOfEvent.Month,
                                                    eventDate.DateOfEvent.Day, waveToValidate.StartTime.Hour,
                                                    waveToValidate.StartTime.Minute, waveToValidate.StartTime.Second);
            waveToValidate.EndTime = new DateTime(eventDate.DateOfEvent.Year, eventDate.DateOfEvent.Month,
                                                  eventDate.DateOfEvent.Day, waveToValidate.EndTime.Hour,
                                                  waveToValidate.EndTime.Minute, waveToValidate.EndTime.Second);


            return result.Success;
        }

        private bool ValidateEventLead(EventLead eventLead, ServiceResult result)
        {
            return result.Success;
        }

        private bool ValidateEventFee(EventFee feeToValidate, ServiceResult result)
        {
            return result.Success;
        }

        private bool ValidateSponsor(EventSponsor sponsorToValidate, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanGenerateWaves(EventDate eventDate, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanRemoveEventDate(EventDate dateToRemove, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanRemoveEventWave(EventWave waveToRemove, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanRemoveEventFee(EventFee feeToValidate, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanRemoveSponsor(EventSponsor sponsorToValidate, ServiceResult result)
        {
            return result.Success;
        }

        private bool CanRemoveEventLead(EventLead eventLead, ServiceResult result)
        {
            return result.Success;
        }

        #endregion

        #region Events

        public Event GetEventById(int eventId)
        {
            return _repository.Events.Find(x => x.EventId == eventId);
        }

        public EventDetails GetEventDetails(int eventId)
        {
            return SetEventDetail(_repository.Events.Find(x => x.EventId == eventId));
        }

        public EventOverview GetEventOverviewById(int eventId)
        {
            return SetEventOverview(_repository.Events.Find(x => x.EventId == eventId));           
        }

        public IList<EventOverview> GetUpcomingEventOverviews()
        {
            return SetEventOverviews(GetAllUpcomingEvents());
        }

        public IList<Event> GetAllEvents()
        {
            return _repository.Events.All().ToList();
        }

        public IList<Event> GetAllUpcomingEvents()
        {
            return GetUpcomingEventQuery().GroupBy(x => x.Event).Select(x => x.Key).ToList();
        }

        public IList<EventDetails> GetAllUpcomingEventDetails()
        {
            return SetEventDetails(GetAllUpcomingEvents());
        }

        public IList<Event> GetActiveUpcomingEvents()
        {
            return
                GetUpcomingEventQuery()
                    .Where(x => x.IsActive && x.Event.IsActive)
                    .GroupBy(x => x.Event)
                    .Select(x => x.Key)
                    .ToList();
        }

        public IList<EventDetails> GetActiveUpcomingEventDetails()
        {
            return SetEventDetails(GetActiveUpcomingEvents());
        }

        public IList<Event> GetEventsInDateRange(DateTime begin, DateTime end)
        {
            return GetEventDateRangeQuery(begin, end).GroupBy(x => x.Event).Select(x => x.Key).ToList();
        }

        public ServiceResult CreateEvent(Event e)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEvent(e))
                {
                    _repository.Events.Create(e);

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

        public ServiceResult CreateEventByTemplate(CreateNewEvent newEvent)
        {
            var result = new ServiceResult();

            try
            {
                EventTemplate template = GetEventTemplateById(newEvent.SelectedTemplateId);

                var e = new Event
                            {
                                GeneralLocality = newEvent.GeneralLocality,
                                RegionId = newEvent.RegionId,
                                Place = template.DefaultPlaceName,
                                IsActive = false
                            };

                int registrationTimeOffset = DirtyGirlServiceConfig.Settings.RegistrationCutoffHours * -1;
                e.RegistrationCutoff = newEvent.EventDate.AddHours(registrationTimeOffset);

                int emailPacketOffset = DirtyGirlServiceConfig.Settings.EmailPacketCutoffDays * -1;
                e.EmailCutoff = newEvent.EventDate.AddDays(emailPacketOffset);

                ServiceResult saveEventResult = CreateEvent(e);
                ServiceResult generateDateResult = GenerateEventDate(e.EventId, newEvent.EventDate, template.StartTime,
                                                                     template.EndTime, template.WaveDuration,
                                                                     template.MaxRegistrantsPerWave);
                var feeResult = new ServiceResult();

                var rfee = new EventFee
                               {
                                   EventId = e.EventId,
                                   EffectiveDate = DateTime.Now,
                                   Cost = template.DefaultRegistrationCost,
                                   EventFeeType = EventFeeType.Registration,
                                   Discountable = true,
                                   Taxable = true
                               };
                var tFee = new EventFee
                               {
                                   EventId = e.EventId,
                                   EffectiveDate = DateTime.Now,
                                   Cost = template.DefaultTransferFeeCost,
                                   EventFeeType= EventFeeType.Transfer,
                                   Discountable = false,
                                   Taxable = false
                               };
                var chFee = new EventFee
                                {
                                    EventId = e.EventId,
                                    EffectiveDate = DateTime.Now,
                                    Cost = template.DefaultChangeFeeCost,
                                    EventFeeType = EventFeeType.ChangeEvent,
                                    Discountable = false,
                                    Taxable = false
                                };
                var cfee = new EventFee
                               {
                                   EventId = e.EventId,
                                   EffectiveDate = DateTime.Now,
                                   Cost = template.DefaultCancellationFeeCost,
                                   EventFeeType = EventFeeType.Cancellation,
                                   Discountable = false,
                                   Taxable = false
                               };

                CreateEventFee(rfee);
                CreateEventFee(tFee);
                CreateEventFee(chFee);
                CreateEventFee(cfee);

                // all payscale increases should take place starting the wednesday before the event.
                var EventOffsetStart = newEvent.EventDate;
                while (EventOffsetStart.DayOfWeek != DayOfWeek.Wednesday)
                    EventOffsetStart = EventOffsetStart.AddDays(-1);

                foreach (EventTemplate_PayScale ps in template.PayScales)
                {
                    var newFee = new EventFee
                                     {
                                         EventId = e.EventId,
                                         EffectiveDate = EventOffsetStart.AddDays(0 - ps.DaysOut).Date,
                                         Cost = ps.Cost,
                                         EventFeeType = ps.EventFeeType,
                                         Taxable = ps.Taxable,
                                         Discountable = ps.Discountable                                         
                                     };

                    feeResult = CreateEventFee(newFee);

                    if (!feeResult.Success)
                        break;
                }

                if (saveEventResult.Success && generateDateResult.Success && feeResult.Success)
                {
                    _repository.SaveChanges();
                    newEvent.EventId = e.EventId;
                }
                else
                    result.AddServiceError("An Error Occured Creating this Event");
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult UpdateEvent(Event e)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEvent(e))
                {
                    _repository.Events.Update(e);
                }

                if (!_sharedRepository)
                    _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveEvent(Event e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region EventWaves

        public IList<EventLead> GetEventLeads()
        {
            return _repository.EventLeads.Filter(x => x.EventId.HasValue == false).ToList();
        }

        public IList<EventLead> GetEventLeads(int eventId, bool includeGlobalLeads)
        {
            return includeGlobalLeads
                       ? _repository.EventLeads.Filter(x => x.EventId.HasValue == false || (x.EventId.Value == eventId))
                                    .OrderBy(x => x.DisplayText)
                                    .ToList()
                       : _repository.EventLeads.Filter(x => (x.EventId.HasValue && x.EventId == eventId))
                                    .OrderBy(x => x.DisplayText)
                                    .ToList();
        }

        public IList<EventLeadType> GetEventLeadTypes()
        {
            return _repository.EventLeadTypes.All().OrderBy(x => x.TypeName).ToList();
        }

        public ServiceResult CreateEventLead(EventLead eventLead)
        {
            var result = new ServiceResult();
            try
            {
                if (ValidateEventLead(eventLead, result))
                {
                    _repository.EventLeads.Create(eventLead);

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

        public ServiceResult UpdateEventLead(EventLead eventLead)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEventLead(eventLead, result))
                {
                    EventLead updateLead = _repository.EventLeads.Find(x => x.EventLeadId == eventLead.EventLeadId);
                    updateLead.EventLeadTypeId = eventLead.EventLeadTypeId;
                    updateLead.DisplayText = eventLead.DisplayText;
                    updateLead.EventId = eventLead.EventId.HasValue ? eventLead.EventId : null;
                    updateLead.Title = eventLead.Title;

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

        public ServiceResult RemoveEventLead(int eventLeadId)
        {
            var result = new ServiceResult();

            try
            {
                EventLead leadToDelete = _repository.EventLeads.Find(x => x.EventLeadId == eventLeadId);

                if (CanRemoveEventLead(leadToDelete, result))
                {
                    _repository.EventLeads.Delete(leadToDelete);
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

        #endregion

        #region EventDates

        public IList<EventDate> GetDatesForEvent(int eventId)
        {
            return _repository.EventDates.Filter(x => x.EventId == eventId).OrderBy(x => x.DateOfEvent).ToList();
        }

        public IList<EventDateDetails> GetActiveDateDetailsByEvent(int eventId)
        {
            List<EventDate> dateList =
                _repository.EventDates.Filter(x => x.EventId == eventId).OrderBy(x => x.DateOfEvent).ToList();

            return dateList.Select(d => new EventDateDetails
                                            {
                                                EventDateId = d.EventDateId, EventId = d.EventId, DateOfEvent = d.DateOfEvent, MaxRegistrants = d.EventWaves.Sum(x => x.MaxRegistrants), RegistrationCount = d.EventWaves.Sum(x => x.Registrations.Count())
                                            }).ToList();
        }

        public List<EventDateDetails> GetAllEventDateDetails()
        {
            var eventDetailsList = _repository.Events.GetAllEventDateDetails();
            return eventDetailsList;
        }

        public IList<EventDateOverview> GetActiveEventDateOverviews(int? regionId, int? month, int? year, string sort, string direction)
        {
            var dt = DateTime.Now.Date.AddDays(-1);

            var result = new List<EventDateOverview>();
            IEnumerable<EventDateCounts> eventDates = _repository.Events.GetEventCounts(dt);

            eventDates = eventDates.Where(x => x.IsActive == true);

            if (regionId.HasValue)
                eventDates = eventDates.Where(x => x.RegionID == regionId.Value);

            if (month.HasValue)
                eventDates = eventDates.Where(x => x.DateOfEvent.Month == month);

            if (year.HasValue)
                eventDates = eventDates.Where(x => x.DateOfEvent.Year == year);

            if (string.IsNullOrEmpty(direction))
                direction = "asc";

            switch (sort.ToLower())
            {
                default:
                    eventDates = direction.ToLower() == "asc" ? eventDates.OrderBy(x => x.DateOfEvent) : eventDates.OrderByDescending(x => x.DateOfEvent);
                    break;
                case "location":
                    eventDates = direction.ToLower() == "asc" ? eventDates.OrderBy(x => x.GeneralLocality) : eventDates.OrderByDescending(x => x.GeneralLocality);
                    break;
            }

            
            foreach (EventDateCounts date in eventDates)
            {
                var overview = new EventDateOverview
                                   {
                                       EventId = date.EventId,
                                       EventDateId = date.EventDateId,
                                       GeneralLocality = date.GeneralLocality,
                                       Place = date.Place,
                                       Address = date.Address1,
                                       City = date.Locality,
                                       State = date.Name,
                                       StateCode = date.Code,
                                       Zip = date.PostalCode,
                                       DateOfEvent = date.DateOfEvent,
                                       MaxRegistrants = date.MaxRegistrants,
                                       RegistrationCount = date.RegistrationCount,
                                       PinXCoordinate = date.PinXCoordinate,
                                       PinYCoordinate = date.PinYCoordinate,
                                       CurrentCost = (date.Cost.HasValue ? date.Cost.Value : 0M),
                                       DisplayIcon = date.FeeIconID.HasValue,
                                       IconImagePath = date.ImagePath,
                                       isRegistrationCutoff = this.IsRegistrationAvailable( date.RegistrationCutoff)
                                   };
                result.Add(overview);
            }

            return result;
        }       

        public ServiceResult CreateEventDate(EventDate ed)
        {
            var result = new ServiceResult();
            try
            {
                if (ValidateEventDate(ed, result))
                {
                    _repository.EventDates.Create(ed);
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        public ServiceResult UpdateEventDate(EventDate ed)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEventDate(ed, result))
                {
                    EventDate updateDate = _repository.EventDates.Find(x => x.EventDateId == ed.EventDateId);
                    updateDate.DateOfEvent = ed.DateOfEvent;

                    foreach (EventWave w in updateDate.EventWaves)
                    {
                        w.StartTime = new DateTime(ed.DateOfEvent.Year, ed.DateOfEvent.Month, ed.DateOfEvent.Day,
                                                   w.StartTime.Hour, w.StartTime.Minute, w.StartTime.Second);
                        w.EndTime = new DateTime(ed.DateOfEvent.Year, ed.DateOfEvent.Month, ed.DateOfEvent.Day,
                                                 w.EndTime.Hour, w.EndTime.Minute, w.EndTime.Second);
                    }
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveEventDate(int eventDateId)
        {
            var result = new ServiceResult();

            try
            {
                EventDate dateToDelete = _repository.EventDates.Find(x => x.EventDateId == eventDateId);
                List<int> waveIds = dateToDelete.EventWaves.Select(x => x.EventWaveId).ToList();

                foreach (int waveId in waveIds)
                {
                    EventWave wave = dateToDelete.EventWaves.Single(x => x.EventWaveId == waveId);

                    if (CanRemoveEventWave(wave, result))
                        _repository.EventWaves.Delete(wave);
                }

                if (result.Success && CanRemoveEventDate(dateToDelete, result))
                {
                    _repository.EventDates.Delete(dateToDelete);
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult GenerateEventDate(int eventId, DateTime eventDate, DateTime startTime, DateTime endTime, int duration, int maxRegistrants)
        {
            var result = new ServiceResult();

            try
            {
                var newDate = new EventDate {EventId = eventId, DateOfEvent = eventDate, IsActive = true};

                if (ValidateEventDate(newDate, result))
                {
                    _repository.EventDates.Create(newDate);

                    DateTime newWaveStartTime = startTime;

                    while (newWaveStartTime.TimeOfDay < endTime.TimeOfDay)
                    {
                        var newWave = new EventWave
                                          {
                                              EventDateId = newDate.EventDateId,
                                              StartTime = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day,
                                                                       newWaveStartTime.Hour, newWaveStartTime.Minute,
                                                                       newWaveStartTime.Second)
                                          };
                        newWave.EndTime = newWave.StartTime.AddMinutes(duration - 1);
                        newWave.IsActive = true;
                        newWave.MaxRegistrants = maxRegistrants;
                        newWaveStartTime = newWaveStartTime.AddMinutes(duration);
                        _repository.EventWaves.Create(newWave);
                    }

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

        #endregion

        #region EventWaves

        public IList<EventWave> GetWavesForEventDate(int eventDateId)
        {
            return _repository.EventWaves.GetWavesForEventDate(eventDateId).OrderBy(x => x.StartTime).ToList();
        }

        public IList<EventWaveDetails> GetWaveDetailsForEventDate(int eventDateId)
        {
            List<EventWave> waveList =
                _repository.EventWaves.GetWavesForEventDate(eventDateId).OrderBy(x => x.StartTime).ToList();

            return waveList.Select(wave => new EventWaveDetails
                                               {
                                                   EventWaveId = wave.EventWaveId, EventDateId = wave.EventDateId, StartTime = wave.StartTime, MaxRegistrations = wave.MaxRegistrants, RegistrationCount = wave.Registrations.Count()
                                               }).ToList();
        }

        public ServiceResult CreateEventWave(EventWave ew)
        {
            var result = new ServiceResult();
            try
            {
                if (ValidateEventWave(ew, result))
                {
                    _repository.EventWaves.Create(ew);

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

        public ServiceResult UpdateEventWave(EventWave ew)
        {
            var result = new ServiceResult();
            try
            {
                if (ValidateEventWave(ew, result))
                {
                    EventWave updateWave = _repository.EventWaves.Find(x => x.EventWaveId == ew.EventWaveId);

                    updateWave.StartTime = ew.StartTime;
                    updateWave.EndTime = ew.EndTime;
                    updateWave.IsActive = ew.IsActive;
                    updateWave.MaxRegistrants = ew.MaxRegistrants;
                    _repository.EventWaves.Update(updateWave);

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

        public ServiceResult RemoveEventWave(int eventWaveId)
        {
            var result = new ServiceResult();

            try
            {
                EventWave waveToDelete = _repository.EventWaves.Find(x => x.EventWaveId == eventWaveId);

                if (CanRemoveEventWave(waveToDelete, result))
                {
                    _repository.EventWaves.Delete(waveToDelete);

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

        public EventWave GetEventWaveById(int eventWaveId)
        {
            return _repository.EventWaves.Find(x => x.EventWaveId == eventWaveId);
        }

        #endregion

        #region EventSponsors

        public IList<EventSponsor> GetSponsorsForEvent(int eventId)
        {
            return _repository.EventSponsors.Filter(x => x.EventId == eventId).ToList();
        }

        public ServiceResult CreateEventSponsor(EventSponsor sponsor)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateSponsor(sponsor, result))
                {
                    _repository.EventSponsors.Create(sponsor);
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult UpdateEventSponsor(EventSponsor sponsor)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateSponsor(sponsor, result))
                {
                    EventSponsor updateSponsor =
                        _repository.EventSponsors.Find(x => x.EventSponsorId == sponsor.EventSponsorId);

                    updateSponsor.SponsorName = sponsor.SponsorName;
                    updateSponsor.Description = sponsor.Description;

                    _repository.EventSponsors.Update(updateSponsor);
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveEventSponsor(int sponsorId)
        {
            var result = new ServiceResult();

            try
            {
                EventSponsor sponsorToDelete = _repository.EventSponsors.Find(x => x.EventSponsorId == sponsorId);

                if (CanRemoveSponsor(sponsorToDelete, result))
                {
                    _repository.EventSponsors.Delete(sponsorToDelete);
                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveEventSponsor(int eventId, string fileName)
        {
            var result = new ServiceResult();

            try
            {
                IQueryable<EventSponsor> sponsorsToDelete =
                    _repository.EventSponsors.Filter(x => x.FileName == fileName && x.EventId == eventId);

                foreach (EventSponsor sponsorToDelete in sponsorsToDelete)
                {
                    if (CanRemoveSponsor(sponsorToDelete, result))
                    {
                        _repository.EventSponsors.Delete(sponsorToDelete);
                    }
                }

                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        #endregion

        #region Regions

        public IList<Region> GetRegionsByCountry(int countryId)
        {
            return _repository.Regions.Filter(x => x.CountryId == countryId).OrderBy(x => x.Name).ToList();
        }

        #endregion

        #region Event Templates

        public EventTemplate GetEventTemplateById(int templateId)
        {
            return _repository.EventTemplates.Find(x => x.EventTemplateId == templateId);
        }

        public IList<EventTemplate> GetEventTemplates()
        {
            return _repository.EventTemplates.All().OrderBy(x => x.Name).ToList();
        }

        #endregion

        #region Event Fees

        public IList<EventFee> GetFeesForEvent(int eventId)
        {
            return
                _repository.EventFees.Filter(x => x.EventId == eventId)
                           .OrderBy(x => x.EventFeeType)
                           .ThenBy(x => x.EffectiveDate)
                           .ToList();
        }

        public EventFee GetCurrentFeeForWave(int eventWaveId, EventFeeType feeType)
        {
            if(eventWaveId <= 0)
                throw new ArgumentException("eventWaveId must be greater than 0");
            var eventId = _repository.EventWaves.Find(x => x.EventWaveId == eventWaveId).EventDate.EventId;
            return GetCurrentFeeForEvent(eventId, feeType);
        }

        public EventFee GetCurrentFeeForEvent(int eventId, EventFeeType feeType)
        {
            if (eventId <= 0)
                throw new ArgumentException("eventId must be greater than 0");
            var eventFee = 
                _repository.EventFees.Filter(
                    x => x.EventId == eventId && x.EventFeeType == feeType && x.EffectiveDate < DateTime.Now)
                           .OrderByDescending(x => x.EffectiveDate)
                           .FirstOrDefault();
            if (eventFee == null)
                throw new Exception(string.Format("Event ID: {0} has no Event Fees of type {1}", eventId, feeType));
            return eventFee;
        }

        public EventFee GetFeeForEventByRegistrationDate(int eventId, EventFeeType feeType, DateTime registrationDate)
        {
            if (eventId <= 0)
                throw new ArgumentException("eventId must be greater than 0");
            return
                _repository.EventFees.Filter(
                  x => x.EventId == eventId && x.EventFeeType == feeType && x.EffectiveDate <= registrationDate)
                           .OrderByDescending(x => x.EffectiveDate)
                           .First();
        }

        public ServiceResult CreateEventFee(EventFee fee)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEventFee(fee, result))
                {
                    _repository.EventFees.Create(fee);

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

        public ServiceResult UpdateEventFee(EventFee fee)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEventFee(fee, result))
                {
                    EventFee updateFee = _repository.EventFees.Find(x => x.PurchaseItemId == fee.PurchaseItemId);
                    updateFee.EffectiveDate = fee.EffectiveDate;
                    updateFee.Cost = fee.Cost;

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

        public ServiceResult RemoveEventFee(int purchaseItemId)
        {
            var result = new ServiceResult();

            try
            {
                EventFee feeToDelete = _repository.EventFees.Find(x => x.PurchaseItemId == purchaseItemId);

                if (CanRemoveEventFee(feeToDelete, result))
                {
                    _repository.EventFees.Delete(feeToDelete);
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

        #endregion

        #region private methods

        private List<EventDetails> SetEventDetails(IEnumerable<Event> events)
        {
            return GetEventDetailsListFromEventList(events);
        }

        public EventDetails SetEventDetail(Event e)
        {
            var overview = new EventDetails
                               {
                                   EventId = e.EventId,
                                   GeneralLocality = e.GeneralLocality,
                                   Place = e.Place,
                                   Address = e.Address1,
                                   City = e.Locality,
                                   State = e.Region.Name,
                                   StateCode = e.Region.Code,
                                   Zip = e.PostalCode,
                                   Latitude = e.Latitude,
                                   Longitude = e.Longitude,
                                   StartDate = e.EventDates.Min(x => x.DateOfEvent),
                                   EndDate = e.EventDates.Max(x => x.DateOfEvent),
                                   EventNews = e.EventDetails,
                                   RegistrationCutoff = this.IsRegistrationAvailable(e),
                                   EmailCutoff = this.IsEmailPacketOptionAvailable(e)
                               };

            var currentRegistrationFee = GetCurrentFeeForEvent(e.EventId, EventFeeType.Registration);
            overview.CurrentCost = currentRegistrationFee.Cost;
            overview.Sponsors = e.Sponsors;

            return overview;
        }

        private List<EventDetails> GetEventDetailsListFromEventList(IEnumerable<Event> events)
        {
            var eventDetails = new List<EventDetails>();

            var eventDateDetailGrouping = GetAllEventDateDetails()
                .GroupBy(ev => ev.EventId)
                .Select(ed =>
                        new
                            {
                                EventId = ed.Key,
                                MaxRegistrants = ed.Sum(x => x.MaxRegistrants),
                                RegistrationCount = ed.Sum(x => x.RegistrationCount),
                            }).ToList();

            foreach (var eventObj in events)
            {
                var eventDetailObj = SetEventDetail(eventObj);
                var totals = eventDateDetailGrouping.SingleOrDefault(x => x.EventId == eventDetailObj.EventId);
                if (totals != null)
                {
                    eventDetailObj.MaxRegistrants = totals.MaxRegistrants;
                    eventDetailObj.RegistrationCount = totals.RegistrationCount;
                    eventDetailObj.IsLive = eventObj.IsActive;
                }
                eventDetails.Add(eventDetailObj);
            }
            return eventDetails;
        }

        private List<EventOverview> SetEventOverviews(IEnumerable<Event> events)
        {
            return events.Select(SetEventOverview).ToList();
        }

        public EventOverview SetEventOverview(Event e)
        {
            var overview = new EventOverview 
                {
                    EventId = e.EventId, 
                    Location = string.Format("{0}, {1}", e.GeneralLocality, e.Region.Code),
                    Place = e.Place,
                    GeneralLocality = e.GeneralLocality
                };

            if (e.EventDates.Count > 1)
                overview.Dates = string.Format("{0} - {1}", e.EventDates.Min(x => x.DateOfEvent).ToShortDateString(),
                                               e.EventDates.Max(x => x.DateOfEvent).ToShortDateString());
            else
                overview.Dates = e.EventDates.Min(x => x.DateOfEvent).ToShortDateString();

            return overview;
        }

        private IQueryable<EventDate> GetUpcomingEventQuery()
        {
            DateTime today = DateTime.Now.Date;
            return _repository.EventDates.Filter(x => x.DateOfEvent >= today).OrderBy(x => x.DateOfEvent);
        }

        private IQueryable<EventDate> GetEventDateRangeQuery(DateTime start, DateTime end)
        {
            return
                _repository.EventDates.Filter(x => x.DateOfEvent >= start && x.DateOfEvent <= end)
                           .OrderBy(x => x.DateOfEvent);
        }

        #endregion
        public bool IsRegistrationAvailable(Event e)
        {
            return IsRegistrationAvailable(e.RegistrationCutoff);
        }

        public bool IsEmailPacketOptionAvailable(Event e)
        {
            return IsEmailPacketOptionAvailable(e.EmailCutoff);
        }

        public bool IsRegistrationAvailable(DateTime RegistrationCutoff)
        {
            return (DirtyGirl.Services.Utils.Utilities.AdjustCurrentTimeForTimezone() - RegistrationCutoff).Seconds > 0;
        }



        public bool IsEmailPacketOptionAvailable(DateTime EmailCutoff)
        {
            return (DirtyGirl.Services.Utils.Utilities.AdjustCurrentTimeForTimezone() - EmailCutoff).Seconds > 0;
        }



       
        #region Event Coupons


        #endregion
    }
}