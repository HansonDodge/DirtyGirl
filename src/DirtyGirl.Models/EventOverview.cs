using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventOverview
    {
        public int EventId { get; set; }
        public string GeneralLocality { get; set; }
        public string Place { get; set; }
        public string Dates {get;set;}
        public string Location {get; set;}
    }
}
