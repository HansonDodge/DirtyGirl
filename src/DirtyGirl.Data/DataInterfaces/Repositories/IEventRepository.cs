using System.Collections.Generic;
using System.Data.Entity;
using DirtyGirl.Models;
using System;

namespace DirtyGirl.Data.DataInterfaces.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        List<EventDateDetails> GetAllEventDateDetails();
        List<EventDateCounts> GetEventCounts(int ID);
        List<EventDateCounts> GetEventCounts(DateTime dt);
    }
}
