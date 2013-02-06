using System;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_CreateEventDate
    {
        public int EventId { get; set; }

        [Required(ErrorMessage="EventDate is Required")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage="Start time is a required to generate waves")]
        public DateTime WaveStartTime { get; set; }
        
        [Required(ErrorMessage="End time is required to generate waves")]
        public DateTime WaveEndTime { get; set; }
        
        public int Duration { get; set; }
        
        public int MaxRegistrants { get; set; }

        public vmAdmin_CreateEventDate() { }

        public vmAdmin_CreateEventDate(int eventId)
        {
            this.EventId = eventId;
            this.EventDate = DateTime.Now.Date;
        }
    }
}