using DirtyGirl.Models;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_EventLeadItem
    {
        public int EventLeadId { get; set; }

        public int? EventId { get; set; }

        public int EventLeadTypeId { get; set; }

        public string Title { get; set; }

        public string DisplayText { get; set; }

        // Int because of the Kendo controls
        public int IsGlobal { get; set; }

        public EventLeadType EventLeadType { get; set; }
    }
}