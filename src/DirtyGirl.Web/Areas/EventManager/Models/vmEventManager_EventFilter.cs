using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.EventManager.Models
{
    public class vmEventManager_EventFilter
    {          
        public List<SelectListItem> EventList { get; set; }
        public int? EventId { get; set; }    
    }
}