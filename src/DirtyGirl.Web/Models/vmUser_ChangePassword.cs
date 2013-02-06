using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmUser_ChangePassword
    {
        public vmUser_ChangePassword()
        {
            Credentials = new vmUser_SetPassword();
        }

        public vmUser_SetPassword Credentials { get; set; }

        [Required(ErrorMessage = "You Must Enter Your Old Password")]
        public string OldPassword { get; set; }
    }
}