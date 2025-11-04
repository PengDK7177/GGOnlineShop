using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GGOnlineShop.Data;
using System.Threading.Tasks;

namespace GGOnlineShop.Filters
{
    // DI-friendly filter to protect admin routes
    public class AdminAuthFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _db;
        public AdminAuthFilter(AppDbContext db) => _db = db;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var userId = http.Session.GetInt32("UserId");
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "UserInfo", null);
                return;
            }

            // 1) Check session flag first (fast)
            var isAdminSession = http.Session.GetString("IsAdmin");
            if (!string.IsNullOrEmpty(isAdminSession) && isAdminSession == "1")
            {
                await next();
                return;
            }

            // 2) Fallback to DB (and update session if admin)
            var user = await _db.UserInfos.FindAsync(userId.Value);
            if (user == null || !user.IsAdmin)
            {
                context.Result = new ForbidResult();
                return;
            }

            // cache admin flag in session for subsequent requests
            http.Session.SetString("IsAdmin", "1");

            await next();
        }
    }
}
