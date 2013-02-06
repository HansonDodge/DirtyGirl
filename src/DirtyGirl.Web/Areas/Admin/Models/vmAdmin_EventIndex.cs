using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EventIndex
    {
        public IList<EventDetails> EventList { get; set; }
        public IList<Region> RegionList { get; set; }
        public IList<EventTemplate> TemplateList { get; set; }
        public CreateNewEvent NewEvent { get; set; } 
    }
}