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
using System.Net.Http;


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
        public ActionResult Ajax_GetCoupons([DataSourceRequest] DataSourceRequest request, int? masterEventId, string search)
        {

            var couponList = _service.GetCouponsByEvent(masterEventId).Select(x => new { x.DiscountItemId, x.Code, x.CouponType, x.DiscountType, x.Description, x.EndDateTime, x.IsActive, x.IsReusable, x.MaxRegistrantCount, x.StartDateTime, x.Value }).ToList();
            
            if (!string.IsNullOrEmpty(search)) {
                couponList = couponList.Where(x => x.Code.ToLower().Contains(search.ToLower())).ToList();   
            }
            
            return Json(couponList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);            
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
        
    }

}
