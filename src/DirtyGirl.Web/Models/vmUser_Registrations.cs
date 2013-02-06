using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmUser_Registrations
    {
        public int RegistrationId { get; set; }
        public int RegistrantId { get; set; }
        public string Event { get; set; }
        public DateTime WaveDate { get; set; }
        public DateTime WaveTime { get; set; }
        public string RegistrationValue { get; set; }
    }
}