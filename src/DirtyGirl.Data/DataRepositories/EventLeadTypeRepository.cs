using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventLeadTypeRepository: Repository<EventLeadType>, IEventLeadTypeRepository
    {
        public EventLeadTypeRepository(DB context) : base(context) { }
    }
}
