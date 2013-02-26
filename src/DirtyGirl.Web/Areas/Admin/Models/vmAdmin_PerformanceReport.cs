using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_PerformanceReport
    {
        public List<FeeReport> FeeReport { get; set; }
        public List<ChargeReport> ChargeReport { get; set; }

        public int DayCount { get; set; }
        
        public int TotalSpots { get; set; }        
        public int SpotsTaken { get; set; }
        public int SpotsLeft { get { return TotalSpots - SpotsTaken < 0 ? 0 : TotalSpots - SpotsTaken; } }
        public decimal RegPerDay { get { return Math.Round(((decimal)SpotsTaken / DayCount), 2); } }

        public List<Dictionary<String, int>> TShirtSizes { get; set; }

        public int EventCount { get; set; }        
        public int RedemptionRegCount { get; set; }  


                
        public decimal FeeValue 
        {
            get { return FeeReport.Sum(x => x.CostTotal); }          
        }        

        public decimal DiscountValue
        {
            get { return FeeReport.Sum(x => x.DiscountTotal); }
        }

        public decimal LocalTaxValue
        {
            get { return FeeReport.Sum(x => x.LocalTaxTotal); }
        }

        public decimal StateTaxValue
        {
            get { return FeeReport.Sum(x => x.StateTaxTotal); }
        }

        public decimal FeeActualRevenue
        {
            get { return FeeReport.Sum(x => x.ActualTotal); }
        }

        public decimal ChargeValue
        {
            get { return ChargeReport.Sum(x => x.CostTotal); }
        }

        public decimal ChargeLocalTaxValue
        {
            get { return ChargeReport.Sum(x => x.LocalTaxTotal); }
        }

        public decimal ChargeStateTaxValue
        {
            get { return ChargeReport.Sum(x => x.StateTaxTotal); }
        }

        public decimal ChargeActualRevenue
        {
            get { return ChargeReport.Sum(x => x.ActualTotal); }
        }

        public decimal TotalRevenue
        {
            get { return FeeActualRevenue + ChargeActualRevenue; }
        }

        public decimal RevenuePerDay { get { return TotalRevenue / DayCount; } }

        public decimal RevenuePerEvent { get { return TotalRevenue / EventCount; } }
        
        public vmAdmin_PerformanceReport()
        {
            FeeReport = new List<FeeReport>();
            ChargeReport = new List<ChargeReport>();
        }
    }
}