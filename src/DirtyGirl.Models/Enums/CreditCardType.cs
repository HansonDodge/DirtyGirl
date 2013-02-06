using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models.Enums
{
    public enum CreditCardType
    {
        [Display(Name="MasterCard")]
        MasterCard = 1,
        [Display(Name = "Visa")]
        Visa = 2,
        [Display(Name="American Express")]
        AmericanExpress = 3,
        [Display(Name = "Discover")]
        Discover = 4
    }
}
