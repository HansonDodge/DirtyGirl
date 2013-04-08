using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class DirtyWordRepository : Repository<DirtyWord>, IDirtyWordRepository
    {
        public DirtyWordRepository(DB context) : base(context) { }
    }
}
