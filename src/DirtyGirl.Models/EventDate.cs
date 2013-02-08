using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class EventDate : IComparable
    {
        public int EventDateId { get; set; }
        
        public int EventId { get; set; }
        
        [Required(ErrorMessage="Date is required")]        
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode=true, DataFormatString="{0:MM/dd/yyyy}")]
        public DateTime DateOfEvent { get; set; }

        public bool IsActive { get; set; }       

        #region Navigation Properites

        public virtual IList<EventWave> EventWaves { get; set; }
        public virtual Event Event { get; set; }

        #endregion


        public int CompareTo(object obj)
        {
            return this.DateOfEvent.CompareTo((obj as EventDate).DateOfEvent);
        }
    }
}
