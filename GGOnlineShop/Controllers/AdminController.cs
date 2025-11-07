using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GGOnlineShop.Controllers
{
    // Admin UI for managing Cards (CRUD). Protect actions with TypeFilter(AdminAuthFilter)
    [TypeFilter(typeof(GGOnlineShop.Filters.AdminAuthFilter))]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext db, IWebHostEnvironment env, ILogger<AdminController> logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }

        // list cards
        public async Task<IActionResult> Cards()
        {
            var cards = await _db.Cards.OrderBy(c => c.PlayerName).ToListAsync();
            return View(cards);
        }

        // create (GET)
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

        // create (POST) - improved: logs modelstate errors and exceptions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Card card, IFormFile? ImageFile)
        {
            var categories = await _db.Categories
                .Where(c => c.ParentId != null)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.SubCategories = new SelectList(categories, "Id", "Name", card?.CategoryId);

            // basic validation
            if (card == null)
                ModelState.AddModelError("", "Invalid card data.");
            else if (card.CategoryId <= 0)
                ModelState.AddModelError(nameof(card.CategoryId), "Please select a subcategory.");

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    if (kv.Value.Errors?.Count > 0)
                    {
                        foreach (var err in kv.Value.Errors)
                        {
                            _logger.LogWarning("CreateCard model error: Field={Field} Error={Error}", kv.Key, err.ErrorMessage);
                        }
                    }
                }
                return View(card);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                try
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var uploadDir = Path.Combine(_env.WebRootPath, "uploads/cards");
                    Directory.CreateDirectory(uploadDir);
                    using var fs = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                    await ImageFile.CopyToAsync(fs);
                    card.CardImg = $"/uploads/cards/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save uploaded image for CreateCard");
                    ModelState.AddModelError("ImageFile", "Failed to save image. Try again.");
                    return View(card);
                }
            }

            card.CreateTime = DateTime.Now;
            card.UpdateTime = DateTime.Now;

            try
            {
                _db.Cards.Add(card);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Card created Id={Id} Player={Player}", card.Id, card.PlayerName);
                TempData["AdminMessage"] = "Card successfully listed";
                return RedirectToAction(nameof(Cards));
            }
            catch (Exception ex)
            {
                // Log full exception (including inner exceptions and stack trace)
                _logger.LogError(ex, "Failed to create card. Full exception details: {Exception}", ex.ToString());

                // Surface a short safe message for the UI. Also store the concise DB error in TempData for debugging if you need it.
                var baseMsg = ex.GetBaseException().Message;
                TempData["AdminMessage"] = "Failed to create card: " + (baseMsg.Length > 120 ? baseMsg.Substring(0, 120) + "…" : baseMsg);

                // Return to Create view so the toast will show (redirect ensures TempData is preserved)
                return RedirectToAction(nameof(CreateCard));
            }
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
                try
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var uploadDir = Path.Combine(_env.WebRootPath, "uploads/cards");
                    Directory.CreateDirectory(uploadDir);
                    using var fs = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                    await ImageFile.CopyToAsync(fs);
                    card.CardImg = $"/uploads/cards/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save uploaded image for EditCard id={Id}", id);
                    ModelState.AddModelError("ImageFile", "Failed to save image. Try again.");
                    return View(posted);
                }
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
