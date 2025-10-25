using Microsoft.AspNetCore.Mvc;
using GGOnlineShop.Models;
using Microsoft.EntityFrameworkCore;
using GGOnlineShop.Data; // 假设AppDbContext在Data文件夹下

namespace GGOnlineShop.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly AppDbContext _context;

        public UserInfoController(AppDbContext context)
        {
            _context = context;
        }

        // 1. 注册功能
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserInfo user)
        {
            if (ModelState.IsValid)
            {
                // 检查用户名是否已存在
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "用户名已存在");
                    return View(user);
                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login"); // 注册后跳转到登录页
            }
            return View(user);
        }

        // 2. 登录功能
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 实际项目中需加密密码，此处简化演示
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                // 假设密码验证通过（实际需比对加密后密码）
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home"); // 登录后跳转到首页
            }
            ModelState.AddModelError("", "用户名或密码错误");
            return View();
        }

        // 3. 退出登录
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        // 4. 个人信息编辑
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserInfo user)
        {
            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        // 5. 获取当前用户信息（供Header调用头像）
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(null); // 未登录返回null
            }
            var user = await _context.Users.FindAsync(userId);
            return Json(user);
        }
    }
}