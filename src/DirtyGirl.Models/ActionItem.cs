using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class ActionItem
    {
        public CartActionType ActionType { get; set; }
        public object ActionObject { get; set; }
        public bool ItemReadyForCheckout { get; set; }
    }
}
