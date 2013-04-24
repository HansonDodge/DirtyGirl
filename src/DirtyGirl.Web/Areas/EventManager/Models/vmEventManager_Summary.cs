using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.EventManager.Models
{
    public class vmEventManager_Summary
    {
        public vmEventManager_EventFilter Filter{ get; set; }
        public vmEventManager_SummaryReport Report { get; set; }
    }
}