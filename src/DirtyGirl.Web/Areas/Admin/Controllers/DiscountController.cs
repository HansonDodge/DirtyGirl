using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Utils;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;


namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class DiscountController : BaseController
    {

        #region private members

        private readonly IDiscountService _service;

        #endregion

        #region Constructor

        public DiscountController(IDiscountService service)
        {
            this._service = service;
        }

        #endregion

        #region Index List

        public ActionResult Index(int? eventId)
        {
            vmAdmin_CouponList vm = new vmAdmin_CouponList
                {
                    SelectedEventId = eventId,
                    EventList = _service.GetUpcomingEvents(),
                    CouponTypeList = DirtyGirlExtensions.ConvertToSelectList<CouponType>(),
                    DiscountTypeList = DirtyGirlExtensions.ConvertToSelectList<DiscountType>()
                };
            
            return View(vm);
        }

        #endregion

        #region Coupons

        [HttpPost]
        public ActionResult Ajax_GetCoupons([DataSourceRequest] DataSourceRequest request, int? masterEventId)
        {

            var couponList = _service.GetCouponsByEvent(masterEventId);
            
            List<SerializeableCoupon> resultList = new List<SerializeableCoupon>(); 
            foreach (Coupon c in couponList)
            {
                SerializeableCoupon nc = new SerializeableCoupon();

                nc.CartDiscountItem = c.CartDiscountItem;
                nc.CartItem = c.CartItem;
                nc.Code = c.Code;
                nc.CouponType = c.CouponType;
                nc.DateAdded = c.DateAdded;
                nc.Description = c.Description;
                nc.DiscountItemId = c.DiscountItemId;
                nc.DiscountType = c.DiscountType;
                nc.EndDateTime = c.EndDateTime;
                //nc.Event = c.Event;
                nc.EventId = c.EventId;
                nc.IsActive = c.IsActive;
                nc.IsReusable = c.IsReusable;
                nc.MaxRegistrantCount = c.MaxRegistrantCount;
                nc.StartDateTime = c.StartDateTime;
                nc.Value = c.Value;

                var tzi = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                
                DateTime startTime = c.StartDateTime;
                DateTime endTime = c.EndDateTime.Value;

                // Get the offset from UTC for the time zone and date in question.
                //var offset = tzi.GetUtcOffset(startTime);
                TimeSpan offset = new TimeSpan(0); 
                
                // Get a DateTimeOffset for the date, and adjust it to the offset found above.
                var startOffsetTime = new DateTimeOffset(startTime).ToOffset(offset);
                var endOffsetTime = new DateTimeOffset(endTime).ToOffset(offset);

                nc.EndDateTimeOff = endOffsetTime;
                nc.StartDateTimeOff = startOffsetTime;

                resultList.Add(nc); 
            }

            return Json(resultList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet); 

            //return new ContentResult() { Content = JsonConvert.SerializeObject(resultList.ToDataSourceResult(request), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }), ContentType = "application/json" };
        }

        [HttpPost]
        public ActionResult Ajax_CreateCoupon([DataSourceRequest] DataSourceRequest request, Coupon coupon, int couponType, int? masterEventId)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(coupon.Code))
                    coupon.Code = _service.GenerateDiscountCode();

                coupon.EventId = masterEventId;                             

                ServiceResult result = _service.CreateCoupon(coupon);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(new[] { coupon }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Ajax_UpdateCoupon([DataSourceRequest] DataSourceRequest request, Coupon coupon, int? masterEventId)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(coupon.Code))
                    coupon.Code = _service.GenerateDiscountCode();

                coupon.EventId = masterEventId;

                ServiceResult result = _service.UpdateCoupon(coupon);

                if (!result.Success)
                    Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());
            }

            return Json(ModelState.ToDataSourceResult());
        }

        [HttpPost]
        public ActionResult Ajax_DeleteCoupon([DataSourceRequest] DataSourceRequest request, Coupon coupon)
        {
            ServiceResult result = _service.RemoveCoupon(coupon.DiscountItemId);

            if (!result.Success)
                Utilities.AddModelStateErrors(this.ModelState, result.GetServiceErrors());

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion       
        
        [Serializable]
        public class SerializeableCoupon : Coupon
        {
            public DateTimeOffset StartDateTimeOff { get; set; }
            public DateTimeOffset EndDateTimeOff { get; set; }
        }


    }

}
