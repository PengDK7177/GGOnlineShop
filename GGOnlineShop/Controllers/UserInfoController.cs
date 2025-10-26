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

        // 1. 注册功能
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
                if (await _context.UserInfos.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "用户名已存在");
                    return View(user);
                }

                // Hash the password before saving
                var hasher = new PasswordHasher<UserInfo>();
                user.Password = hasher.HashPassword(user, user.Password);

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

        // 2. 登录功能
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
                ModelState.AddModelError("", "请输入用户名。");
                return View();
            }

            var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("", "无效的用户名或密码。");
                return View();
            }

            var hasher = new PasswordHasher<UserInfo>();
            var verifyResult = hasher.VerifyHashedPassword(user, user.Password, password ?? string.Empty);

            if (verifyResult == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }

            // Fallback for accounts that still store plain-text passwords:
            // if stored password equals the provided plain password, re-hash and accept login.
            if (!string.IsNullOrEmpty(password) && user.Password == password)
            {
                user.Password = hasher.HashPassword(user, password);
                _context.Update(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "无效的用户名或密码。");
            return View();
        }

        // 3. 退出登录
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
            // Load original user first so we can re-show DB values on validation failure
            var originalUser = await _context.UserInfos.FindAsync(user.Id);
            if (originalUser == null)
            {
                return NotFound();
            }

            // Make password optional for profile edits (the Register flow still enforces it)
            ModelState.Remove(nameof(UserInfo.Password));
            ModelState.Remove(nameof(UserInfo.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                // Return the original user from DB so existing AvatarUrl/Username remain populated
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

            // If user supplied a new password, hash it before saving.
            if (!string.IsNullOrEmpty(user.Password))
            {
                var hasher = new PasswordHasher<UserInfo>();
                originalUser.Password = hasher.HashPassword(originalUser, user.Password);
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

            // Ensure AvatarUrl returns a usable path for the client
            var avatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? "/images/default-avatar.jpg" : user.AvatarUrl;
            return Json(new { id = user.Id, username = user.Username, avatarUrl });
        }
    }
}