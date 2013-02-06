using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventFee: PurchaseItem
    {
        public int EventId { get; set; }

        public EventFeeType EventFeeType { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EffectiveDate { get; set; }

        public int? FeeIconId { get; set; }

        #region Navigation Properties

        public virtual Event Event { get; set; }
        public virtual FeeIcon FeeIcon { get; set; }

        #endregion

    }
}
