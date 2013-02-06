using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{
    public class DiscountItem
    {
        public int DiscountItemId { get; set; }               
        
        public string Code { get; set; }

        [Required(ErrorMessage = "Value type is required.")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Coupon must have a value.")]
        public decimal Value { get; set; }

        public bool IsActive { get; set; }

        private DateTime dateAdded = default(DateTime);
        public DateTime DateAdded
        {
            get { return (this.dateAdded == default(DateTime)) ? DateTime.Now : this.dateAdded; }
            set { dateAdded = value; }
        }

        #region navigation properties

        public virtual IList<CartDiscountItem> CartDiscountItem { get; set; }

        public virtual IList<CartItem> CartItem { get; set; }

        #endregion

    }
}
