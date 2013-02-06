using DirtyGirl.Models;
using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace DirtyGirl.Web.Utils
{
	public static class SessionManager
	{
        public static SessionCart CurrentCart
        {
            get 
            {
                var currentCart = (SessionCart)HttpContext.Current.Session[DirtyGirlConfig.Settings.CurrentCartKey];

                if (currentCart == null)
                {
                    currentCart = new SessionCart();
                    currentCart.ActionItems = new Dictionary<Guid, ActionItem>();                    
                    HttpContext.Current.Session[DirtyGirlConfig.Settings.CurrentCartKey] = currentCart;
                }
             
                return currentCart;
            }

            set
            {
                HttpContext.Current.Session[DirtyGirlConfig.Settings.CurrentCartKey] = value;
            }
        }       

	}
}