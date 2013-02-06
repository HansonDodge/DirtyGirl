using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;
using System.Linq;

namespace DirtyGirl.Data.DataRepositories
{
    public class EventWaveRepository: Repository<EventWave>, IEventWaveRepository
    {
        public EventWaveRepository(DB context) : base(context){ }

        public IQueryable<EventWave> GetWavesForEventDate(int eventDateId)
        {
            return this.All().Where(x => x.EventDateId == eventDateId);
        }
    }
}
