using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    class TeamPostRepository : Repository<TeamPost>, ITeamPostRepository
    {
        public TeamPostRepository(DB context) : base(context) { }
    }
}
