using Microsoft.EntityFrameworkCore;
using TPSBackend.Models;

namespace TPSBackend.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options): base(options)
    {
        
    }
    
    public DbSet<Atm> Atms { get; set; }
    public DbSet<AtmTransaction> AtmTransactions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<UserBalance> UserBalances { get; set; }
    public DbSet<UserTransaction> UserTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atm>().HasKey(a => a.AtmId);
        
        modelBuilder.Entity<AtmTransaction>().HasKey(at => at.TransactionId);
        modelBuilder.Entity<AtmTransaction>().HasOne(at => at.Atm);
        modelBuilder.Entity<AtmTransaction>().HasOne(at => at.TransactedBy);
        modelBuilder.Entity<AtmTransaction>().HasOne(at => at.UserTransaction);
        
        modelBuilder.Entity<User>().HasKey(u => u.UserId);
        
        modelBuilder.Entity<UserAccount>().HasKey(ua => ua.UserAccountId);
        modelBuilder.Entity<UserAccount>().HasOne(ua => ua.User);
        
        modelBuilder.Entity<UserBalance>().HasKey(ub => ub.UserBalanceId);
        modelBuilder.Entity<UserBalance>().HasOne(ub => ub.UserAccount);
        
        modelBuilder.Entity<UserTransaction>().HasKey(ut => ut.TransactionId);
        modelBuilder.Entity<UserTransaction>().HasOne(ut => ut.User);
        modelBuilder.Entity<UserTransaction>().HasOne(ut => ut.AccountFrom);
        modelBuilder.Entity<UserTransaction>().HasOne(ut => ut.AccountTo);
        modelBuilder.Entity<UserTransaction>().HasOne(ut => ut.TransactedBy);
        
    }
}