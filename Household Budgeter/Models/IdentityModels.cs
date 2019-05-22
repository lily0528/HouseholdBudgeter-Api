using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Household_Budgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Household_Budgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [InverseProperty(nameof(Household.Creator))]
        public virtual List<Household> CreatedHouseholds { get; set; }
  
        [InverseProperty(nameof(Household.JoinedUsers))]
        public virtual List<Household> JoinedHouseholds { get; set; }

        public virtual List<Invitation> Invitations { get; set; }
        public ApplicationUser()
        {
            JoinedHouseholds = new List<Household>();
            CreatedHouseholds = new List<Household>();
            Invitations = new List<Invitation>();

        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Household> Households { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Household>()
                .HasMany(s => s.Categories)
                .WithRequired(p => p.Household)
                .WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}