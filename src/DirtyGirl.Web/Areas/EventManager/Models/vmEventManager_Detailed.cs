using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.EventManager.Models
{
    public class vmEventManager_Detailed
    {
        public vmEventManager_EventFilter Filter{ get; set; }
        public vmEventManager_DetailedReport Report { get; set; }
    }
}