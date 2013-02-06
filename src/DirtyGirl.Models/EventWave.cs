using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class EventWave
    {
        public int EventWaveId { get; set; }
        public int EventDateId { get; set; }
        
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }        

        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        public bool IsActive { get; set; }

        public int MaxRegistrants { get; set; }

        #region Navigation Properties

        public virtual EventDate EventDate { get; set; }

        public virtual IList<Registration> Registrations { get; set; }

        #endregion
    }
}
