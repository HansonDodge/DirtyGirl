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


        public EventDateDetails GetEventDateDetails(int ID)
        {
            EventDateDetails eventDateDetail = null;
            var ctx = Context;
            var regCounts = ctx.SpGetEventDateCounts(ID);
            if (regCounts != null)
            {
                var regCount = regCounts.First();
                eventDateDetail = new EventDateDetails() ;
                eventDateDetail.EventId = regCount.EventId;
                eventDateDetail.EventDateId = regCount.EventDateId;
                eventDateDetail.DateOfEvent = regCount.DateOfEvent;
                eventDateDetail.MaxRegistrants = regCount.MaxRegistrants;
                eventDateDetail.RegistrationCount = regCount.RegistrationCount;                 
            }

            return eventDateDetail;
        }
    }
}
