using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EventPerformance
    {
        public vmAdmin_EventFilter Filter { get; set; }
        public vmAdmin_PerformanceReport Report { get; set; }
    }
}