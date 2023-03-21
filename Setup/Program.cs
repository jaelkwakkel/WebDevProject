using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Setup.Areas.Identity.Data;
using Setup.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Add Signalr | https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?tabs=visual-studio&WT.mc_id=dotnet-35129-website&view=aspnetcore-7.0
builder.Services.AddSignalR();

//Get connection string and add DbContext
var connectionString = builder.Configuration.GetConnectionString("SetupContextConnection");
//builder.Services.AddDbContext<SetupContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddDbContext<SetupContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<SetupUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SetupContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 1;
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
;

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.MapHub<GameHub>("/gameHub");

app.MapRazorPages();

app.Run();