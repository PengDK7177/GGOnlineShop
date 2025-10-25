using GGOnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(option=>
{
    option.IdleTimeout = TimeSpan.FromMinutes(1); // 设置会话超时时间
    option.Cookie.HttpOnly = true; // 设置HttpOnly属性，防止客户端脚本访问Cookie
    option.Cookie.IsEssential = true; // 确保Cookie在GDPR合规性下仍然可用
});
builder.Services.AddControllersWithViews();
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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
