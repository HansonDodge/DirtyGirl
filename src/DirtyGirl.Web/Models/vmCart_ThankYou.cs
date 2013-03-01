using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirtyGirl.Models;

namespace DirtyGirl.Web.Models
{
    public class vmCart_ThankYou
    {
        public string ConfirmationCode { get; set; }
        public CartFocusType CartFocus { get; set; }
        public User CurrentUser { get; set; }
    }
}