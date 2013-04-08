using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Data.DataInterfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Data.DataInterfaces.RepositoryGroups
{
    public interface IRepositoryGroup: IUnitOfWork
    {

        #region Events

        IEventRepository Events { get; }

        IEventDateRepository EventDates { get; }

        IEventWaveRepository EventWaves { get; }

        IEventSponsorRepository EventSponsors { get; }

        IEventFeeRepository EventFees { get;}

        IEventTemplateRepository EventTemplates { get; }

        IEventTemplate_PayScaleRepository EventTemplate_PayScales { get; }

        IEventLeadRepository EventLeads { get; }

        IEventLeadTypeRepository EventLeadTypes { get; }

        IDirtyWordRepository DirtyWord { get; }

        #endregion

        #region Shopping Cart

        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IPurchaseItemRepository PurchaseItems { get; }
        ICartDiscountItemRepository CartDiscountItems { get; }
        IDiscountItemRepository DiscountItems { get; }
        ICouponRepository Coupons { get; }
        IRedemptionCodeRepository RedemptionCodes { get; } 

        #endregion

        #region registrations

        IRegistrationRepository Registrations { get; }
                   
        #endregion

        #region Charges

        IChargeRepository Charges { get; }

        #endregion

        #region Regions

        IRegionRepository Regions { get; }

        #endregion

        #region Users

        IUserRepository Users { get; }

        IRoleRepository Roles { get; }

        IUser_RoleRepository UserRoles { get; }

        #endregion

        #region Teams

        ITeamRepository Teams { get; }

        ITeamPostRepository TeamPosts { get; }

        #endregion

    }
}
