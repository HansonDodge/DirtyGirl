using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
  public class vmAdmin_ReportPerformance
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal AvgDailyRegistrations { get; set; }
    public decimal AvgDailyRevenue { get; set; }
    public decimal AvgRegistrationsPerEvent { get; set; }
    public int TotalRegistrations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalRegistrationFees { get; set; }
    public decimal TotalCancelFees { get; set; }
    public decimal TotalTransferFees { get; set; }
    public decimal TotalChangeFees { get; set; }
    public int TotalEvents { get; set; }
  }
}