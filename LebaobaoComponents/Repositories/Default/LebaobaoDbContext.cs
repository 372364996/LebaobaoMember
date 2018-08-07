using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LebaobaoComponents.Domains;

namespace LebaobaoComponents.Repositories.Default
{
    public class LebaobaoDbConfiguration : DbConfiguration
    {
        public LebaobaoDbConfiguration()
        {
            this.SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy(3, TimeSpan.FromSeconds(10)));
        }
    }
    [DbConfigurationType(typeof(LebaobaoDbConfiguration))]
    public class LebaobaoDbContext:DbContext
    {
        public LebaobaoDbContext() : base("LebaobaoDb")
        {
        }

     

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Users> Users { get; set; }
    }
    public class UserMapping : EntityTypeConfiguration<Users>
    {
        public UserMapping()
        {
            ToTable("Users");

            HasMany(u => u.Orders)
                .WithRequired(c => c.User)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);
        }
    }
    public class UserTypeMapping : EntityTypeConfiguration<UserType>
    {
        public UserTypeMapping()
        {
            ToTable("UserTypes");

            HasKey(t =>t.Id)
                .HasMany(t=>t.Users)
                .WithRequired(t=>t.UserType)
                .HasForeignKey(t =>t.UserTypeId)
                .WillCascadeOnDelete(false);
        }
    }
    
}
