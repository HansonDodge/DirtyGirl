using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_WaveItem
    {
        public int EventWaveId { get; set; }
        public int WaveNumber { get; set; }
        public string WaveNotification { get; set; }
        public string cssClassName { get; set; }
        public DateTime StartTime { get; set; }
        public bool isFull { get; set; }
    }
}