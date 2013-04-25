using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_PerformanceFilter
    {
        public bool fullEvent { get; set; }
        public List<SelectListItem> EventList { get; set; }

        public int? EventId { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

    }
}