using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DirtyGirl.Services
{
    public class DiscountService : ServiceBase, IDiscountService
    {

        #region constructor

        public DiscountService(IRepositoryGroup repository):base(repository, false){}
        public DiscountService(IRepositoryGroup repository, bool isSharedRepository):base(repository, isSharedRepository){}

        #endregion

        #region validation

        protected bool ValidateCoupon(Coupon couponToValidate, ServiceResult serviceResult)
        {
            if (couponToValidate.EndDateTime < couponToValidate.StartDateTime)
                serviceResult.AddServiceError("EndDateTime", "The effective end date must be after the effective start date.");
            
            if (couponToValidate.Value <= 0)
                serviceResult.AddServiceError("Value", "Value must be positive.");
                     
            if (couponToValidate.MaxRegistrantCount <= 0)
                serviceResult.AddServiceError("MaxRegistrantCount", "Max registrant count must be positive or empty");

            if (_repository.DiscountItems.Filter(x => x.DiscountItemId != couponToValidate.DiscountItemId && x.Code.ToUpper() == couponToValidate.Code.ToUpper()).Any())
                serviceResult.AddServiceError("Code", "The code you entered is already in use by another discount.");

            if (couponToValidate.CouponType == CouponType.Registration && !couponToValidate.EventId.HasValue)
                serviceResult.AddServiceError("EventId", "Registration coupons must be associated to an event.");
            
            return serviceResult.Success;
        }

        protected bool CanRemoveCoupon(Coupon couponToRemove, ServiceResult serviceResult)
        {

            return serviceResult.Success;
        }       


        #endregion

        #region Coupon
        
        public IList<Coupon> GetCouponsByEvent(int? eventId)
        {
            return _repository.Coupons.Filter(c => eventId.HasValue ? c.EventId == eventId : c.EventId == null).ToList();
        }

        public Coupon GetCouponById(int discountItemId)
        {
          return _repository.Coupons.Filter(c => c.DiscountItemId == discountItemId).FirstOrDefault();
        }

        
        public ServiceResult CreateCoupon(Coupon coupon)
        {
            var result = new ServiceResult();
            try
            {               

                if (ValidateCoupon(coupon, result))
                {                    
                    _repository.Coupons.Create(coupon);

                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        public ServiceResult UpdateCoupon(Coupon coupon)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                if (ValidateCoupon(coupon, result))
                {
                    Coupon updateCoupon = _repository.Coupons.Find(x => x.DiscountItemId == coupon.DiscountItemId);

                    updateCoupon.Code = coupon.Code;
                    updateCoupon.CouponType = coupon.CouponType;
                    updateCoupon.DiscountType = coupon.DiscountType;
                    updateCoupon.Description = coupon.Description;
                    updateCoupon.EndDateTime = coupon.EndDateTime;
                    updateCoupon.IsActive = coupon.IsActive;
                    updateCoupon.IsReusable = coupon.IsReusable;
                    updateCoupon.MaxRegistrantCount = coupon.MaxRegistrantCount;
                    updateCoupon.StartDateTime = coupon.StartDateTime;
                    updateCoupon.EventId = coupon.EventId;
                    updateCoupon.Value = coupon.Value;

                    if (!_sharedRepository)
                        _repository.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveCoupon(int DiscountItemId)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                Coupon couponToDelete = _repository.Coupons.Find(x => x.DiscountItemId == DiscountItemId);
                if (CanRemoveCoupon(couponToDelete, result))
                {
                    _repository.Coupons.Delete(couponToDelete);
                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;

        }
        

        #endregion

        #region Redemption Code

        public RedemptionCode GetRedemptionById(int discountItemId)
        {
          return _repository.RedemptionCodes.Filter(c => c.DiscountItemId == discountItemId).FirstOrDefault();
        }

        public ServiceResult CreateRedemptionCode(RedemptionCode redemptionCode)
        {
            var result = new ServiceResult();
            
            try
            {                
                _repository.RedemptionCodes.Create(redemptionCode);                

                if (!_sharedRepository)
                    _repository.SaveChanges();

            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult ValidateRedemptionCode(string code)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                RedemptionCode redemptionCode = _repository.RedemptionCodes.Find(x => x.Code.ToLower() == code.ToLower());

                if (redemptionCode != null)
                {
                    if (redemptionCode.ResultingRegistrationId == null)
                    {
                        if (redemptionCode.RedemptionCodeType == RedemptionCodeType.Transfer && !(redemptionCode.DateAdded >= DateTime.Now.AddDays(-DirtyGirlServiceConfig.Settings.MaxTransferHeldDays)))
                        {
                            redemptionCode.GeneratingRegistration.RegistrationStatus = RegistrationStatus.Cancelled;
                            redemptionCode.RedemptionCodeType = RedemptionCodeType.StraightValue;
                            redemptionCode.GeneratingRegistration.DateUpdated = DateTime.Now;

                            if (!_sharedRepository)
                                _repository.SaveChanges();
                        }
                    }
                    else
                        result.AddServiceError("This code has already been used.");
                }
                else
                    result.AddServiceError("Discount does not exist.");
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public string GenerateDiscountCode()
        {
            string discountCode = Path.GetRandomFileName().ToUpper().Replace(".", "");

            while (!string.IsNullOrEmpty(discountCode) && _repository.DiscountItems.Filter(r => r.Code.ToUpper() == discountCode.ToUpper()).Any())
            {
                discountCode = Path.GetRandomFileName().ToUpper().Replace(".", "");
            }

            return discountCode;
        }

        #endregion

        #region ValidateDiscount

        public ServiceResult ValidateDiscount(DiscountItem discount)
        {
            ServiceResult result = new ServiceResult();

            try
            {                             
                if (discount is RedemptionCode)
                {
                    var rCode = (RedemptionCode)discount;
                    if (rCode.ResultingRegistrationId != null)
                        result.AddServiceError("This code has already been used.");

                    return result;
                }

                if (discount is Coupon)
                {
                    var cCode = (Coupon)discount;
                    int useCount = 0;

                    switch (cCode.CouponType)
                    {
                        case CouponType.Registration:
                                useCount = _repository.CartItems.Filter(x => x.DiscountItemId == cCode.DiscountItemId).Count();
                                break;
                    }

                    if (!cCode.IsActive)
                        result.AddServiceError("This discount is not currently active");

                    if (cCode.IsReusable && cCode.MaxRegistrantCount.HasValue && useCount >= cCode.MaxRegistrantCount)
                        result.AddServiceError("This discount has exceeded the max number of allowable uses.");

                    if (!cCode.IsReusable && useCount > 0)
                        result.AddServiceError("This discount has already been used.");

                    if (DateTime.Now < cCode.StartDateTime || (cCode.EndDateTime.HasValue ? DateTime.Now > cCode.EndDateTime: true))
                        result.AddServiceError("This discount is not currently active.");

                    return result;
                }                         

                result.AddServiceError("The coupon entered does not exist.");                 

            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;

        }

        #endregion

        #region Events

        public IList<EventOverview> GetUpcomingEvents()
        {
            IEventService _eventService = new EventService(this._repository, false);
            return _eventService.GetUpcomingEventOverviews();
        }

        #endregion

    }
}