using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_Transfer
    {
        public EventOverview EventOverview { get; set; }
        public Guid ItemId {get; set;}

        [Required(ErrorMessage="First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage="Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Email address format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Compare("Email", ErrorMessage="Emails must match.")]
        public string ConfirmationEmail { get; set; }
    }
}