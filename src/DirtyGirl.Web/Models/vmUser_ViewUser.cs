using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmUser_ViewUser
    {
        public User User { get; set; }
        public IList<Registration> Registrations { get; set; }
        public Dictionary<int, decimal> RegistrationValues { get; set; }
        public IList<RedemptionCode> OpenCodes { get; set; }
    }
}