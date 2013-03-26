using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_EventSelection
    {
        public int EventId { get; set; }
        public int EventDateId { get; set; }
        public int EventDateCount { get; set; }
        public int EventWaveId { get; set; }
        public EventOverview EventOverview { get; set; }
        public Guid ItemId { get; set; }
        public string EventName { get; set; }
        public bool LockEvent { get; set; }
        public CartFocusType CartFocus { get; set; }
        public int RegistrationId { get; set; }
        public DateTime EmailOptionCutoff { get; set; }
        public DateTime RegistrationCutoff { get; set; }
        public bool ReturnToRegistrationDetails { get; set; }
    }
}