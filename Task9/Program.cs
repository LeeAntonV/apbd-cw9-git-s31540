using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Task9.Models;
using Task9.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
builder.Services.AddScoped<PasswordHasher<AppUser>>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "Task9.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await SeedAdminUserAsync(app.Services);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedAdminUserAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<AppUser>>();

    const string adminEmail = "admin@example.com";
    const string localDemoPassword = "Admin123!";

    if (await repository.FindByEmailAsync(adminEmail) is not null)
    {
        return;
    }

    var admin = new AppUser
    {
        Email = adminEmail,
        Role = UserRoles.Admin
    };

    admin.PasswordHash = passwordHasher.HashPassword(admin, localDemoPassword);
    await repository.AddUserAsync(admin);
}
