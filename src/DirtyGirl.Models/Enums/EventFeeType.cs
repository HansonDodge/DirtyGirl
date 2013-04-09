using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models.Enums
{
    public enum EventFeeType
    {
        [Display(Name="Registration", Description="The fee that applies to an event's regisistration")]
        Registration = 1,
        [Display(Name="Change Event", Description="The fee that applies to changing your event")]
        ChangeEvent = 2,
        [Display(Name="Event Transfer", Description="The Fee that applies to transfering your event")]
        Transfer = 3,
        [Display(Name="Cancel Event", Description="The Fee applied to cancelling your event")]
        Cancellation = 4,
        [Display(Name = "Shipping & Handling", Description = "The Fee applied for mailing your registration materials")]
        Shipping = 5,
        [Display(Name = "Processing Fee", Description = "The processing fee that applies to an event's regisistration.")]
        ProcessingFee = 6
    }
}
