using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Web.Areas.EventManager.Models
{
    public class vmEventManager_DetailedReport
    {
        public int EventID { get; set; }
        public string EventName { get; set; }

        public List<DetailedReportWaveData> WaveData { get; set; }

        public vmEventManager_DetailedReport()
        {
            EventID = 0;
            EventName = string.Empty;
            WaveData = new List<DetailedReportWaveData>();
        }
    }

    public class DetailedReportWaveData
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string MedicalInformation { get; set; }
        public string SpecialNeeds { get; set; }
        public string ShirtSize { get; set; }
        public string AgreedLegal { get; set; }
        public string AgreedTrademark { get; set; }
        public string ConfirmationCode { get; set; }        
        public DateTime StartTime { get; set; }                
    }
}
