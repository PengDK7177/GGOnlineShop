using GGOnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using GGOnlineShop.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with SQL Server using the connection string from configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IHttpContextAccessor for access to HttpContext outside controllers (services, filters)
builder.Services.AddHttpContextAccessor();

// Register the admin authorization filter so it can be applied via TypeFilter or DI
builder.Services.AddScoped<GGOnlineShop.Filters.AdminAuthFilter>();

// Configure session options (server-side session store)
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(1); // Session idle timeout (adjust for production)
    option.Cookie.HttpOnly = true;                 // Prevent client-side scripts from accessing the cookie
    option.Cookie.IsEssential = true;              // Ensure cookie is set even when GDPR consent policies apply
});

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Minimum log level: Debug (change to Information in production)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Keep framework logs at Warning+
    .Enrich.FromLogContext() // Enrich logs with contextual information
    .WriteTo.Console() // Write logs to console (useful for development / container stdout)
    .WriteTo.File(
        path: "Logs/log-.txt",                    // Write rolling files under Logs/
        rollingInterval: RollingInterval.Day,     // Create a new file daily
        retainedFileCountLimit: 7,                // Keep 7 days of logs
        restrictedToMinimumLevel: LogEventLevel.Information) // Minimum level for file sink
    .CreateLogger();

// Replace the default Microsoft logger with Serilog
builder.Host.UseSerilog();

// Add MVC (controllers with views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure error handling middleware for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Route to a friendly error page
}

// --- dev-time seed admin (only for testing) --
// During development we ensure an 'admin' account exists so the admin UI can be demonstrated.
// In production you should remove automatic seeding or protect it behind environment checks.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Create admin user if missing (development convenience)
    if (!db.UserInfos.Any(u => u.Username == "admin"))
    {
        db.UserInfos.Add(new UserInfo
        {
            Username = "admin",
            Email = "admin@local.test",
            Password = "admin123!", // test only ¡ª use proper hashing/identity in production
            IsAdmin = true,
            AvatarUrl = "/images/default-avatar.jpg"
        });
        db.SaveChanges();
    }
}
// --- end seed ---

// Configure middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware (must be before Authorization if sessions are used for auth)
app.UseSession();

app.UseAuthorization();

// Default route mapping for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
