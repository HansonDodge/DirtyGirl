using AuthorizeNet;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace DirtyGirl.Services
{
    public class CartService:ServiceBase, ICartService
    {
        private const string PROCESSING_FEE = "Processing Fee";

        #region constructor

        public CartService(IRepositoryGroup repository) : base(repository, false) { }
        public CartService(IRepositoryGroup repository, bool isShared) : base(repository, isShared) { }

        #endregion

        #region Cart
        
        public IList<Cart> GetCartsByDateRange(DateTime start, DateTime end)
        {
          return _repository.Carts.Filter(x => x.TransactionDate >= start && x.TransactionDate <= end).ToList();
        }

        public ServiceResult ProcessCart(CartCheckOut checkOutDetails, SessionCart tempCart, int userId)
        {
            ServiceResult result = new ServiceResult();
            if (checkOutDetails != null && checkOutDetails.CartSummary != null && checkOutDetails.CartSummary.TotalCost > 0)
            {
                DateTime expired = new DateTime();
                expired.AddYears(checkOutDetails.ExpirationYear);
                expired.AddMonths(checkOutDetails.ExpirationMonth);

                if (DateTime.Now.CompareTo(expired) < 0)
                    result.AddServiceError("This credit card is expired");

                Regex rg = new Regex(@"^[a-zA-Z].*$");
                if (string.IsNullOrWhiteSpace(checkOutDetails.CardHolderFirstname))
                {
                    result.AddServiceError("Cardholder first name is required.");
                }
                else if (!rg.IsMatch(checkOutDetails.CardHolderFirstname))
                {
                    result.AddServiceError("Cardholder first name is invalid.");
                }

                if (string.IsNullOrWhiteSpace(checkOutDetails.CardHolderLastname))
                {
                    result.AddServiceError("Cardholder last name is required.");
                }
                else if (!rg.IsMatch(checkOutDetails.CardHolderLastname))
                {
                    result.AddServiceError("Cardholder last name is invalid.");
                }
            }

            if (result.GetServiceErrors().Count > 0)
            {
                return result;
            }
            try
            {
                CartSummary summary = GenerateCartSummary(tempCart);           

                string transactionId = string.Empty;
                CartType cartType;                

                if (summary.TotalCost == 0)
                {
                    transactionId = GenerateCartCode();
                    cartType = CartType.Free;
                }
                else
                {
                    IGatewayResponse payment;
                    payment = ChargeConsumer(checkOutDetails, summary);

                    if (payment.Approved)
                    {
                        transactionId = payment.TransactionID;
                        cartType = CartType.Standard;
                    }
                    else
                    {
                        switch (int.Parse(payment.ResponseCode))
                        {
                            case 2:
                                result.AddServiceError("This Card has been declined.");
                                break;
                            case 3:
                                result.AddServiceError(payment.Message);
                                break;
                            default:
                                result.AddServiceError("Card Error");
                            break;
                        }
                        return result;
                    }             
                }               

                if (!SaveCart(tempCart, summary, userId, transactionId, cartType))
                    result.AddServiceError("An error occured saving the shopping cart");                
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public CartSummary GenerateCartSummary(SessionCart currentCart)
        {
            CartSummary cartSummary = new CartSummary
                {
                    CartItems = GenerateLineItems(currentCart),
                    SummaryMessages = new List<string>()
                };

            ApplySpecials(cartSummary);

            if (!string.IsNullOrEmpty(currentCart.DiscountCode))
            {
                if (!ApplyDiscount(cartSummary, currentCart))
                    currentCart.DiscountCode = string.Empty;
            }

            return cartSummary;
        }

        #region private methods for cart creation

        private bool SaveCart(SessionCart tempCart, CartSummary summary, int userId, string transactionId, CartType cartType)
        {
            try
            {
                Cart cart = new Cart
                {
                    TransactionDate = DateTime.Now,
                    TransactionId = transactionId,
                    UserId = userId,
                    CartType = cartType,
                    CartItems = new List<CartItem>(),
                    TotalCost = summary.TotalCost
                };

                _repository.Carts.Create(cart);
                _repository.SaveChanges();                

                foreach (var summaryItem in summary.CartItems)
                {
                    CartItem newItem = new CartItem
                        {
                            CartId = cart.CartId,
                            PurchaseItemId = summaryItem.PurchaseItemId,
                            Cost = summaryItem.ItemCost,
                            Total = summaryItem.ItemTotal,
                            StandAloneItem = summaryItem.ProcessType == ProcessType.Individual ? true : false,
                            LocalTaxPercentage = summaryItem.LocalTaxPercentage,
                            LocalTaxValue = summaryItem.LocalTax,
                            StateTaxPercentage = summaryItem.StateTaxPercentage,
                            StateTaxValue = summaryItem.StateTax,
                            DiscountItemId = summaryItem.DiscountItemId,
                            DiscountValue = summaryItem.DiscountValue,
                            DiscountType = summaryItem.DiscountType,
                            DiscountValueTotal = summaryItem.DiscountTotal
                        };

                    cart.CartItems.Add(newItem);

                    _repository.SaveChanges();

                    if (summaryItem.SessionKey.HasValue && tempCart.ActionItems[summaryItem.SessionKey.Value].ActionObject != null)
                    {
                        ActionItem action = tempCart.ActionItems[summaryItem.SessionKey.Value];
                        int? discountId = summaryItem.DiscountItemId;
                        CompleteActions(action, newItem.CartItemId, discountId, transactionId);
                    }

                }

                IEmailService emailService = new EmailService();
                emailService.SendPaymentConfirmationEmail(cart.CartId);

                tempCart.ResultingConfirmationCode = cart.TransactionId;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }

        private void CompleteActions(ActionItem item, int cartItemId, int? discountId, string confirmationCode)
        {
            IRegistrationService regService = new RegistrationService(this._repository, false);

            switch (item.ActionType)
            {
                case CartActionType.NewRegistration:
                    var newReg = (Registration)item.ActionObject;
                    newReg.CartItemId = cartItemId;
                    newReg.ConfirmationCode = confirmationCode;
                    regService.CreateNewRegistration(newReg, discountId);
                    break;
                case CartActionType.EventChange:
                    var changeAction = (ChangeEventAction)item.ActionObject;
                    regService.ChangeEvent(changeAction.RegistrationId, changeAction.UpdatedEventWaveId, cartItemId, confirmationCode);
                    break;
                case CartActionType.TransferRregistration:
                    var transferAction = (TransferAction)item.ActionObject;
                    regService.TransferRegistration(transferAction.RegistrationId, transferAction.FullName, transferAction.Email);
                    break;
                case CartActionType.CancelRegistration:
                    var cancelAction = (CancellationAction)item.ActionObject;
                    regService.CancelRegistration(cancelAction.RegistrationId);
                    break;
            }            

        }

        private List<CartSummaryLineItem> GenerateLineItems(SessionCart currentCart)
        {
            List<CartSummaryLineItem> lineItems = new List<CartSummaryLineItem>();
            IRegistrationService regService = new RegistrationService(this._repository, false);

            foreach (var item in currentCart.ActionItems.Where(x => x.Value.ItemReadyForCheckout))
            {

                var lineItem = new CartSummaryLineItem();

                EventService evtService = new EventService(_repository, false);
                Registration reg;
                EventFee fee;
                Event evt;
                EventDate evtDate;
                EventWave evtWave;

                switch (item.Value.ActionType)
                {
                    case CartActionType.NewRegistration:

                        reg = (Registration)item.Value.ActionObject;
                        evtWave = evtService.GetEventWaveById(reg.EventWaveId);
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.Registration);


                        lineItems.Add(GenerateLineItem(evt.EventId, 
                                        item.Key,
                                        PurchaseType.Registration,
                                        ProcessType.Individual, 
                                        "Registration", 
                                        string.Format("{0}, {1} : {2} {3}", evt.GeneralLocality, evt.Region.Code, evtDate.DateOfEvent.ToString("dddd  MMMM dd, yyyy"), evtWave.StartTime.ToString("h:mm tt")),
                                        fee.PurchaseItemId,
                                        reg.RegistrationType == RegistrationType.CancerRegistration ? 0 : fee.Cost,
                                        fee.Discountable,
                                        fee.Taxable,
                                        evt.StateTax,
                                        evt.LocalTax,
                                        true));
                        
                        break;
                    case CartActionType.EventChange:

                        var changeAction = (ChangeEventAction)item.Value.ActionObject;                        
                        evtWave = evtService.GetEventWaveById(changeAction.UpdatedEventWaveId);
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.ChangeEvent);
                        

                        lineItems.Add(GenerateLineItem(evt.EventId, 
                                        item.Key,
                                        PurchaseType.Fee,            
                                        ProcessType.General,
                                        "Event Change",
                                        string.Format("Changing your registration to {0}, {1} : {2} {3}", evt.GeneralLocality, evt.Region.Code, evtDate.DateOfEvent.ToString("dddd  MMMM dd, yyyy"), evtWave.StartTime.ToString("h:mm tt")),
                                        fee.PurchaseItemId,
                                        fee.Cost,
                                        false,
                                        false,
                                        null,
                                        null,
                                        true));


                        var originalCost = regService.GetRegistrationPathValue(changeAction.RegistrationId);
                        var regFee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.Registration);

                        if (regFee.Cost > originalCost)
                        {
                            var additionalcost = regFee.Cost - originalCost;

                            lineItems.Add(GenerateLineItem(evt.EventId,
                                            null,
                                            PurchaseType.FeeDifference,
                                            ProcessType.Individual, 
                                            "Difference Cost",
                                            string.Format("The event you have selected has a fee difference of {0}.", additionalcost.ToString("c")),
                                            1,
                                            additionalcost,
                                            true,
                                            true,
                                            evt.StateTax,
                                            evt.LocalTax,
                                            true));
                        }                        

                        break;
                    case CartActionType.TransferRregistration:
                        var transferAction = (TransferAction)item.Value.ActionObject;                        

                        evtWave = _repository.Registrations.Find(transferAction.RegistrationId).EventWave;
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.Transfer);

                        lineItems.Add(GenerateLineItem(evt.EventId, 
                                        item.Key,
                                        PurchaseType.Fee,
                                        ProcessType.General,
                                        "Transfer to friend",
                                        string.Format("Transfering your registration for {0}, {1} : {2} {3} to {4} using {5}.", evt.GeneralLocality, evt.Region.Code, evtDate.DateOfEvent.ToString("dddd  MMMM dd, yyyy"), evtWave.StartTime.ToString("h:mm tt"), string.Format("{0} {1}", transferAction.FirstName, transferAction.LastName), transferAction.Email),
                                        fee.PurchaseItemId,
                                        fee.Cost,
                                        false,
                                        false,
                                        null,
                                        null,
                                        true));                        

                        break;
                    case CartActionType.CancelRegistration:
                        var cancelAction = (CancellationAction)item.Value.ActionObject;

                        evtWave = _repository.Registrations.Find(cancelAction.RegistrationId).EventWave;
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.Cancellation);

                        lineItems.Add(GenerateLineItem(evt.EventId, 
                                        item.Key,
                                        PurchaseType.Fee,
                                        ProcessType.General,                                        
                                        "Cancellation",
                                        string.Format("Cancelling your registration for {0}, {1} : {2} {3}.  You will be issued a cancellation code of the original value that can be used towards another event.", evt.GeneralLocality, evt.Region.Code, evtDate.DateOfEvent.ToString("dddd  MMMM dd, yyyy"), evtWave.StartTime.ToString("h:mm tt")),
                                        fee.PurchaseItemId,
                                        fee.Cost,
                                        false,
                                        false,
                                        null,
                                        null,
                                        true));      

                        break;

                    case CartActionType.ShippingFee:

                        var shippingCost = (ShippingFeeAction)item.Value.ActionObject;
                        evtWave = evtService.GetEventWaveById(shippingCost.EventWaveId);
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.Shipping);
                        lineItems.Add(GenerateLineItem(evt.EventId,
                                        item.Key,
                                        PurchaseType.Fee,
                                        ProcessType.General,
                                        "Shipping Fee",
                                        "Shipping Fee for registration packet",
                                        fee.PurchaseItemId,
                                        fee.Cost,
                                        false,
                                        false,
                                        null,
                                        null,
                                        false));
                        break;

                    case CartActionType.ProcessingFee:

                        var processingCost = (ProcessingFeeAction)item.Value.ActionObject;
                        evtWave = evtService.GetEventWaveById(processingCost.EventWaveId);
                        evtDate = evtWave.EventDate;
                        evt = evtDate.Event;
                        fee = evtService.GetCurrentFeeForEvent(evt.EventId, EventFeeType.ProcessingFee);
                        lineItems.Add(GenerateLineItem(evt.EventId,
                                        item.Key,
                                        PurchaseType.Fee,
                                        ProcessType.General,
                                        PROCESSING_FEE,
                                        "",
                                        fee.PurchaseItemId,
                                        fee.Cost,
                                        false,
                                        false,
                                        null,
                                        null,
                                        false));
                        break; 
                }               

            }           

            return lineItems;
        }

        private CartSummaryLineItem GenerateLineItem(int eventId, Guid? itemId, PurchaseType purchaseType, ProcessType processType, string name, string description, int? purchaseItemId, decimal cost, bool discountable, bool taxable, decimal? stateTax, decimal? localTax, bool removable)
        {
            var lineItem = new CartSummaryLineItem
            {
                EventId = eventId,
                SessionKey = itemId,
                PurchaseType = purchaseType,
                ProcessType = processType,
                ItemName = name,
                ItemDescription = description,
                PurchaseItemId = purchaseItemId.Value,
                ItemCost = cost,
                Discountable = discountable,
                Taxable = taxable, 
                Removable = removable
            };

            if (lineItem.Taxable)
            {
                lineItem.StateTaxPercentage = stateTax;
                lineItem.LocalTaxPercentage = localTax;
            }

            return lineItem;
        }

        private void ApplySpecials(CartSummary cartSummary)
        {
            //Apply upsale: if there is more than one registration give half off to the lowest registration
            var regList = cartSummary.CartItems.Where(x => x.PurchaseType == PurchaseType.Registration && x.ItemCost > 0).OrderBy(x => x.ItemCost).ToList();

            if (regList.Count > 1)
            {
                regList[0].ItemCost = regList[0].ItemCost / 2;
                regList[0].ItemDescription += " including upsale.";
            }
        }

        private bool ApplyDiscount(CartSummary cartSummary, SessionCart currentCart)        
        {
            DiscountItem discount = _repository.DiscountItems.Find(x => x.Code.ToLower() == currentCart.DiscountCode.ToLower());

            if (discount != null)
            {
                if (cartSummary.TotalCost == 0) {
                    discount = null;
                    currentCart.DiscountCode = null;
                    cartSummary.SummaryMessages.Add("You cannot apply a discount code to this cart. There are no charges.");
                }

                if (discount is RedemptionCode)
                {
                    var discountableRegList = cartSummary.CartItems.Where(x => x.PurchaseType == PurchaseType.Registration && x.Discountable == true).OrderByDescending(x => x.ItemCost).ToList();
                    
                    if (discountableRegList.Count() > 0)
                    {
                        RedemptionCode code = (RedemptionCode)discount;

                        IDiscountService discountService = new DiscountService(this._repository, false);
                        ServiceResult validationResult = discountService.ValidateDiscount(code);

                        if (validationResult.Success)
                        {
                            var cost = discountableRegList[0].ItemCost;
                            var discountValue = code.DiscountType == DiscountType.Dollars ? code.Value : cost * (code.Value / 100);                            
                            
                            discountableRegList[0].DiscountItemId = code.DiscountItemId;
                            discountableRegList[0].DiscountDescription = code.Code;
                            discountableRegList[0].DiscountType = code.DiscountType;
                            discountableRegList[0].DiscountValue = discountableRegList[0].ItemTotal <= 0 ? cost : discountValue;

                            if (NeedToRemoveProcessingFee(currentCart, code))
                                RemoveProcessingFee(cartSummary);
                        }
                        else
                            cartSummary.SummaryMessages.Add(validationResult.GetServiceErrors().First().ErrorMessage);
                    }
                    else
                        cartSummary.SummaryMessages.Add("There are no applicable items for this discount.");                    
                }

                if (discount is Coupon)
                {
                    Coupon coupon = (Coupon)discount;
                    IDiscountService discountService = new DiscountService(this._repository, false);
                    ServiceResult validationResult = discountService.ValidateDiscount(coupon);

                    if (validationResult.Success)
                    {
                        switch (coupon.CouponType)
                        {
                            case CouponType.Registration:
                                var discountableRegList = cartSummary.CartItems.Where(
                                                x => x.PurchaseType == PurchaseType.Registration && 
                                                x.Discountable == true && 
                                                x.EventId == ((coupon.EventId.HasValue) ? coupon.EventId.Value : x.EventId)).OrderByDescending(x => x.ItemCost).ToList();

                                if (discountableRegList.Any())
                                {
                                    var cost = discountableRegList[0].ItemCost;
                                    var discountValue = coupon.DiscountType == DiscountType.Dollars ? coupon.Value : cost * (coupon.Value / 100); 
                                    var discountedCost = cost - discountValue;

                                    discountableRegList[0].DiscountItemId = coupon.DiscountItemId;
                                    discountableRegList[0].DiscountDescription = coupon.Code;
                                    discountableRegList[0].DiscountType = coupon.DiscountType;
                                    discountableRegList[0].DiscountValue = discountableRegList[0].ItemTotal <= 0 ? cost : discountValue;

                                    if (NeedToRemoveProcessingFee(currentCart, coupon))
                                        RemoveProcessingFee(cartSummary);
                                }
                                else
                                    cartSummary.SummaryMessages.Add("There are no applicable items for this discount.");

                                break;
                        }
                    }
                    else
                        cartSummary.SummaryMessages.Add(validationResult.GetServiceErrors().First().ErrorMessage);                    
                }
            }
            else
                cartSummary.SummaryMessages.Add("This discount does not exist.");

            return cartSummary.SummaryMessages.Count <= 0;
            
        }

       // remove all the processing fees from this cart...
        private void RemoveProcessingFee(CartSummary cartSummary)
        {
            var fees =
                cartSummary.CartItems.Where(
                    item => item.PurchaseType == PurchaseType.Fee && item.ItemName == PROCESSING_FEE).ToList();
            foreach (var item in fees)
            {                
                cartSummary.CartItems.Remove(item);
            }            
        }

        private bool NeedToRemoveProcessingFee(SessionCart currentCart)
        {
            if (currentCart.ActionItems.All(x => (x.Value as ActionItem).ActionType != CartActionType.ProcessingFee))
                return false;

            return true;
        }

        private bool NeedToRemoveProcessingFee(SessionCart currentCart, RedemptionCode code)
        {
            if (!NeedToRemoveProcessingFee(currentCart))            // check base method
                return false;

            if (code.RedemptionCodeType == RedemptionCodeType.StraightValue ||      // check out redemption type
                code.RedemptionCodeType == RedemptionCodeType.Transfer)
                return true;

            return false;                                           // no good reason to remove it...
        }        

        private bool NeedToRemoveProcessingFee(SessionCart currentCart, Coupon coupon)
        {
            if (!NeedToRemoveProcessingFee(currentCart))            // check base method
                return false;

            if (coupon.CouponType != CouponType.Registration)       // only if a registration coupon
                return false;

            if (coupon.DiscountType == DiscountType.Dollars)        //  remove for all dollar amounts
                return true;

            if (coupon.DiscountType == DiscountType.Percentage && coupon.Value >= 75) // only 75% disocunt or greater
                return true;

            return false;
        }

       
        private IGatewayResponse ChargeConsumer(CartCheckOut checkOutDetails, CartSummary cartSummary)
        {                       

            var paymentRequest = new AuthorizationRequest(checkOutDetails.CardNumber, 
                                                    string.Format("{0}{1}", checkOutDetails.ExpirationMonth, checkOutDetails.ExpirationYear), 
                                                    cartSummary.TotalCost, 
                                                    "Dirty Girl Cart Purchase", 
                                                    true);
            
            paymentRequest.FirstName = checkOutDetails.CardHolderFirstname;
            paymentRequest.LastName = checkOutDetails.CardHolderLastname;
            paymentRequest.Zip = checkOutDetails.CardHolderZipCode;
            paymentRequest.CardCode = checkOutDetails.CCVNumber;

            var totalTax = 0.0M;
            foreach (var item in cartSummary.CartItems)
            {
                paymentRequest.AddLineItem(item.PurchaseItemId.ToString(), item.ItemName, item.DiscountDescription, 1, item.ItemTotal, item.Taxable);
                if (item.Taxable)
                    totalTax += (item.StateTax + item.LocalTax);
            }

            paymentRequest.AddTax(totalTax);

            var gateway = new Gateway(DirtyGirlServiceConfig.Settings.PaymentGatewayId, DirtyGirlServiceConfig.Settings.PaymentGatewayKey, true);
            gateway.TestMode = DirtyGirlServiceConfig.Settings.PaymentTestMode;            

            return gateway.Send(paymentRequest);
        }

        private string GenerateCartCode()
        {
            string cartCode = Path.GetRandomFileName().ToUpper().Replace(".", "");

            while (!string.IsNullOrEmpty(cartCode) && _repository.Carts.Filter(r => r.TransactionId.ToUpper() == cartCode.ToUpper()).Any())
            {
                cartCode = Path.GetRandomFileName().ToUpper().Replace(".", "");
            }

            return cartCode;
        }

        #endregion

        #endregion

        #region CartItems

        public IList<CartItem> GetDiscountCartItemsByDateRange(DateTime start, DateTime end)
        {
          return _repository.CartItems.Filter(x => x.DateAdded >= start && x.DateAdded <= end && x.DiscountItemId != null ).ToList();
        }

        public IList<CartItem> GetCartItemsByDateRange(DateTime start, DateTime end)
        {
          return _repository.CartItems.Filter(x => x.DateAdded >= start && x.DateAdded <= end).ToList();
        }

        public IList<CartItem> GetCartItemsByPurchaseItemList(List<int> purchaseItemList)
        {
          return _repository.CartItems.Filter(r => purchaseItemList.Contains(r.PurchaseItemId)).ToList();
        }

        public CartItem GetCartItemByCartItemId(int cartItemId)
        {
          return _repository.CartItems.Filter(r => r.CartItemId == cartItemId).First();
        }
       
        #endregion

        #region Event

        public EventWave GetEventWaveById(int eventWaveId)
        {
            return _repository.EventWaves.Find(x => x.EventWaveId == eventWaveId);
        }

        #endregion

        #region Charges

        public CartCharge GetChargeById(int purchaseItemId)
        {
            return _repository.Charges.Find(x => x.PurchaseItemId == purchaseItemId);
        }

        #endregion

        #region Discounts

        public DiscountItem GetDiscountByCode(string code)
        {
            RedemptionCode redemptionCode = _repository.RedemptionCodes.Filter(x => x.Code.ToLower() == code.ToLower()).FirstOrDefault();            
            
            if (redemptionCode != null)
                return redemptionCode;
            else
                return  _repository.Coupons.Filter(x => x.Code.ToLower() == code.ToLower() && x.StartDateTime <= DateTime.Now && (x.EndDateTime == null || x.EndDateTime > DateTime.Now)).OrderByDescending(x => x.StartDateTime).FirstOrDefault();      

        }

        #endregion     

    }
}
