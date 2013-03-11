using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Objects;
using System.Data.SqlClient;
using DirtyGirl.Models;

namespace DirtyGirl.Data
{
    public class DB: DbContext
    {
        public DB()
            : base("name=DirtyGirlContext")
        {
           Configuration.LazyLoadingEnabled = true;                        
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(r => r.Users).Map(m => { m.MapLeftKey("UserId"); m.MapRightKey("RoleId"); m.ToTable("User_Role"); });
            modelBuilder.Entity<EventFee>().ToTable("EventFee");
            modelBuilder.Entity<CartCharge>().ToTable("Charge");
            modelBuilder.Entity<Coupon>().ToTable("Coupon");
            modelBuilder.Entity<RedemptionCode>().ToTable("RedemptionCode");
        }

        public DbSet<DiscountItem> DiscountItems { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<RedemptionCode> RedemptionCodes { get; set; }

        public DbSet<EventTemplate> EventTemplates { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public virtual ObjectResult<EventDateDetails> SpGetAllEventDateCounts()
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(EventDateDetails).Assembly);
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventDateDetails>("GetAllEventDateCounts");
        }
        public virtual ObjectResult<EventDateDetails> SpGetEventDateCounts(int EventID)
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(EventDateDetails).Assembly);
            var prams = new object[] {new SqlParameter("@EventId", EventID)};

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventDateDetails>("GetAllEventDateCounts", prams);
        }
        public virtual ObjectResult<EventDateCounts> SpGetEventCounts()
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(EventDateCounts).Assembly);
            var prams = new object[] { new SqlParameter("@EventId", 0) };
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventDateCounts>("GetCurrentEventCounts");
        }
        public virtual ObjectResult<EventDateCounts> SpGetEventCounts(int EventID)
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(EventDateCounts).Assembly);
            var prams = new object[] { new SqlParameter("@EventId", EventID) };

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventDateCounts>("GetCurrentEventCounts", prams);
        }

        public virtual ObjectResult<EventDateCounts> SpGetEventCounts(DateTime Eventdate)
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(EventDateCounts).Assembly);
            var pDt = new SqlParameter("EDate", System.Data.SqlDbType.DateTime);
            var pID = new SqlParameter("EID", System.Data.SqlDbType.Int);
            pID.Value = 0;
            pDt.Value = Eventdate;
            
            var prams = new object[2] ;
            prams[0] = pID;
            prams[1] = pDt;

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<EventDateCounts>("GetCurrentEventCounts @EventId = @EID, @EventDate = @EDate", prams);
        }
    }
}
