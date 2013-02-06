using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface ICartService
    {
        #region Cart

        IList<Cart> GetCartsByDateRange(DateTime start, DateTime end);        

        ServiceResult ProcessCart(CartCheckOut checkOutDetails, SessionCart cart, int userId);

        CartSummary GenerateCartSummary(SessionCart currentCart);

        #endregion

        #region Cart Items

        IList<CartItem> GetDiscountCartItemsByDateRange(DateTime start, DateTime end);
        
        IList<CartItem> GetCartItemsByDateRange(DateTime start, DateTime end);

        IList<CartItem> GetCartItemsByPurchaseItemList(List<int> purchaseItemList);

        CartItem GetCartItemByCartItemId(int cartItemId);

        #endregion       
        
        #region Cart Charge

        CartCharge GetChargeById(int purchaseItemId); 

        #endregion

        #region Discounts

        DiscountItem GetDiscountByCode(string code);

        #endregion

        #region EventWave

        EventWave GetEventWaveById(int eventWaveId);

        #endregion


    }
}
