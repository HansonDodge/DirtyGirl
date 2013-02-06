using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventSponsorRepository: Repository<EventSponsor>, IEventSponsorRepository
    {
        public EventSponsorRepository(DB context) : base(context){ }
    }
}
