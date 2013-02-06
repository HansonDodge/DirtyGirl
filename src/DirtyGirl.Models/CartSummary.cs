using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class CartSummary
    {
        public List<CartSummaryLineItem> CartItems { get; set; }

        public List<string> SummaryMessages { get; set; }

        public Decimal TotalCost
        {
            get
            {
                return CartItems.Sum(x => x.ItemTotal);
            }
        }

    }
}
