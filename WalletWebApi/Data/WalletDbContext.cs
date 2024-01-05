using Microsoft.EntityFrameworkCore;
using WalletWebApi.Models.Domain;

namespace WalletWebApi.Data;

public class WalletDbContext: DbContext
{
    
    public WalletDbContext(DbContextOptions<WalletDbContext> dbContextOptions): base(dbContextOptions)
    {
        
    }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<ApplicationUser> Users { get; set; }

    // Your other DbSet properties and configurations

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wallet>()
            .HasOne(w => w.Owner)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // or Cascade if needed
    }
}