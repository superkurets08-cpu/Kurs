using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services;
using KURS_ASP.NET.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IReferenceDataService, ReferenceDataService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    })
    .AddCookie("External")
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = "External";
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-google";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedOnly", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(SecurityRoles.Admin));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

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
