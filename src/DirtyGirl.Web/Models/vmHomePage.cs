using DirtyGirl.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Globalization;

namespace DirtyGirl.Web.Models
{
    public class vmHomePage
    {
        public IList<SelectListItem> RegionList { get; set; }

        public IList<SelectListItem> MonthList { get; set; }

        public IList<SelectListItem> YearList { get; set; }

        public IList<EventDateOverview> EventDateDetails { get; set; }

    }
}