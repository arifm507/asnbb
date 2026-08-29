using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using AllamaShibliQuiz.Data;

// Must be set before any Npgsql type initialization to retain DateTime (not DateTimeOffset) mapping.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("ASNBB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Environment variable 'ASNBB_CONNECTION_STRING' is not set.");
builder.Services.AddDbContext<AsnbbDBContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Admin/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();
