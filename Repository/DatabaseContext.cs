using Microsoft.EntityFrameworkCore;
using stok.Repository.Model.TokenManager;
using stok.Repository.Model.UserAccounts;

namespace stok.Repository
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> context) : DbContext(context)
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<UserInformation> UserInformations { get; set; }
        public DbSet<PositionType> PositionTypes { get; set; }
        public DbSet<RefreshTokenManager> RefreshTokenManagers { get; set; }
        public DbSet<ForgotPasswordTokenManager> ForgotPasswordTokenManagers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Setting Primay Key
            modelBuilder.Entity<UserAccount>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<AccountType>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<UserInformation>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<PositionType>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<RefreshTokenManager>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<ForgotPasswordTokenManager>()
                .HasKey(k => k.Id);
            #endregion

            #region Setting Relationships
            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.UserInformation)
                .WithOne(u => u.UserAccount)
                .HasForeignKey<UserAccount>(u => u.UserInformationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.AccountType)
                .WithMany(u => u.UserAccounts)
                .HasForeignKey(u => u.AccountTypeId);

            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.PositionType)
                .WithMany(u => u.UserAccounts)
                .HasForeignKey(u => u.PositionTypeId);

            modelBuilder.Entity<RefreshTokenManager>()
                .HasOne(r => r.UserAccount)
                .WithMany(u => u.RefreshTokenManagers)
                .HasForeignKey(r => r.UserAccountId);

            modelBuilder.Entity<ForgotPasswordTokenManager>()
                .HasOne(f => f.UserAccount)
                .WithMany(u => u.forgotPasswordTokenManagers)
                .HasForeignKey(f => f.UserAccountId);
            #endregion
        }
    }
}
