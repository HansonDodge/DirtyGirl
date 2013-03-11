using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models.Enums
{
    public enum RegistrationMaterialsDeliveryOption
    {
        [Display(Name = "On site pickup on day of event", Description = "On site pickup on day of event")]
        OnSitePickup = 0,
        [Display(Name = "Mail my packet to the address above", Description = "Mail my packet to the address above")]
        Mail = 1
    }
}
