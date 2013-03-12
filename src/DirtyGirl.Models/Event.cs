using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class Event
    {
        public int EventId { get; set; }
      
        public string Place { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }
        
        public string Locality { get; set; }

        public int RegionId { get; set; }
        
        public string PostalCode {get; set;}

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "You must enter a general locality.")]
        public string GeneralLocality { get; set; }

        public string EventDetails { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string PinXCoordinate { get; set; }

        public string PinYCoordinate { get; set; }

        public decimal StateTax { get; set; }

        public decimal LocalTax { get; set; }

        private DateTime _dateAdded = default(DateTime);
        public DateTime DateAdded 
        { 
            get { return (_dateAdded == default(DateTime)) ? DateTime.Now : _dateAdded;}
            set { _dateAdded = value; } 
        }

        [Required(ErrorMessage = "You must enter the registration cutoff date/time")]
        public DateTime RegistrationCutoff { get; set; }

        [Required(ErrorMessage = "You must enter the email packet cutoff date")]
        public DateTime EmailCutoff { get; set; }

        #region Navigation Properties

        public virtual Region Region { get; set; }

        public virtual IList<EventLead> EventLead { get; set; }

        public virtual IList<EventDate> EventDates { get; set; }

        public virtual IList<EventFee> EventFees { get; set; }

        public virtual IList<EventSponsor> Sponsors { get; set; }

        public virtual IList<Coupon> Coupons { get; set; }

        #endregion
    }
}
