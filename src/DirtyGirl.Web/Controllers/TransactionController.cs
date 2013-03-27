using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Utils;
using System;
using System.Web.Mvc;

namespace DirtyGirl.Web.Controllers
{
    [Authorize(Roles = "Registrant, Admin, SuperUser")]
    public class TransactionController : BaseController
    {

        #region constructor

        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        #endregion

        #region new Registration

        public ActionResult StartNewRegistration(int? eventId, int? eventDateId)
        {
            var itemId = Guid.NewGuid();
            var newReg = new Registration
                {
                    UserId = CurrentUser.UserId,
                    FirstName = CurrentUser.FirstName,
                    LastName = CurrentUser.LastName,
                    Address1 = CurrentUser.Address1,
                    Address2 = CurrentUser.Address2,
                    Locality = CurrentUser.Locality,
                    RegionId = CurrentUser.RegionId,
                    PostalCode = CurrentUser.PostalCode,
                    Email = CurrentUser.EmailAddress,
                    RegistrationStatus = RegistrationStatus.Active
                };            
            var newCartItem = new ActionItem
                {
                    ActionType = CartActionType.NewRegistration,                                      
                    ActionObject = newReg,
                    ItemReadyForCheckout = false
                };
           
            SessionManager.CurrentCart.ActionItems.Add(itemId, newCartItem);
            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.Registration;

            return RedirectToAction("eventSelection", "registration", new { itemId, eventId, eventDateId });
        }

        #endregion

        #region ChangeEvent

        
        public ActionResult StartChangeEvent(int eventId, int regId, int waveId, int dateId)
        {        
            if (!IsRegistrationAlreadyInCart(regId))
            {
                var newItemId = Guid.NewGuid();

                var newAction = new ChangeEventAction
                    {
                        RegistrationId = regId
                    };
                var newCartItem = new ActionItem
                    {
                        ActionType = CartActionType.EventChange,
                        ActionObject = newAction,
                        ItemReadyForCheckout = false
                    };

                SessionManager.CurrentCart.ActionItems.Add(newItemId, newCartItem);
                SessionManager.CurrentCart.CheckOutFocus = CartFocusType.ChangeEvent;

                return RedirectToAction("EventSelection", "registration",
                                        new
                                            {
                                                itemId = newItemId,
                                                eventId = eventId,
                                                eventDateId = dateId,
                                                eventWaveId = waveId
                                            });
            }
            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.CancelEvent;

            return RedirectToAction("checkout", "cart");
        }

        #endregion

        #region Change Wave

        public ActionResult StartChangeWave(int regId)
        {           
            var itemId = Guid.NewGuid();
            var newAction = new ChangeWaveAction 
                { 
                    RegistrationId = regId 
                };
            var newCartItem = new ActionItem
            {
                ActionType = CartActionType.WaveChange,               
                ActionObject = newAction,
                ItemReadyForCheckout = false
            };

            SessionManager.CurrentCart.ActionItems.Add(itemId, newCartItem);
            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.ChangeWave;

            var reg = _service.GetRegistrationById(regId);
            return RedirectToAction("EventSelection", "registration", new { itemId, reg.EventWave.EventDate.EventId, reg.EventWave.EventDateId, reg.EventWaveId});
        }

        #endregion

        #region Cancel Registration

        public ActionResult StartCancellation(int regId)
        {          
            if (!IsRegistrationAlreadyInCart(regId))
            {
                var itemId = Guid.NewGuid();
                var newAction = new CancellationAction
                    {
                        RegistrationId = regId
                    };
                var newCartItem = new ActionItem
                    {
                        ActionType = CartActionType.CancelRegistration,
                        ActionObject = newAction,
                        ItemReadyForCheckout = true
                    };

                SessionManager.CurrentCart.ActionItems.Add(itemId, newCartItem);
            }
            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.CancelEvent;

            return RedirectToAction("checkout", "cart");
        }

