using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models
{
    public class EventBasics
    {
        public EventBasics(int _id, string _locality, string _state, DateTime _start, DateTime _end)
        {
            EventId = _id;
            GeneralLocality = _locality;
            StateCode = _state;
            StartDate = _start;
            EndDate = _end;

        }

        public int EventId { get; set; }
        public string GeneralLocality { get; set; }
        public string StateCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
