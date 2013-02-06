using DirtyGirl.Models;
using System.Collections.Generic;

namespace DirtyGirl.Web.Models
{
    public class vmTeam_Details
    {
        public Team Team { get; set; }

        public Event Event { get; set; }

        public IEnumerable<EventWave> EventWave { get; set; }

        public IEnumerable<Registration> RegistrationList { get; set; }

        public IEnumerable<DisplayMessage> Messages { get; set; }

        public bool IsTeamMember { get; set; }

        public User User { get; set; }
    }
}