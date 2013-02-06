using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_CreateCoupon
    {
        public Coupon coupon { get; set; }

        public int startDateYear { get; set; }
        public int startDateMonth { get; set; }
        public int startDateDay { get; set; }

        public int endDateYear { get; set; }
        public int endDateMonth { get; set; }
        public int endDateDay { get; set; }

        public bool nullEndDate { get; set; }
        public string redirectUrl { get; set; }
        public bool allEvents { get; set; }

        //Enumerations
        public IList<KeyValuePair<string,string>> ValueTypes { get; set; }
        public IList<int> Days { get; set; }
        public IList<KeyValuePair<int, string>> Months { get; set; }
        public IList<int> Years { get; set; }
        public IList<Region> RegionList { get; set; }
    }
}