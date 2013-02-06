using System.Collections.Generic;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EventFilter
    {
        public List<SelectListItem> EventList { get; set; }
        public int? EventId { get; set; }
    }
}