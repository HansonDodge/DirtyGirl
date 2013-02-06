using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class CartItem: ModelBase
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int PurchaseItemId { get; set; }
        public int? DiscountItemId { get; set; }
        public bool StandAloneItem { get; set; }

        public decimal Cost { get; set; }        
        public decimal? StateTaxPercentage { get; set; }
        public decimal? StateTaxValue { get; set; }
        public decimal? LocalTaxPercentage { get; set; }
        public decimal? LocalTaxValue { get; set; }

        public decimal? DiscountValue { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValueTotal { get; set; }

        public decimal Total { get; set; }
        
        public string Notes { get; set; }       

        #region navigation properties

        public virtual Cart Cart { get; set; }
        public virtual DiscountItem DiscountItem { get; set; }
        public virtual PurchaseItem PurchaseItem { get; set; }        

        #endregion
    }
}
