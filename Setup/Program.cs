using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Setup.Areas.Identity.Data;
using Setup.Hubs;
using Setup.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=Setup;User Id=SA;Password=QQQTODOQQQ;";
connectionString = builder.Configuration.GetConnectionString("ProductionContextConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
//Add Signalr | https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?tabs=visual-studio&WT.mc_id=dotnet-35129-website&view=aspnetcore-7.0
builder.Services.AddSignalR();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

//Get connection string and add DbContext
builder.Services.AddDbContext<SetupContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<SetupUser>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SetupContext>();

if (!builder.Environment.IsDevelopment())
    builder.Services.Configure<IdentityOptions>(options =>
    {
        //To make testing and development easier, a unique email is only required in production and acceptance
        options.User.RequireUniqueEmail = true;
    });

//Fix for error 13?
//builder.Services.AddDataProtection()
//    .SetApplicationName($"jaelscoolegame.hbo-ict.org-{builder.Environment.EnvironmentName}")
//    .PersistKeysToFileSystem(new DirectoryInfo($@"{builder.Environment.ContentRootPath}dp-keys"));

builder.Services.AddDataProtection().DisableAutomaticKeyGeneration(); // .PersistKeysToDbContext<SetupContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 1;

    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

//if (!app.Environment.IsDevelopment())
//{
app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
//}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
    context.Response.Headers.Add("Content-Security-Policy", /*"default-src 'self'; breaks jquery and bootstrap*/"frame-ancestors 'none';");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.MapHub<GameHub>("/gameHub");

app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always,
        HttpOnly = HttpOnlyPolicy.Always,
        MinimumSameSitePolicy = SameSiteMode.Strict
    });

app.MapRazorPages();

app.Run();