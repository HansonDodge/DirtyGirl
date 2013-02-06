using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventDateDetails
    {
        public int EventDateId { get; set; }

        public int EventId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfEvent { get; set; }

        public string DisplayDateOfEvent
        {
            get
            {
                return DateOfEvent.ToString("dddd MM/dd/yyyy");
            }
        }

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
    }
}
