using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GGOnlineShop.Controllers
{
    // Admin UI for managing Cards (CRUD). Protect actions with TypeFilter(AdminAuthFilter)
    [TypeFilter(typeof(GGOnlineShop.Filters.AdminAuthFilter))]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // list cards
        public async Task<IActionResult> Cards()
        {
            var cards = await _db.Cards.OrderBy(c => c.PlayerName).ToListAsync();
            return View(cards);
        }

        // create
        public async Task<IActionResult> CreateCard()
        {
            // populate third-level categories (ParentId != null)
            var categories = await _db.Categories
                .Where(c => c.ParentId != null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.SubCategories = new SelectList(categories, "Id", "Name");
            return View(new Card());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Card card, IFormFile? ImageFile)
        {
            // repopulate categories for redisplay in case of error
            var categories = await _db.Categories
                .Where(c => c.ParentId != null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.SubCategories = new SelectList(categories, "Id", "Name", card.CategoryId);

            if (!ModelState.IsValid) return View(card);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads/cards");
                Directory.CreateDirectory(uploadDir);
                using var fs = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                await ImageFile.CopyToAsync(fs);
                card.CardImg = $"/uploads/cards/{fileName}";
            }

            card.CreateTime = DateTime.Now;
            card.UpdateTime = DateTime.Now;
            _db.Cards.Add(card);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Cards));
        }

        // edit
        public async Task<IActionResult> EditCard(int id)
        {
            var card = await _db.Cards.FindAsync(id);
            if (card == null) return NotFound();

            var categories = await _db.Categories
                .Where(c => c.ParentId != null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.SubCategories = new SelectList(categories, "Id", "Name", card.CategoryId);

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard(int id, Card posted, IFormFile? ImageFile)
        {
            var card = await _db.Cards.FindAsync(id);
            if (card == null) return NotFound();

            // repopulate categories for redisplay on error
            var categories = await _db.Categories
                .Where(c => c.ParentId != null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.SubCategories = new SelectList(categories, "Id", "Name", posted.CategoryId);

            if (!ModelState.IsValid) return View(posted);

            card.PlayerName = posted.PlayerName;
            card.Team = posted.Team;
            card.CardYear = posted.CardYear;
            card.CardGrade = posted.CardGrade;
            card.CardPrice = posted.CardPrice;
            card.CardStock = posted.CardStock;
            card.CardDesc = posted.CardDesc;
            card.CategoryId = posted.CategoryId; // ensure category is saved
            card.UpdateTime = DateTime.Now;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads/cards");
                Directory.CreateDirectory(uploadDir);
                using var fs = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                await ImageFile.CopyToAsync(fs);
                card.CardImg = $"/uploads/cards/{fileName}";
            }

            _db.Update(card);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Cards));
        }

        // delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _db.Cards.FindAsync(id);
            if (card != null)
            {
                _db.Cards.Remove(card);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Cards));
        }
    }
}
