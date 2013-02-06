using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace DirtyGirl.Services
{
    public class RegionService : IRegionService
    {

        #region private members

        IRepositoryGroup _repository;       

        #endregion

        #region constructor

        public RegionService(IRepositoryGroup repository)
        {
            this._repository = repository;
        }

        #endregion        

        public IList<Region> GetRegionsByCountryId(int countryId)
        {
            return _repository.Regions.Filter(x => x.CountryId == countryId).OrderBy(x => x.Name).ToList();
        }
    }
}
