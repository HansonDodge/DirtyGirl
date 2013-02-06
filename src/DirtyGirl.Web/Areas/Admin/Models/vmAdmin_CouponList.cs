using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_CouponList
    {
        public int? SelectedEventId { get; set; }

        public IList<EventOverview> EventList { get; set; }

        public IList<SelectListItem> CouponTypeList { get; set; }

        public IList<SelectListItem> DiscountTypeList { get; set; }

    }
}