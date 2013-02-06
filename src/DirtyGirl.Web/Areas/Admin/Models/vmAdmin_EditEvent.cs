using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EditEvent
    {
        public Event Event { get; set; }
        public IList<Region> RegionList { get; set; }
        public IList<SelectListItem> FeeTypes { get; set; }
        public IList<SelectListItem> EventLeadTypes { get; set; }
        
        public IList<SelectListItem> CouponTypeList { get; set; }
        public IList<SelectListItem> DiscountTypeList { get; set; }

        public IList<SelectListItem> IsGlobal
        {
            get
            {
                var list = new List<SelectListItem>
                               {
                                   new SelectListItem {Text = "Yes", Value = "1"},
                                   new SelectListItem {Text = "No", Value = "0"}
                               };
                return list;
            }
        }

    }
}