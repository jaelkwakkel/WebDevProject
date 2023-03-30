using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Setup.Areas.Identity.Data;

public class SetupContext : IdentityDbContext<SetupUser>
{
    private IPasswordHasher<SetupUser> _passwordHasher;

    public SetupContext(DbContextOptions<SetupContext> options, IPasswordHasher<SetupUser> passwordHasher)
        : base(options)
    {
        _passwordHasher = passwordHasher;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // When removing user, also remove the games corresponding to it
        builder.Entity<SetupUser>().HasMany(b => b.FinishedGames).WithOne().OnDelete(DeleteBehavior.Cascade);

        // Seeding an admin user
        SetupUser user = new()
        {
            Email = "gamemailservice18+test@gmail.com",
            NormalizedEmail = "GAMEMAILSERVICE18+TEST@GMAIL.COM",
            EmailConfirmed = true,
            UserName = "testadmin",
            NormalizedUserName = "TESTADMIN"
        };
        var password = System.Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        var hash = _passwordHasher.HashPassword(user, password);
        user.PasswordHash = hash;

        // Seeding an admin role
        IdentityRole adminRoleData = new() { Name = "admin", NormalizedName = "ADMIN".ToUpper() };
        builder.Entity<IdentityRole>().HasData(adminRoleData);

        // Seeding relation between user and role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleData.Id,
                UserId = user.Id
            }
        ); ;

        // Database Seeding Create an admin user
        builder.Entity<SetupUser>().HasData(user);

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}