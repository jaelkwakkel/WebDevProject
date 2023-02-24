using Microsoft.EntityFrameworkCore;

namespace Setup.Models.Account;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AccountModel> AccountModels { get; set; }
}