using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(DB context) : base(context) { }       
       
        public List<EventDateDetails> GetAllEventDateDetails()
        {
            var eventDateDetailsList = new List<EventDateDetails>();
            var ctx = Context;
            var regCounts = ctx.SpGetAllEventDateCounts();
            if (regCounts != null)
            {

                foreach (var regCount in regCounts)
                {
                    eventDateDetailsList.Add(new EventDateDetails
                                                 {
                                                     EventId = regCount.EventId,
                                                     EventDateId = regCount.EventDateId,
                                                     DateOfEvent = regCount.DateOfEvent,
                                                     MaxRegistrants = regCount.MaxRegistrants,
                                                     RegistrationCount = regCount.RegistrationCount
                                                 });
                }
            }

            return eventDateDetailsList;
        }
       
        public List<EventDateCounts> GetEventCounts(DateTime dt)
        {
            var ctx = Context;
            var eventDateDetailsList = new List<EventDateCounts>();
            var regCounts = ctx.SpGetEventCounts(dt);
            foreach (var regCount in regCounts)
            {
                eventDateDetailsList.Add(new EventDateCounts
                {                    
                    Address1 = regCount.Address1,
                    Code = regCount.Code,
                    Cost = regCount.Cost,
                    DateOfEvent = regCount.DateOfEvent,
                    LastDateOfEvent =  regCount.LastDateOfEvent,
                    EventId = regCount.EventId,
                    FeeIconID = regCount.FeeIconID,
                    IsActive = regCount.IsActive,
                    RegionID = regCount.RegionID,
                    GeneralLocality = regCount.GeneralLocality,
                    ImagePath = regCount.ImagePath,
                    Locality = regCount.Locality,
                    MaxRegistrants = regCount.MaxRegistrants,
                    Name = regCount.Name,
                    PinXCoordinate = regCount.PinXCoordinate,
                    PinYCoordinate = regCount.PinYCoordinate,
                    Place = regCount.Place,
                    PostalCode = regCount.PostalCode,
                    PurchaseItemID = regCount.PurchaseItemID,
                    RegistrationCount = regCount.RegistrationCount,
                    RegistrationCutoff = regCount.RegistrationCutoff,
                    EmailCutoff = regCount.EmailCutoff
                });

            }

            return eventDateDetailsList.ToList();
        }

        // TODO refractor this duplicate code
        public List<EventDateCounts> GetEventCounts(int ID)
        {
            var ctx = Context;
            var eventDateDetailsList = new List<EventDateCounts>();
            var regCounts = ctx.SpGetEventCounts(ID);
            foreach (var regCount in regCounts)
            {
                eventDateDetailsList.Add(new EventDateCounts
                {
                    Address1 = regCount.Address1,
                    Code = regCount.Code,
                    Cost = regCount.Cost,
                    DateOfEvent = regCount.DateOfEvent,
                    LastDateOfEvent = regCount.LastDateOfEvent,
                    EventId = regCount.EventId,
                    FeeIconID = regCount.FeeIconID,
                    IsActive = regCount.IsActive,
                    RegionID = regCount.RegionID,
                    GeneralLocality = regCount.GeneralLocality,
                    ImagePath = regCount.ImagePath,
                    Locality = regCount.Locality,
                    MaxRegistrants = regCount.MaxRegistrants,
                    Name = regCount.Name,
                    PinXCoordinate = regCount.PinXCoordinate,
                    PinYCoordinate = regCount.PinYCoordinate,
                    Place = regCount.Place,
                    PostalCode = regCount.PostalCode,
                    PurchaseItemID = regCount.PurchaseItemID,
                    RegistrationCount = regCount.RegistrationCount,
                    RegistrationCutoff = regCount.RegistrationCutoff,
                    EmailCutoff = regCount.EmailCutoff
                });

            }

            return eventDateDetailsList.ToList();
        }
    }
}
