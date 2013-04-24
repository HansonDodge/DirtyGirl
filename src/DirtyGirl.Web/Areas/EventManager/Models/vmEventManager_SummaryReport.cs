using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.EventManager.Models
{
    public class vmEventManager_SummaryReport
    {
        public int EventID { get; set; }
        public string EventName { get; set; }

        public List<SummaryReportWaveData> WaveData { get; set; }

        public vmEventManager_SummaryReport()
        {
            EventID = 0;
            EventName = string.Empty;
            WaveData = new List<SummaryReportWaveData>();
        }
    }

    public class SummaryReportWaveData
    {
        public int EventWaveID { get; set; }
        public int WaveNumber { get; set; }
        public DateTime StartTime { get; set; }
        public int NumParticipants { get; set; }
        public string  Active { get; set; }
    }
}