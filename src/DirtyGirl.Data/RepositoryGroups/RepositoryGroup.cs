using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Data.DataRepositories;
using System;
using System.Data.Entity.Validation;
using System.Text;

namespace DirtyGirl.Data.RepositoryGroups
{
    public class RepositoryGroup: IRepositoryGroup
    {
        #region private members

        private DB dbContext;

        #region Events
        private IEventRepository events;
        private IEventDateRepository eventDates;
        private IEventWaveRepository eventWaves;
        private IEventSponsorRepository eventSponsors;
        private IEventFeeRepository eventFees;
        private IEventTemplateRepository eventTemplates;
        private IEventTemplate_PayScaleRepository eventTemplate_PayScales;
        private IEventLeadRepository eventLeads;
        private IEventLeadTypeRepository eventLeadTypes;

        #endregion

        #region registrations

        private IRegistrationRepository registrations;
                
        #endregion

        #region regions

        private IRegionRepository regions;

        #endregion

        #region charges

        private IChargeRepository charges;

        #endregion

        #region Shopping Cart

        private ICartRepository carts;
        private ICartItemRepository cartItems;
        private IPurchaseItemRepository purchaseItems;
        private ICartDiscountItemRepository cartDiscountItems;
        private ICouponRepository coupons;
        private IDiscountItemRepository discountItems;
        private IRedemptionCodeRepository redemptionCodes;

        #endregion

        #region teams

        private ITeamRepository teams;
        private ITeamPostRepository teamPosts;

        #endregion

        #region users

        private IUserRepository users;
        private IRoleRepository roles;

        #endregion

        #endregion

        #region public members

        #region events

        public IEventRepository Events
        {
            get
            {
                if (events == null)
                    events = new EventRepository(dbContext);
                return events;
            }
        }

        public IEventDateRepository EventDates
        {
            get
            {
                if (eventDates == null)
                    eventDates = new EventDateRepository(dbContext);

                return eventDates;
            }
        }

        public IEventWaveRepository EventWaves
        {
            get
            {
                if (eventWaves == null)
                    eventWaves = new EventWaveRepository(dbContext);

                return eventWaves;
            }
        }

        public IEventSponsorRepository EventSponsors
        {
            get
            {
                if (eventSponsors == null)
                    eventSponsors = new EventSponsorRepository(dbContext);

                return eventSponsors;
            }
        }

        public IEventTemplateRepository EventTemplates
        {
            get
            {
                if (eventTemplates == null)
                    eventTemplates = new EventTemplateRepository(dbContext);

                return eventTemplates;
            }
        }

        public IEventFeeRepository EventFees
        {
            get
            {
                if (eventFees == null)
                    eventFees = new EventFeeRepository(dbContext);
                return eventFees;
            }
        }

        
        public IEventTemplate_PayScaleRepository EventTemplate_PayScales
        {
            get
            {
                if (eventTemplate_PayScales == null)
                    eventTemplate_PayScales = new EventTemplate_PayScaleRepository(dbContext);

                return eventTemplate_PayScales;
            }
        }

        public IEventLeadRepository EventLeads
        {
            get { return eventLeads ?? (eventLeads = new EventLeadRepository(dbContext)); }
        }

        public IEventLeadTypeRepository EventLeadTypes
        {
            get { return eventLeadTypes ?? (eventLeadTypes = new EventLeadTypeRepository(dbContext)); }
        }

        #endregion

        #region regions

        public IRegionRepository Regions
        {
            get
            {
                if (regions == null)
                    regions = new RegionRepository(dbContext);
                return regions;
            }
        }

        #endregion

        #region charges

        public IChargeRepository Charges
        {
            get
            {
                if (charges == null)
                    charges = new ChargeRepository(dbContext);
                return charges;
            }
        }

        #endregion

        #region registrations

        public IRegistrationRepository Registrations
        {
            get
            {
                if (registrations == null)
                    registrations = new RegistrationRepository(dbContext);
                return registrations;
            }
        }

        #endregion

        #region shopping carts

        public ICartRepository Carts
        {
            get
            {
                if (carts == null)
                    carts = new CartRepository(dbContext);
                return carts;
            }
        }

        public ICartItemRepository CartItems
        {
            get
            {
                if (cartItems == null)
                    cartItems = new CartItemRepository(dbContext);
                return cartItems;
            }
        }

        public IPurchaseItemRepository PurchaseItems
        {
            get
            {
                if (purchaseItems == null)
                    purchaseItems = new PurchaseItemRepository(dbContext);
                return purchaseItems;
            }
        }

        #endregion

        #region teams

        public ITeamRepository Teams
        {
            get
            {
                if (teams == null)
                    teams = new TeamRepository(dbContext);
                return teams;
            }
        }

        public ITeamPostRepository TeamPosts
        {
            get { return teamPosts ?? (teamPosts = new TeamPostRepository(dbContext)); }
        }

        #endregion

        #region users

        public IUserRepository Users
        {
            get
            {
                if (users == null)
                    users = new UserRepository(dbContext);
                return users;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (roles == null)
                    roles = new RoleRepository(dbContext);
                return roles;
            }
        }

        public IUser_RoleRepository UserRoles
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Discounts

        public ICartDiscountItemRepository CartDiscountItems
        {
            get
            {
                if (cartDiscountItems == null)
                    cartDiscountItems = new CartDiscountItemRepository(dbContext);
                return cartDiscountItems;
            }
        }

        public ICouponRepository Coupons
        {
            get
            {
                if (coupons == null)
                    coupons = new CouponRepository(dbContext);
                return coupons;
            }
        }

        public IDiscountItemRepository DiscountItems
        {
            get
            {
                if (discountItems == null)
                    discountItems = new DiscountItemRepository(dbContext);
                return discountItems;
            }
        }

        public IRedemptionCodeRepository RedemptionCodes
        {
            get
            {
                if (redemptionCodes == null)
                    redemptionCodes = new RedemptionCodeRepository(dbContext);
                return redemptionCodes;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public RepositoryGroup()
        {
            dbContext = new DB();
        }

        #endregion

        #region Public Methods

        public int SaveChanges()
        {
            try
            {
                return dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); //addthe original exception as the innerException
            }            
        }

        public void Dispose()
        {
            if (events != null)
                events.Dispose();
            if (eventDates != null)
                eventDates.Dispose();
            if (eventWaves != null)
                eventWaves.Dispose();
            if (eventSponsors != null)
                eventSponsors.Dispose();
            if (eventTemplates != null)
                eventTemplates.Dispose();
            if (eventTemplate_PayScales != null)
                eventTemplate_PayScales.Dispose();
            if (eventLeads != null)
                eventLeads.Dispose();
            if (registrations != null)
                registrations.Dispose();
            if (regions != null)
                regions.Dispose();
            if (purchaseItems != null)
                purchaseItems.Dispose();
            if (eventFees != null)
                eventFees.Dispose();
            if (users != null)
                users.Dispose();
            if (roles != null)
                roles.Dispose();
            if (teams != null)
                teams.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
