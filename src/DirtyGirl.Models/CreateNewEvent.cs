using System;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class CreateNewEvent
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Enter the general locality the event will take place")]
        public string GeneralLocality { get; set; }

        [Required(ErrorMessage = "Select a state")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Select an event template")]
        public int SelectedTemplateId { get; set; }

        public CreateNewEvent()
        {
            EventDate = DateTime.Now;
        }

    }
}
