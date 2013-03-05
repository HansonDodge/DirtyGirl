using System;
using System.Linq;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventDateRepository : Repository<EventDate>, IEventDateRepository
    {
        public EventDateRepository(DB context) : base(context) { }

    }
}
