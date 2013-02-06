using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventTemplateRepository:Repository<EventTemplate>, IEventTemplateRepository
    {
        public EventTemplateRepository(DB context) : base(context) { }
    }
}
