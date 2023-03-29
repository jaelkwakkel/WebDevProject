using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Setup.Areas.Identity.Data;
using Setup.Hubs;
using Setup.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "";
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("SetupContextConnection");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("ProductionContextConnection");
}

Console.WriteLine("QQQXXX");
Console.WriteLine(connectionString);

if (args.Contains("--RunMigrations"))
{
    //Run migrations
    DbContextOptionsBuilder<SetupContext> optionsBuilder = new();
    optionsBuilder.UseSqlServer(connectionString);
    SetupContext setupContext = new(optionsBuilder.Options);

    //setupContext.Database.EnsureCreated();
    setupContext.Database.Migrate();
    return;
}

// Add services to the container.
builder.Services.AddControllersWithViews();
//Add Signalr | https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?tabs=visual-studio&WT.mc_id=dotnet-35129-website&view=aspnetcore-7.0
builder.Services.AddSignalR();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

//Get connection string and add DbContext
builder.Services.AddDbContext<SetupContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<SetupUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SetupContext>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<IdentityOptions>(options =>
    {
        //To make testing and development easier, a unique email is only required in production and acceptance
        options.User.RequireUniqueEmail = true;
    });
}

//Fix for error 13?
//builder.Services.AddDataProtection()
//    .SetApplicationName($"jaelscoolegame.hbo-ict.org-{builder.Environment.EnvironmentName}")
//    .PersistKeysToFileSystem(new DirectoryInfo($@"{builder.Environment.ContentRootPath}dp-keys"));

builder.Services.AddDataProtection().DisableAutomaticKeyGeneration(); // .PersistKeysToDbContext<SetupContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 1;

    options.SignIn.RequireConfirmedEmail = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<GameHub>("/gameHub");

app.MapRazorPages();

app.Run();