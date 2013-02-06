using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IRegionService
    {
        IList<Region> GetRegionsByCountryId(int countryId);
    }
}
