using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Areas.Admin.Models
{
  public class vmAdmin_ReportCoupon
  {
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string CouponName { get; set; }
    public string CouponOrRedemption { get; set; }
    public decimal DiscountAmount { get; set; }
    public string DiscountType { get; set; }
    public DateTime RedeemedDate { get; set; }

  }
}