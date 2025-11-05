using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GGOnlineShop.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserInfoController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // 注册功能
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserInfo user)
        {
            if (ModelState.IsValid)
            {
                // Trim inputs
                user.Username = user.Username?.Trim() ?? string.Empty;
                user.Email = user.Email?.Trim() ?? string.Empty;

                // Check username uniqueness
                if (await _context.UserInfos.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "用户名已存在");
                    return View(user);
                }

                // Check email uniqueness (case-insensitive)
                var emailLower = user.Email.ToLowerInvariant();
                if (await _context.UserInfos.AnyAsync(u => u.Email.ToLower() == emailLower))
                {
                    ModelState.AddModelError("Email", "该邮箱已被注册");
                    return View(user);
                }


                if (user.AvatarFile != null && user.AvatarFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.AvatarFile.FileName);
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/avatars");
                    Directory.CreateDirectory(uploadDir);
                    using (var fileStream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        await user.AvatarFile.CopyToAsync(fileStream);
                    }
                    user.AvatarUrl = $"/uploads/avatars/{fileName}";
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        //  登录功能
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "请输入用户名或邮箱。");
                return View();
            }

            var input = username.Trim();

            // allow login by username OR email (case-insensitive for email)
            var user = await _context.UserInfos
                .FirstOrDefaultAsync(u => u.Username == input || u.Email.ToLower() == input.ToLower());

            if (user == null)
            {
                ModelState.AddModelError("", "无效的用户名或密码。");
                return View();
            }

            bool authenticated = false;

       
            if (!string.IsNullOrEmpty(password) && user.Password == password)
            {
                authenticated = true;
            }
            else
            {
                // Backwards compatibility: if stored value is a hashed password, try verify
                try
                {
                    var hasher = new PasswordHasher<UserInfo>();
                    var verifyResult = hasher.VerifyHashedPassword(user, user.Password, password ?? string.Empty);
                    if (verifyResult == PasswordVerificationResult.Success)
                    {
                        authenticated = true;
                    }
                }
                catch
                {
                    // Ignore format/verification errors; do not authenticate
                }
            }

            if (authenticated)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "1" : "0");

                // If the account is admin, go to admin card management immediately.
                if (user.IsAdmin)
                {
                    return RedirectToAction("Cards", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "无效的用户名或密码。");
            return View();
        }

        //  退出登录
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        // 个人信息编辑 - 显示页面（从Session获取用户ID）
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var userFromDb = await _context.UserInfos.FindAsync(userId);
            if (userFromDb == null)
            {
                return NotFound();
            }
            return View(userFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserInfo user)
        {
            var originalUser = await _context.UserInfos.FindAsync(user.Id);
            if (originalUser == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(UserInfo.Password));
            ModelState.Remove(nameof(UserInfo.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                return View(originalUser);
            }

            if (user.AvatarFile != null && user.AvatarFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.AvatarFile.FileName);
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/avatars");
                Directory.CreateDirectory(uploadDir);
                using (var fileStream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                {
                    await user.AvatarFile.CopyToAsync(fileStream);
                }
                originalUser.AvatarUrl = $"/uploads/avatars/{fileName}";
            }

            // Store new password as plain text (test project)
            if (!string.IsNullOrEmpty(user.Password))
            {
                originalUser.Password = user.Password;
            }

            originalUser.Age = user.Age;
            originalUser.Gender = user.Gender;
            originalUser.FavoriteTeam = user.FavoriteTeam;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // 5. 获取当前用户信息（供Header调用头像）
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Forbid();
            }
            var user = await _context.UserInfos.FindAsync(userId);
            if (user == null)
            {
                return Forbid();
            }

            var avatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? "/images/default-avatar.jpg" : user.AvatarUrl;
            return Json(new { id = user.Id, username = user.Username, avatarUrl });
        }
    }
}