using System.Collections.Generic;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DirtyGirl.Web.Controllers
{
    public class CartController : BaseController
    {

        #region constructor

// ReSharper disable FieldCanBeMadeReadOnly.Local
        private ICartService _service;
// ReSharper restore FieldCanBeMadeReadOnly.Local

        public CartController(ICartService service)
        {
            _service = service;
        }

        #endregion

        #region CheckOut

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult CheckOut()
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            var vm = new CartCheckOut 
                {                    
                    ExpirationMonthList = DirtyGirlExtensions.ConvertToSelectList<Months>(),
                    ExpirationYearList = Utilities.CreateNumericSelectList(DateTime.Now.Year, DateTime.Now.AddYears(20).Year),
                    CardHolderFirstname = CurrentUser.FirstName,
                    CardHolderLastname = CurrentUser.LastName,
                    CardHolderZipCode = CurrentUser.PostalCode,
                    CartSummary = _service.GenerateCartSummary(SessionManager.CurrentCart)
                };

                        
            return View(vm);
        }

        [HttpPost]
        public ActionResult CheckOut(CartCheckOut model)
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            ServiceResult result = _service.ProcessCart(model, SessionManager.CurrentCart, CurrentUser.UserId);

            if (result.Success)
            {
                var confirmationCode = SessionManager.CurrentCart.ResultingConfirmationCode;
                var cartFocusType = SessionManager.CurrentCart.CheckOutFocus;
                var eventCity = (string.IsNullOrEmpty(SessionManager.CurrentCart.EventCity)) ? "" : SessionManager.CurrentCart.EventCity;
                var summary = _service.GenerateCartSummary(SessionManager.CurrentCart);
                TempData["cartSummary"] = summary;

                // ensure cart focus is correct
                ResetCartFocus();

                SessionManager.CurrentCart = null;
                return RedirectToAction("ThankYou", new { id = cartFocusType, confirm = confirmationCode, city = eventCity });
            }
            
            Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            model.ExpirationMonthList = DirtyGirlExtensions.ConvertToSelectList<Months>();
            model.ExpirationYearList = Utilities.CreateNumericSelectList(DateTime.Now.Year, DateTime.Now.AddYears(20).Year);
            model.CartSummary = _service.GenerateCartSummary(SessionManager.CurrentCart);

            return View(model);
        }
        
        #endregion

        #region ThankYou

        public ActionResult ThankYou(CartFocusType id, string confirm, string city,List<CartSummaryLineItem> lineItems)
        {
            var summary = TempData["cartSummary"] as CartSummary;
            
            var vm = new vmCart_ThankYou
                {
                    Summary = summary,
                    CartFocus = id,
                    ConfirmationCode = confirm,
                    UserName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName), 
                    EventCity = city
                };
            
            return View(vm);
        }

        #endregion

        #region ajax methods

        public ActionResult RemoveItem(Guid itemId)
        {
            
            // shouldn't ever happen but just incase... 
            if (!SessionManager.CurrentCart.ActionItems.ContainsKey(itemId)){
                return RedirectToAction("CheckOut");
            }
           
            // entering the danger zone... 
            ActionItem removeAction = SessionManager.CurrentCart.ActionItems[itemId];
            
            // if new event- remove shipping and/or processing charge related to that event if any 
            if (removeAction.ActionType == CartActionType.NewRegistration)
            {
                var processingActions = SessionManager.CurrentCart.ActionItems.Where(x => x.Value.ActionType == CartActionType.ProcessingFee).ToList();
                foreach (var proc in processingActions)
                {
                    var processesAction = proc.Value.ActionObject as ProcessingFeeAction;
                    if (processesAction != null && processesAction.RegItemGuid == itemId)
                    {
                        SessionManager.CurrentCart.ActionItems.Remove(proc.Key);
                    }
                }

                var reg = (Registration)removeAction.ActionObject;
                if (reg.PacketDeliveryOption.HasValue &&  (int)reg.PacketDeliveryOption.Value == 1)
                {
                  
                    var shippingActions = SessionManager.CurrentCart.ActionItems.Where(x => x.Value.ActionType == CartActionType.ShippingFee).ToList();
                    foreach (var ship in shippingActions)
                    {
                        var shipAction = ship.Value.ActionObject as ShippingFeeAction;
                        if (shipAction != null && shipAction.RegItemGuid == itemId)
                        {
                            SessionManager.CurrentCart.ActionItems.Remove(ship.Key);
                        }
                    }
                }
            }
            // else if shipping charge- update registration DB to remove mailing option  
            else if (removeAction.ActionType == CartActionType.ShippingFee)
            {
                var shipAction = (ShippingFeeAction)removeAction.ActionObject;
                var reg = (Registration)SessionManager.CurrentCart.ActionItems[shipAction.RegItemGuid].ActionObject;
                reg.PacketDeliveryOption = 0; 
                SessionManager.CurrentCart.ActionItems[shipAction.RegItemGuid].ActionObject = reg;  
            }

            // remove initial request 
            SessionManager.CurrentCart.ActionItems.Remove(itemId);

            if (!SessionManager.CurrentCart.ActionItems.Any() )
            {
                SessionManager.CurrentCart = null;
                return RedirectToAction("index", "home");
            }

            ResetCartFocus();

            return RedirectToAction("CheckOut");
        }

        // check if there is a registration item in the cart.  If so, the focus should be registration
        // otherwise set the focus to the first item in the cart
        private void ResetCartFocus()
        {
            if (SessionManager.CurrentCart.ActionItems.Any(item => item.Value.ActionType == CartActionType.NewRegistration))
            {
                SessionManager.CurrentCart.CheckOutFocus = CartFocusType.Registration;
                return;
            }

            var curItem = SessionManager.CurrentCart.ActionItems.FirstOrDefault(x => x.Value.ActionType != CartActionType.ProcessingFee &&
                                                                                     x.Value.ActionType != CartActionType.ShippingFee);

            switch (curItem.Value.ActionType)
            {
                case CartActionType.CancelRegistration:
                    SessionManager.CurrentCart.CheckOutFocus = CartFocusType.CancelEvent;
                    break;
                case CartActionType.EventChange:
                    SessionManager.CurrentCart.CheckOutFocus = CartFocusType.ChangeEvent;
                    break;
                case CartActionType.TransferRregistration:
                    SessionManager.CurrentCart.CheckOutFocus = CartFocusType.TransferEvent;
                    break;
                case CartActionType.WaveChange:
                    SessionManager.CurrentCart.CheckOutFocus = CartFocusType.ChangeWave;
                    break;
            }
        }

        [HttpPost]
        public ActionResult AddDiscount(string txtCouponCode)
        {
            SessionManager.CurrentCart.DiscountCode = txtCouponCode;
            return RedirectToAction("CheckOut");
        }

        #endregion

        #region private methods
        
        
        #endregion

    }
}
