using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventTemplate_PayScale
    {
        public int EventTemplate_PayScaleId { get; set; }
        public int EventTemplateId { get; set; }
        public EventFeeType EventFeeType { get; set; }
        public int DaysOut { get; set; }
        public decimal Cost { get; set; }
        public bool Taxable { get; set; }
        public bool Discountable { get; set; }

        #region navigation properties

        public virtual EventTemplate EventTemplate { get; set; }

        #endregion
    }
}
