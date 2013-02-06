using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_WaveSelection
    {
        public EventOverview EventOverview { get; set; } 
      
        public IList<EventDateDetails> EventDates { get; set; }        

        public int EventId { get; set; }
        public int? EventWaveId { get; set; }
        public int? EventDateId {get; set;}

        public Guid TempRegId { get; set; }

    }
}