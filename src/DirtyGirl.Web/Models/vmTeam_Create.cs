using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirtyGirl.Models;

namespace DirtyGirl.Web.Models
{
    public class vmTeam_Create
    {
        public Team CurrentTeam { get; set; }
        public int EventId { get; set; }
        public int RegistrationId { get; set; }
        public string TeamName { get; set; }
        public string ReturnUrl { get; set; }
        
    }
}