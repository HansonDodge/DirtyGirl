using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventWaveDetails
    {
        public int EventWaveId { get; set; }
        public int EventDateId { get; set; }
        public DateTime StartTime { get; set; }
        public int MaxRegistrations { get; set; }
        public int RegistrationCount { get; set; }       
        public int SpotsLeft
        {
            get
            {
                return ((MaxRegistrations - RegistrationCount) < 0) ? 0 : MaxRegistrations - RegistrationCount; 
            }
        }
    }
}
