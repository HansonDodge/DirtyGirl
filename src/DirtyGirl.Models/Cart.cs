using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Cart: ModelBase
    {
        public int CartId { get; set; }

        public int UserId {get; set;}

        public CartType CartType {get;set;}

        public string BillingAddress1 { get; set; }

        public string BillingAddress2 { get; set; }

        public string BillingLocality { get; set; }

        public int BillingRegionId { get; set; }

        public string BillingPostalCode { get; set; }

        public decimal TotalCost { get; set; }

        public string TransactionId { get; set; }

        public string InvoiceNumber { get; set; }
        
        public DateTime TransactionDate { get; set; }

        #region navigation properties

        public virtual IList<CartItem> CartItems { get; set; }
        public virtual IList<CartDiscountItem> CartDiscounts { get; set; }
        public virtual User User { get; set; }

        #endregion 

    }
}
