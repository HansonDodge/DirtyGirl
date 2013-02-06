using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
  public class vmAdmin_ReportTransfer
  {
    public int EventId { get; set; }
    public string EventName { get; set; }
    public int NumberOfChanges { get; set; }
    public decimal ChangeFeeTotal { get; set; }
    public decimal RegistrationFeeDifference { get; set; }
    public decimal TotalFees { get; set; }
    public string CouponCode { get; set; }
    public string ValueType { get; set; }
  }
}