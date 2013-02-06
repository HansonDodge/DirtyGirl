using DirtyGirl.Models;
using System;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_CreateTeam
    {
        public EventOverview EventOverview { get; set; }

        public int EventId { get; set; }

        public Guid ItemId { get; set; }

        public string RegistrationType { get; set; }

        public string TeamName { get; set; }

        public string TeamCode { get; set; }

        public string TeamType { get; set; }

    }

}