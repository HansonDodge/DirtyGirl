using System.Collections.Generic;
using System.Web.Mvc;
using DirtyGirl.Models;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EventLead
    {
        public IList<EventLead> EventLead { get; set; }

        public IList<SelectListItem> EventLeadTypes { get; set; }

    }
}