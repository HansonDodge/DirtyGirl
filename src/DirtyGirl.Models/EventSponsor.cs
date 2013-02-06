
using System.ComponentModel.DataAnnotations;
namespace DirtyGirl.Models
{
    public class EventSponsor
    {
        public int EventSponsorId { get; set; }

        public int EventId { get; set; }

        public string SponsorName { get; set; }
        
        [Editable(false)]
        public string FileName { get; set; }

        public string Description { get; set; }

        [Editable(false)]
        public string Url { get; set; }

        [Editable(false)]
        public string thumbnailUrl { get; set; }

        #region Navigation Properties

        public virtual Event Event { get; set; }

        #endregion

    }
}
