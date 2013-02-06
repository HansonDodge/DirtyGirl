using System.Web.Security;
using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Models
{
    public class vmUser_EditUser
    {
        public vmUser_EditUser() { User = new User(); }
        public vmUser_EditUser(User u) { User = u; }        

        public User User { get; set; }

        public string returnUrl { get; set; }
  
        public string Password
        {
            get { return User.Password; }
            set { User.Password = value; }
        }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords Must Match")]
        public string PasswordVerification { get; set; }

        public string EmailAddress
        {
            get { return User.EmailAddress; }
            set { User.EmailAddress = value; }
        }

        [System.ComponentModel.DataAnnotations.Compare("EmailAddress", ErrorMessage = "Email Addresses must match")]
        public string EmailAddressVerification { get; set; }

        public bool ImAGirl { get; set; }

        //Enumerations
        public IList<Region> Regions { get; set; }
        public IList<int> Days { get; set; }
        public IList<SelectListItem> Months { get; set; }
        public IList<int> Years { get; set; }
        public IList<string> AllExistingRoles{ get; set; }
        public IList<string> UsersRoles { get; set; }
    }
}