        private static bool IsRegistrationAlreadyInCart(int regId)
        {
            foreach (var actionItem in SessionManager.CurrentCart.ActionItems.Values)
            {
                if (actionItem.ActionType == CartActionType.CancelRegistration)
                {
                    var cancelAction = (CancellationAction)actionItem.ActionObject;
                    if (cancelAction.RegistrationId == regId)
                        return true;
                }
                if (actionItem.ActionType == CartActionType.TransferRregistration)
                {
                    var transferAction = (TransferAction)actionItem.ActionObject;
                    if (transferAction.RegistrationId == regId)
                        return true;
                }
                if (actionItem.ActionType == CartActionType.EventChange)
                {
                    var changeAction = (ChangeEventAction)actionItem.ActionObject;
                    if (changeAction.RegistrationId == regId)
                        return true;
                }

            }
            return false;
        }

        #endregion

        #region Transfer Registration

        public ActionResult StartTransfer(int regId)
        {
            if (!IsRegistrationAlreadyInCart(regId))
            {
                var itemId = Guid.NewGuid();
                var newAction = new TransferAction
                    {
                        RegistrationId = regId
                    };
                var newCartItem = new ActionItem
                    {
                        ActionType = CartActionType.TransferRregistration,
                        ActionObject = newAction,
                        ItemReadyForCheckout = false
                    };

                SessionManager.CurrentCart.ActionItems.Add(itemId, newCartItem);
                SessionManager.CurrentCart.CheckOutFocus = CartFocusType.TransferEvent;

                return RedirectToAction("transfer", "registration", new {itemId});
            }
            SessionManager.CurrentCart.CheckOutFocus = CartFocusType.CancelEvent;

            return RedirectToAction("checkout", "cart");
        }


        #endregion

        #region Use Redemption Code 

        public ActionResult StartRedemption(string id)
        {
            ServiceResult result = _service.ValidateRedemptionCode(id);

            if (result.Success)            
            {
                RedemptionCode redemptionCode = _service.GetRedemptionCode(id);
                var itemId = Guid.NewGuid();
                var newReg = new Registration
                {                     
                    UserId = CurrentUser.UserId,
                    FirstName = CurrentUser.FirstName,
                    LastName = CurrentUser.LastName,
                    Address1 = CurrentUser.Address1,
                    Address2 = CurrentUser.Address2,
                    Locality = CurrentUser.Locality,
                    RegionId = CurrentUser.RegionId,
                    PostalCode = CurrentUser.PostalCode,
                    Email = CurrentUser.EmailAddress,
                    RegistrationStatus = RegistrationStatus.Active,                       
                };

                if (redemptionCode.RedemptionCodeType == RedemptionCodeType.Transfer)
                {
                    newReg.EventWaveId = redemptionCode.GeneratingRegistration.EventWaveId;
                    newReg.TeamId = redemptionCode.GeneratingRegistration.TeamId;                    
                }

                SessionManager.CurrentCart.DiscountCode = id;
                
                var newCartItem = new ActionItem
                {
                    ActionType = CartActionType.NewRegistration,
                    ActionObject = newReg,                   
                    ItemReadyForCheckout = false
                }; 
                
                SessionManager.CurrentCart.ActionItems.Add(itemId, newCartItem);
                SessionManager.CurrentCart.CheckOutFocus = CartFocusType.Registration;

                if (redemptionCode.RedemptionCodeType == RedemptionCodeType.Transfer)
                    return RedirectToAction("registrationdetails", "Registration", new {itemId});

                SessionManager.CurrentCart.CheckOutFocus = CartFocusType.Redemption;
                return RedirectToAction("eventselection", "registration", new {itemId});               
            }

            string error = "Invalid Redemption Code";
            var errors = result.GetServiceErrors();
            if (errors != null && errors.Count > 0)
                error = errors[0].ErrorMessage;

 
            return RedirectToAction("InvalidRedemption", "registration", new { m = error });
        }

        #endregion
    }
}
