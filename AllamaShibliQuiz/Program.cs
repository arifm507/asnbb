using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using AllamaShibliQuiz;
using AllamaShibliQuiz.Data;

// Must be set before any Npgsql type initialization to retain DateTime (not DateTimeOffset) mapping.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AsnbbConnectionString")
    ?? Environment.GetEnvironmentVariable("ASNBB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Connection string 'AsnbbConnectionString' is not configured.");
builder.Services.AddDbContext<AsnbbDBContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Admin/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddScoped<AppMapper>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AsnbbDBContext>();
    db.Database.Migrate();
}

app.Run();
