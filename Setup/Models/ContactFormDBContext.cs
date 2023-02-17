using Microsoft.EntityFrameworkCore;

namespace Setup.Models;

public class ContactFormDbContext : DbContext
{
    public ContactFormDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ContactFormModel> ContactFormModels { get; set; }
}