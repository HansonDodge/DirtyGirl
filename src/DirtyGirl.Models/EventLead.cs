using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class EventLead
    {
        public int EventLeadId { get; set; }

        public int? EventId { get; set; }

        public int EventLeadTypeId { get; set; }

        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Display Text is Required")]
        public string DisplayText { get; set; }

        #region Navigation Properties

        public virtual EventLeadType EventLeadType { get; set; }

        #endregion
    }
}
