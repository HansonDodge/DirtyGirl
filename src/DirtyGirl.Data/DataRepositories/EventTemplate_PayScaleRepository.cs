using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventTemplate_PayScaleRepository:Repository<EventTemplate_PayScale>, IEventTemplate_PayScaleRepository
    {
        public EventTemplate_PayScaleRepository(DB context) : base(context) { }
    }
}
