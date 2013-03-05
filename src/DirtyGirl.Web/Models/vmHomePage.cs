using DirtyGirl.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Globalization;
using DirtyGirl.Web.Utils;

namespace DirtyGirl.Web.Models
{
    public class vmHomePage
    {
        public IList<SelectListItem> RegionList { get; set; }

        public IList<SelectListItem> MonthList { get; set; }

        public IList<SelectListItem> YearList { 
            get {
                List<EventDateOverview> l = (List<EventDateOverview>)EventDateDetails;
                return Utilities.CreateSelectList(l.GroupBy(x => x.DateOfEvent.Year).Select(x => x.Key).ToList(), x => x, x => x);         
            } 
        }

        public IList<EventDateOverview> EventDateDetails { get; set; }

    }
}