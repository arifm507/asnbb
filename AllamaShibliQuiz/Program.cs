using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Render terminates HTTPS at a proxy whose address is not known at build time.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Admin/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddScoped<AppMapper>();

// Blog posts are loaded from Content/Blog Markdown files once and cached in memory.
builder.Services.AddSingleton<AllamaShibliQuiz.Services.IBlogService, AllamaShibliQuiz.Services.BlogService>();

var app = builder.Build();

app.UseForwardedHeaders();

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
