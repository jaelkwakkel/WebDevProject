using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Setup.Areas.Identity.Data;

public class SetupContext : IdentityDbContext<SetupUser>
{
    private const string ADMIN_ROLE_ID = "c79d3d41-1379-45b1-8f77-aae3bd6042ac";
    private const string ADMIN_USER_ID = "54173ae5-e1fd-461a-960d-c9c666157f45";
    private readonly IPasswordHasher<SetupUser> _passwordHasher;

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
            Id = ADMIN_USER_ID,
            Email = "gamemailservice18+admin@gmail.com",
            NormalizedEmail = "GAMEMAILSERVICE18+ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            UserName = "admin",
            NormalizedUserName = "ADMIN"
        };
        var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        var hash = _passwordHasher.HashPassword(user, password);
        user.PasswordHash = hash;

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        // Seeding an admin role
        IdentityRole roleData = new() { Id = ADMIN_ROLE_ID, Name = "admin" };
        var adminRoleData = builder.Entity<IdentityRole>().HasData(roleData);

        // Seeding relation between user and role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_USER_ID
            }
        );

        // Database Seeding Create an admin user
        builder.Entity<SetupUser>().HasData(user);

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}