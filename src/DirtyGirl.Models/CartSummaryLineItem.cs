using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class CartSummaryLineItem
    {
        public PurchaseType PurchaseType { get; set; }
        public ProcessType ProcessType { get; set; }

        public Guid? SessionKey { get; set; }
        public int? EventId { get; set; }


        public bool Discountable { get; set; }
        public int? DiscountItemId { get; set; }
        public string DiscountDescription { get; set; }
        public DiscountType DiscountType { get; set; }  
        public decimal? DiscountValue { get; set; }              

        public int PurchaseItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemCost { get; set; }

        public bool Taxable { get; set; }
        public decimal? LocalTaxPercentage { get; set; }
        public decimal? StateTaxPercentage { get; set; }

        public decimal LocalTax
        {
            get
            {
                if (LocalTaxPercentage.HasValue)
                {
                    var tax = Math.Round((ItemCost - DiscountTotal) * (LocalTaxPercentage.Value / 100), 2, MidpointRounding.ToEven);
                    return tax <= 0 ? 0 : tax;
                }

                return 0;
            }
        }
        public decimal StateTax
        {
            get
            {
                if (StateTaxPercentage.HasValue)
                {
                    var tax = Math.Round((ItemCost - DiscountTotal) * (StateTaxPercentage.Value / 100), 2, MidpointRounding.ToEven);
                    return tax <= 0 ? 0 : tax;
                }

                return 0;
            }
        }

        public decimal DiscountTotal 
        {
            get
            {
                if (DiscountValue.HasValue)
                {
                    decimal discountValue = 0;

                    switch (DiscountType)
                    {
                        case DiscountType.Dollars:default:
                            discountValue = DiscountValue.Value;
                            break;
                        case DiscountType.Percentage:
                            discountValue = Math.Round(ItemCost * (DiscountValue.Value / 100), 2, MidpointRounding.ToEven);
                            break;
                    }

                    return discountValue <= 0 ? ItemCost : discountValue;
                }

                return 0;
            }            
        }

        public decimal ItemTotal 
        {
            get
            {
                var itemTotal = ItemCost - DiscountTotal + LocalTax + StateTax;
                return itemTotal <= 0 ? 0 : itemTotal;               
            }            
        }       
        
    }
}
