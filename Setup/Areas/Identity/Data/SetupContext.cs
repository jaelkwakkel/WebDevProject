﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Setup.Areas.Identity.Data;

public class SetupContext : IdentityDbContext<SetupUser>
{
    public SetupContext(DbContextOptions<SetupContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder
        //   .Entity<SetupUser>()
        //   .HasMany<GameFinishData>();
        //builder
        //   .Entity<GameFinishData>()
        //   .HasOne<SetupUser>()
        //   .WithMany(user => user.FinishedGames);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}