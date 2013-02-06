using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models.Enums
{
    public enum RegistrationType
    {
        [Display(Name="Standard Registration", Description="Standard Registration")]
        StandardRegistration = 1,
        [Display(Name="Cancer Survivor", Description="Cancer Survivor")]
        CancerRegistration = 2
    }
}
