using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemId { get; set; }
        public decimal Cost { get; set; }
        public bool Discountable { get; set; }
        public bool Taxable { get; set; }

        private DateTime dateAdded = default(DateTime);
        public DateTime DateAdded
        {
            get { return (this.dateAdded == default(DateTime)) ? DateTime.Now : this.dateAdded; }
            set { dateAdded = value; }
        }

        #region navigation properties

        public virtual IList<CartItem> CartItems { get; set; }        

        #endregion
    }
}
