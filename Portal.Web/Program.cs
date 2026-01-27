using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal.Data.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PortalWebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortalWebContext") ?? throw new InvalidOperationException("Connection string 'PortalWebContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<Portal.Services.Interfaces.IAccountService, Portal.Services.AccountService>();
builder.Services.AddScoped<Portal.Services.Interfaces.ITicketService, Portal.Services.TicketService>();
builder.Services.AddScoped<Portal.Services.Interfaces.IAssetService, Portal.Services.AssetService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Where to send unauthorized users
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Auto-logout after inactivity
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
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Make Login the default page for now

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Portal.Data.Data.PortalWebContext>();

    // Check if Admin exists
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        // Create Default Admin
        // Note: Use the SAME HashPassword logic here manually or extract it
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Admin123!"));
        var hash = Convert.ToBase64String(bytes);

        context.Users.Add(new Portal.Data.Users
        {
            Username = "admin",
            Password = hash,
            Role = "Admin"
        });
        context.SaveChanges();
    }
}

app.Run();
