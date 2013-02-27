using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models.Enums
{
    public enum TShirtSize
    {
        [Display(Name = "Unknown", Description="Unknown/Not selected")]
        Unknown = 0,
        [Display(Name = "Small", Description = "Small")]
        S = 1,
        [Display(Name = "Medium", Description = "Medium")]
        M = 2,
        [Display(Name = "Large", Description = "Large")]
        L = 3,
        [Display(Name = "XL", Description = "XL")]
        XL = 4,
        [Display(Name = "XXL", Description = "XXL")]
        XXL = 5,
        [Display(Name = "XXXL", Description = "XXXL")]
        XXXL = 6
    }
}