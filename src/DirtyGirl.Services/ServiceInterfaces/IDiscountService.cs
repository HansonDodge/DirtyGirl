using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IDiscountService
    {

        #region Coupons

        IList<Coupon> GetCouponsByEvent(int? eventId);

        Coupon GetCouponById(int discountItemId);

        ServiceResult CreateCoupon(Coupon coupon);

        ServiceResult UpdateCoupon(Coupon coupon);

        ServiceResult RemoveCoupon(int DiscountItemId);

        #endregion

        #region Redemption Code

        RedemptionCode GetRedemptionById(int discountItemId);

        ServiceResult CreateRedemptionCode(RedemptionCode redemptionCode);

        ServiceResult ValidateRedemptionCode(string code);

        string GenerateDiscountCode();

        #endregion        

        #region ValidateDiscount

        ServiceResult ValidateDiscount(DiscountItem discount);

        #endregion

        #region Event

        IList<EventOverview> GetUpcomingEvents();

        #endregion
    }
}
