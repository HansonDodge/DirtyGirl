using System;
using DirtyGirl.Data.DataInterfaces;

namespace DirtyGirl.Data
{
    public class RepositoryGroup : IRepositoryGroup
    {
        private DB dbContext;

        #region Private members

        private IEventRepository events;       
        private IEventDateRepository eventDates;
        private IEventWaveRepository eventWaves;
        private IEventSponsorRepository eventSponsors;
        private IEventTemplateRepository eventTemplates;
        private IEventTemplate_PayScaleRepository eventTemplate_PayScales;
        private IEventTemplate_PayScaleTypeRepository eventTemplate_PayScaleTypes;
        
        private IRegistrationRepository registrations;
        private IRegistrationFeeRepository registrationFees;
        private IRegistrationTypeRepository registrationTypes;

        private IChangeFeeRepository changeFees;
        private IPurchaseItemRepository purchaseItems;

        private ITeamRepository teams;
        
        private IUserRepository users;        
        private IRoleRepository roles;

        private IRegionRepository regions;        

        #endregion

        public RepositoryGroup()
        {
            dbContext = new DB();
        }

        #region Event Repositories
        
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
        
        #endregion

        #region Event Templates

        public IEventTemplateRepository EventTemplates
        {
            get
            {
                if (eventTemplates == null)
                    eventTemplates = new EventTemplateRepository(dbContext);

                return eventTemplates;
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

        public IEventTemplate_PayScaleTypeRepository EventTemplate_PayScaleTypes
        {
            get
            {
                if (eventTemplate_PayScaleTypes == null)
                    eventTemplate_PayScaleTypes = new EventTemplate_PayScaleTypeRepository(dbContext);

                return eventTemplate_PayScaleTypes;
            }
        }

        #endregion

        #region Fees

        public IRegistrationFeeRepository RegistrationFees
        {
            get
            {
                if (registrationFees == null)
                    registrationFees = new RegistrationFeeRepository(dbContext);

                return registrationFees;
            }
        }

        public IChangeFeeRepository ChangeFees
        {
            get
            {
                if (changeFees == null)
                    changeFees = new ChangeFeeRepository(dbContext);

                return changeFees;
            }
        }

        #endregion

        #region Registration

        public IRegistrationRepository Registrations
        {
            get
            {
                if (registrations == null)
                    registrations = new RegistrationRepository(dbContext);
                return registrations;
            }
        }

        public IRegistrationTypeRepository RegistrationTypes
        {
            get
            {
                if (registrationTypes == null)
                    registrationTypes = new RegistrationTypeRepository(dbContext);

                return registrationTypes;
            }
        }

        #endregion

        #region Cart Items

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

        

        public ITeamRepository Teams
        {
            get
            {
                if (teams == null)
                    teams = new TeamRepository(dbContext);
                return teams;
            }
        }



        public IRegionRepository Regions
        {
            get
            {
                if (regions == null)
                    regions = new RegionRepository(dbContext);
                return regions;                
            }
        }        



        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public void Dispose()
        {
            if (events != null)
                events.Dispose();
            if (users != null)
                users.Dispose();
            if (roles != null)
                events.Dispose();
            if (regions != null)
                events.Dispose();
            if (dbContext != null)
                dbContext.Dispose();           

            GC.SuppressFinalize(this);
        }

    }
    
}
