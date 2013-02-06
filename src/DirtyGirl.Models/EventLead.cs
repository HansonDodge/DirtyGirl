namespace DirtyGirl.Models
{
    public class EventLead
    {
        public int EventLeadId { get; set; }

        public int? EventId { get; set; }

        public int EventLeadTypeId { get; set; }

        public string Title { get; set; }

        public string DisplayText { get; set; }

        #region Navigation Properties

        public virtual EventLeadType EventLeadType { get; set; }

        #endregion
    }
}
