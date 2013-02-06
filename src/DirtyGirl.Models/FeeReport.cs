using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models
{
    public class FeeReport
    {        
        public EventFeeType FeeType { get; set; }
        public decimal Cost { get; set; }       
        public int UseCount { get; set; }
        public decimal StateTaxTotal { get; set; }
        public decimal LocalTaxTotal { get; set; }
        public decimal CostTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal ActualTotal { get; set; }
    }
}
