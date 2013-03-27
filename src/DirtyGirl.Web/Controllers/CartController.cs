using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Controllers
{
    public class CartController : BaseController
    {

        #region constructor

        private ICartService _service;

        public CartController(ICartService service)
        {
            this._service = service;
        }

        #endregion

        #region CheckOut

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult CheckOut()
        {
            if (!Utilities.IsValidCart())
                return RedirectToAction("Index", "home");

            CartCheckOut vm = new CartCheckOut 
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
                var CartFocusType = SessionManager.CurrentCart.CheckOutFocus;

                SessionManager.CurrentCart = null;
                return RedirectToAction("ThankYou", new {id = CartFocusType, confirm = confirmationCode});
            }
            else
                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());                                   

            model.ExpirationMonthList = DirtyGirlExtensions.ConvertToSelectList<Months>();
            model.ExpirationYearList = Utilities.CreateNumericSelectList(DateTime.Now.Year, DateTime.Now.AddYears(20).Year);
            model.CartSummary = _service.GenerateCartSummary(SessionManager.CurrentCart);

            return View(model);
        }
        
        #endregion

        #region ThankYou

        public ActionResult ThankYou(CartFocusType id, string confirm)
        {
            vmCart_ThankYou vm = new vmCart_ThankYou
                {
                    CartFocus = id,
                    ConfirmationCode = confirm,
                    UserName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName)
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
            
            // if new event- remove shipping charge related to that event if any 
            if (removeAction.ActionType == CartActionType.NewRegistration)
            {
                Registration reg = (Registration)removeAction.ActionObject;
                if (reg.PacketDeliveryOption.HasValue &&  (int)reg.PacketDeliveryOption.Value == 1)
                {
                    var shippingActions = SessionManager.CurrentCart.ActionItems.Where(x => (x.Value as ActionItem).ActionType == CartActionType.ShippingFee).ToList();
                    foreach (var ship in shippingActions)
                    {
                        var shipAction = ship.Value.ActionObject as ShippingFeeAction;
                        if (shipAction.RegItemGuid == itemId)
                        {
                            SessionManager.CurrentCart.ActionItems.Remove(ship.Key);
                        }
                    }
                }
            }
            // else if shipping charge- update registration DB to remove mailing option  
            else if (removeAction.ActionType == CartActionType.ShippingFee)
            {
                ShippingFeeAction shipAction = (ShippingFeeAction)removeAction.ActionObject;
                Registration reg = (Registration)SessionManager.CurrentCart.ActionItems[shipAction.RegItemGuid].ActionObject;
                reg.PacketDeliveryOption = 0; 
                SessionManager.CurrentCart.ActionItems[shipAction.RegItemGuid].ActionObject = reg;  
            }

            // remove initial request 
            SessionManager.CurrentCart.ActionItems.Remove(itemId);

            if (SessionManager.CurrentCart.ActionItems.Count() == 0 )
            {
                SessionManager.CurrentCart = null;
                return RedirectToAction("index", "home");
            }

            return RedirectToAction("CheckOut");
        }

        [HttpPost]
        public ActionResult AddDiscount(string txtCouponCode)
        {
            SessionManager.CurrentCart.DiscountCode = txtCouponCode;
            return RedirectToAction("CheckOUt");         
        }

        #endregion

        #region private methods
        
        
        #endregion

    }
}
