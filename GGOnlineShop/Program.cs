using GGOnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using GGOnlineShop.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GGOnlineShop.Filters.AdminAuthFilter>();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(1); // 设置会话超时时间
    option.Cookie.HttpOnly = true; // 设置HttpOnly属性，防止客户端脚本访问Cookie
    option.Cookie.IsEssential = true; // 确保Cookie在GDPR合规性下仍然可用
});
//  配置 Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // 最低日志级别：Debug（可改为 Information）
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // 框架内部日志只显示警告
    .Enrich.FromLogContext() // 增强日志上下文信息
    .WriteTo.Console() // 输出到控制台
    .WriteTo.File(
        path: "Logs/log-.txt",          // 输出到 Logs 文件夹
        rollingInterval: RollingInterval.Day, // 每天生成一个新日志文件
        retainedFileCountLimit: 7,      // 保留 7 天日志
        restrictedToMinimumLevel: LogEventLevel.Information) // 写入等级
    .CreateLogger();

//  Step 2: 使用 Serilog 替换默认日志系统
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();
var app = builder.Build();


//  Step 3: 使用中间件
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Add services to the container.



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
