using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmCart_ThankYou
    {
        public string ConfirmationCode { get; set; }
        public CartFocusType CartFocus { get; set; }
        public string UserName { get; set; }
    }
}