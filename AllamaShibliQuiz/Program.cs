using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using AllamaShibliQuiz.Data;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AsnbbConnectionString") ??
    throw new InvalidOperationException("Connection string 'AsnbbConnectionString' not found.");
builder.Services.AddDbContext<AsnbbDBContext>(options => options.UseNpgsql(connectionString));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Admin/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
