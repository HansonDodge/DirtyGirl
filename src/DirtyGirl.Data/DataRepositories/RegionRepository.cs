using System.Collections.Generic;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;
using System.Linq;

namespace DirtyGirl.Data.DataRepositories
{
    public class RegionRepository : Repository<Region>, IRegionRepository
    {
        public RegionRepository(DB context) : base(context){ }

    }
    
}
