using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Coupon : DiscountItem 
    {
        [Required(ErrorMessage="Please Select a Coupon Type")]
        public CouponType CouponType { get; set; }      

        public int? MaxRegistrantCount { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date Effective is required.")]
        public DateTime StartDateTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDateTime { get; set; }

        public bool IsReusable { get; set; }

        public string Description { get; set; }        

        public int? EventId { get; set; }        

        #region Naviation Properties

        public virtual Event Event { get; set; }

        #endregion

    }
}
