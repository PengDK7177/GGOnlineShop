using GGOnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using GGOnlineShop.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GGOnlineShop.Filters.AdminAuthFilter>();
builder.Services.AddSession(option=>
{
    option.IdleTimeout = TimeSpan.FromMinutes(1); // 设置会话超时时间
    option.Cookie.HttpOnly = true; // 设置HttpOnly属性，防止客户端脚本访问Cookie
    option.Cookie.IsEssential = true; // 确保Cookie在GDPR合规性下仍然可用
});
builder.Services.AddControllersWithViews();
var app = builder.Build();



// --- dev-time seed admin (only for testing) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // create admin if missing
    if (!db.UserInfos.Any(u => u.Username == "admin"))
    {
        db.UserInfos.Add(new UserInfo
        {
            Username = "admin",
            Email = "admin@local.test",
            Password = "admin123!", // test only
            IsAdmin = true,
            AvatarUrl = "/images/default-avatar.jpg"
        });
        db.SaveChanges();
    }
}
// --- end seed ---

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
