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
    public DbSet<UserTransaction> UserTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atm>()
                .HasKey(a => a.AtmId);

            modelBuilder.Entity<AtmTransaction>()
                .HasKey(at => at.TransactionId);

            modelBuilder.Entity<AtmTransaction>()
                .HasOne(at => at.Atm)
                .WithMany(a => a.AtmTransactions)
                .HasForeignKey(at => at.AtmId);

            modelBuilder.Entity<AtmTransaction>()
                .HasOne(at => at.TransactedBy)
                .WithMany(u => u.AtmTransactions)
                .HasForeignKey(at => at.TransactedById);

            modelBuilder.Entity<AtmTransaction>()
                .HasOne(at => at.UserTransaction)
                .WithMany(ut => ut.AtmTransactions)
                .HasForeignKey(at => at.UserTransactionId)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserAccount>()
                .HasKey(ua => ua.UserAccountId);

            modelBuilder.Entity<UserAccount>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAccounts)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserTransaction>()
                .HasKey(ut => ut.TransactionId);

            modelBuilder.Entity<UserTransaction>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTransactions)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTransaction>()
                .HasOne(ut => ut.AccountFrom)
                .WithMany(ua => ua.UserTransactionsFrom)
                .HasForeignKey(ut => ut.AccountFromId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserTransaction>()
                .HasOne(ut => ut.AccountTo)
                .WithMany(ua => ua.UserTransactionsTo)
                .HasForeignKey(ut => ut.AccountToId)
                .OnDelete(DeleteBehavior.NoAction);
    }
}