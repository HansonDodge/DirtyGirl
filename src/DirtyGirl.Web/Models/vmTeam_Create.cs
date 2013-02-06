using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmTeam_Create
    {
        public int EventId { get; set; }

        public int RegistrationId { get; set; }

        public string TeamName { get; set; }
    }
}