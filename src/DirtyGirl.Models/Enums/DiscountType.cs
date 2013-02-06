using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models.Enums
{
    public enum DiscountType
    {
        [Display(Name="Dollars", Description="Actual value of a coupon in Dollars")]
        Dollars = 1,

        [Display(Name="Percentage", Description="Percentage of value to be applied by the coupon")]
        Percentage = 2

    }
}
