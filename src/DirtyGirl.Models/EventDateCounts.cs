using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models
{
    public class EventDateCounts
    {
        public int EventId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfEvent { get; set; }
        public DateTime LastDateOfEvent { get; set; }

        public string DisplayDateOfEvent
        {
            get
            {
                return DateOfEvent.ToString("dddd MM/dd/yyyy");
            }
        }
        public bool IsActive { get; set; }
        public string GeneralLocality { get; set; }
        public string Place { get; set; }
        public string Address1 { get; set; }
        public string Locality { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PostalCode { get; set; }
        public int RegionID { get; set; }
        public DateTime RegistrationCutoff { get; set; }
        public DateTime EmailCutoff {get;set;}
        public int MaxRegistrants { get; set; }
        public int RegistrationCount { get; set; }

        public int SpotsLeft
        {
            get
            {
                return (MaxRegistrants - RegistrationCount) <= 0 ? 0 : MaxRegistrants - RegistrationCount;
            }
        }

        public bool IsFull { get { return SpotsLeft <= 0; } }
        public int PurchaseItemID { get; set; }
        public string PinXCoordinate { get; set; }
        public string PinYCoordinate { get; set; }
        public decimal? Cost { get; set; }
        public int? FeeIconID { get; set; }
        public string ImagePath { get; set; }
    }
}
