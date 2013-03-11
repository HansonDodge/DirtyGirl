using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventDetails
    {
        public int EventId { get; set; }

        public string Place { get; set; }

        public string GeneralLocality { get; set; }     

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string StateCode { get; set; }

        public string Zip { get; set; }

        public bool RegistrationCutoff { get; set; }

        public bool EmailCutoff { get; set; }

        public string Location
        {
            get { return string.Format("{0}, {1}", GeneralLocality, StateCode); }
        }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CurrentCost { get; set; }

        public int MaxRegistrants { get; set; }

        public int RegistrationCount { get; set; }

        public int SpotsLeft
        {
            get { return MaxRegistrants - RegistrationCount < 0 ? 0 : MaxRegistrants - RegistrationCount; }
        }

        public bool isFull
        {
            get { return SpotsLeft <= 0; }
        }

        [DisplayName("Is Event Live?")]
        public bool IsLive { get; set; }

        public bool DisplayIcon { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string EventNews { get; set; }

        public virtual IList<EventSponsor> Sponsors { get; set; }

    }
}
