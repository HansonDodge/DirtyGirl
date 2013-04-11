using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class SessionCart
    {

        public Dictionary<Guid, ActionItem> ActionItems { get; set; }

        public CartFocusType CheckOutFocus
        {
            get;
            set;
        }

        public string ResultingConfirmationCode { get; set; }
                
        public string DiscountCode { get; set; }

        public string EventCity { get; set; }
    }
}
