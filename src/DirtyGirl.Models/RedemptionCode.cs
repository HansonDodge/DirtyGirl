using DirtyGirl.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DirtyGirl.Models
{
    public class RedemptionCode : DiscountItem 
    {
        public RedemptionCodeType RedemptionCodeType { get; set; }

        [ForeignKey("GeneratingRegistration")]
        public int GeneratingRegistrationId { get; set; }

        [ForeignKey("ResultingRegistration")]
        public int? ResultingRegistrationId { get; set; }

        #region Navigation Properties   
        
        public virtual Registration GeneratingRegistration { get; set; }

        public virtual Registration ResultingRegistration { get; set; }

        #endregion
    }
}
