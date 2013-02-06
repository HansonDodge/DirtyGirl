using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmUser_SetPassword
    {

        public int UserId { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "A New Password is Required")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords must match")]
        public string NewPasswordVerification { get; set; }


    }
}