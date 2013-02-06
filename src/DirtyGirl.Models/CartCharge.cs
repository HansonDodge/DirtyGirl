using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class CartCharge : PurchaseItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
