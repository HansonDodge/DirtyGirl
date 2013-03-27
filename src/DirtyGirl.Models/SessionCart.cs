using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class SessionCart
    {
        private CartFocusType[] _foci;

        public Dictionary<Guid, ActionItem> ActionItems { get; set; }

        public CartFocusType[] CheckOutFocus
        {
            get {
                if (_foci == null) {
                    _foci = new CartFocusType[]{};
                }
                return _foci;
            }
            set;
        }

        public string ResultingConfirmationCode { get; set; }
                
        public string DiscountCode { get; set; }
    }
}
