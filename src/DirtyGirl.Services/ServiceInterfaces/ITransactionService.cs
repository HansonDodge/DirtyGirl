using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface ITransactionService
    {

        #region redemption code data

        RedemptionCode GetRedemptionCode(string code);

        ServiceResult ValidateRedemptionCode(string code);

        #endregion
        
        #region Registration data

        Registration GetRegistrationById(int regId);

        #endregion        

        #region fee data

        EventFee GetEventFeeByPurchaseItem(int purchaseItemId);

        #endregion

    }
}
