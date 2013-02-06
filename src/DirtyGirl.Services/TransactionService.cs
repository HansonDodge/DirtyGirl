using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.Linq;

namespace DirtyGirl.Services
{
    public class TransactionService:ServiceBase, ITransactionService
    {
        
        #region constructor

        public TransactionService(IRepositoryGroup repository):base(repository, false){}

        public TransactionService(IRepositoryGroup repository, bool isShared) : base(repository, isShared) { }

        #endregion

        #region redemption code data

        public RedemptionCode GetRedemptionCode(string code)
        {
            return _repository.RedemptionCodes.Find(x => x.Code.ToLower() == code.ToLower());
        }

        public ServiceResult ValidateRedemptionCode(string code)
        {
            DiscountService discountService = new DiscountService(this._repository, false);
            return discountService.ValidateRedemptionCode(code);
        }

        #endregion

        #region registration data

        public Registration GetRegistrationById(int regId)
        {
            return _repository.Registrations.Find(x => x.RegistrationId == regId);
        }

        #endregion       

        #region fee data

        public EventFee GetEventFeeByPurchaseItem(int purchaseItemId)
        {
            return _repository.EventFees.Find(x => x.PurchaseItemId == purchaseItemId);
        }

        #endregion

    }
}
