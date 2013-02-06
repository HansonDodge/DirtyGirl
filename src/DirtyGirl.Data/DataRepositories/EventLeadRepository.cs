using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    class EventLeadRepository: Repository<EventLead>, IEventLeadRepository
    {
        public EventLeadRepository(DB context) : base(context) { }
    }
}
