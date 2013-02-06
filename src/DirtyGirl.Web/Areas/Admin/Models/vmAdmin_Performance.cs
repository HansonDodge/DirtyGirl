using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_Performance
    {
        public vmAdmin_PerformanceFilter Filter { get; set; }
        public vmAdmin_PerformanceReport Report { get; set; }
    }
}