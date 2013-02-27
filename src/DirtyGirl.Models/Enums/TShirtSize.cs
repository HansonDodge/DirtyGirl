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
        [Display(Name = "Extra Large", Description = "Extra Large")]
        XL = 4,
        [Display(Name = "Double XL", Description = "Double XL")]
        XXL = 5,
        [Display(Name = "Triple XL", Description = "Triple XL")]
        XXXL = 6
    }
}