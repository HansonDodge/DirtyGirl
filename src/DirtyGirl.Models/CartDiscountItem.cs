using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models
{
    public class CartDiscountItem
    {
        public int CartDiscountItemId { get; set; }
        public int CartId { get; set; }
        public int DiscountItemId { get; set; }
        public decimal value { get; set; }

        public virtual DiscountItem DiscountItem { get; set; }
    }
}
