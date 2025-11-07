using System.Text.RegularExpressions;
using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGOnlineShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        // Inject AppDbContext via constructor
        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // "All" view - query all cards from the database
        public async Task<IActionResult> All()
        {
            var cards = await _dbContext.Cards.ToListAsync();
            return View(cards);
        }

        // Basketball secondary category page (includes third-level subcategories)
        public IActionResult Basketball()
        {
            // Find the Basketball parent category (Id = 2)
            var basketballCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == 2);
            if (basketballCategory == null)
            {
                return NotFound();
            }

            // Load third-level categories under Basketball (NBA, FIBA, CBA)
            var subCategories = _dbContext.Categories
                .Where(c => c.ParentId == 2)
                .OrderBy(c => c.SortOrder)
                .ToList();

            // Pass subcategory list to the view
            ViewData["SubCategories"] = subCategories;
            // Return the Basketball view
            return View();
        }

        // Football secondary category page (includes third-level subcategories)
        public IActionResult Football()
        {
            // Find the Football parent category (Id = 3)
            var footballCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == 3);
            if (footballCategory == null)
            {
                return NotFound();
            }

            // Load third-level categories under Football (Serie A, La Liga, Premier League)
            var subCategories = _dbContext.Categories
                .Where(c => c.ParentId == 3)
                .OrderBy(c => c.SortOrder)
                .ToList();

            // Pass subcategory list to the view
            ViewData["SubCategories"] = subCategories;
            // Return the Football view with the parent category as model
            return View(footballCategory);
        }

        // Third-level category page (example: NBA page)
        public async Task<IActionResult> SubCategory(int id)
        {
            var subCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            // Load cards that belong to this subcategory
            var cards = await _dbContext.Cards
                .Where(c => c.CategoryId == id)
                .ToListAsync();

            // Generate a safe CSS class key from the category name (lowercase, hyphen-separated)
            var key = Regex.Replace(subCategory.Name ?? string.Empty, @"[^\w]+", "-").Trim('-').ToLowerInvariant();

            ViewData["SubCategoryName"] = subCategory.Name;
            ViewData["SubCategoryKey"] = string.IsNullOrWhiteSpace(key) ? "subcategory" : key;

            return View(cards);
        }

        // "New" page: show newly listed cards using the SubCategory view
        // GET: /Category/New
        public async Task<IActionResult> New()
        {
            // Get latest cards (most recent first). Adjust Take() as desired.
            var cards = await _dbContext.Cards
                .OrderByDescending(c => c.CreateTime)
                .Take(24)
                .ToListAsync();

            ViewData["SubCategoryName"] = "New Arrivals";
            ViewData["SubCategoryKey"] = "new";

            // Reuse the SubCategory view which expects a List<Card>
            return View("SubCategory", cards);
        }

        // Add new card (optional, for test data entry)
        [HttpPost]
        public async Task<IActionResult> New(Card card)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Cards.Add(card);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return View(card);
        }

        // Generic third-level listing by id (kept for compatibility)
        public IActionResult SubSerieA(int id)
        {
            // Find the specified subcategory
            var subSerieA = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (subSerieA == null)
            {
                return NotFound();
            }

            // Load cards for this subcategory
            var cards = _dbContext.Cards
                .Where(c => c.CategoryId == id)
                .ToList();

            return View(cards);
        }

        // Card details page
        public async Task<IActionResult> Details(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(p => p.Id == id);
            if (card == null)
                return NotFound();

            // Explicitly render the CardDetails view
            return View("CardDetails", card);
        }

        // Route for Serie A helper - resolves to Serie A category
        [HttpGet("Category/SubLaLiga")]
        public async Task<IActionResult> SubSerieA()
        {
            // Find Serie A category by name
            var serieaCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Serie A");

            if (serieaCategory == null)
            {
                return NotFound("Serie A category does not exist");
            }

            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == serieaCategory.Id)
                .ToListAsync();

            return View(subCategories);
        }

        // La Liga page - return subcategories for La Liga
        public async Task<IActionResult> SubLaLiga()
        {
            var laLigaCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "La Liga");

            if (laLigaCategory == null)
            {
                return NotFound("La Liga category does not exist");
            }

            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == laLigaCategory.Id)
                .ToListAsync();

            return View(subCategories);
        }

        // Premier League page - return subcategories for Premier League
        public async Task<IActionResult> SubPremierLeague()
        {
            var premierleagueCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Premier League");

            if (premierleagueCategory == null)
            {
                return NotFound("Premier League category does not exist");
            }

            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == premierleagueCategory.Id)
                .ToListAsync();

            return View(subCategories);
        }
    }
}