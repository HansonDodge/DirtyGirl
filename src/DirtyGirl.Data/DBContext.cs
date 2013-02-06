using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Objects;
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
    }
}
