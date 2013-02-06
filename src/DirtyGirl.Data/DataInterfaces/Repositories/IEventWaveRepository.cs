using DirtyGirl.Models;
using System.Linq;

namespace DirtyGirl.Data.DataInterfaces.Repositories
{
    public interface IEventWaveRepository: IRepository<EventWave>
    {
        IQueryable<EventWave> GetWavesForEventDate(int eventDateId);
    }
}
