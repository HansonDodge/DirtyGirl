using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models.Enums
{
    public enum TShirtSize
    {
        [Display(Name = "Small", Description = "Small")]
        Small = 1,
        [Display(Name = "Medium", Description = "Medium")]
        Medium = 2,
        [Display(Name = "Large", Description = "Large")]
        Large = 3,
        [Display(Name = "XLarge", Description = "XLarge")]
        XLarge = 4,
        [Display(Name = "XXLarge", Description = "XXLarge")]
        XXLarge = 5
    }
}