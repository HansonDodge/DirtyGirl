using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    class TeamRepository:Repository<Team>, ITeamRepository
    {
        public TeamRepository(DB context) : base(context) { }
    }
}
