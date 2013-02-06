using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventFeeRepository: Repository<EventFee>, IEventFeeRepository
    {
        public EventFeeRepository(DB context) : base(context){ }
    }
}
