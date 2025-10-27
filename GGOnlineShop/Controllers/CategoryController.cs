using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGOnlineShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        // 依赖注入 DbContext
        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // 商品“All”视图 - 从数据库查询所有商品
        public async Task<IActionResult> All()
        {
            var cards = await _dbContext.Cards.ToListAsync();
            return View(cards);
        }

      
     
        // Basketball二级分类页面（含三级分类数据）
        public IActionResult Basketball()
        {
            // 查询Basketball二级分类（Id=2）
            var basketballCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == 2);
            if (basketballCategory == null)
            {
                return NotFound();
            }

            // 查询Basketball下的三级分类（NBA、FIBA、CBA）
            var subCategories = _dbContext.Categories
                .Where(c => c.ParentId == 2)
                .OrderBy(c => c.SortOrder)
                .ToList();

            // 传递二级分类和三级分类数据到视图
            ViewData["SubCategories"] = subCategories;
            // 将父分类作为模型传递给视图
            return View();
        }

        // Football二级分类页面（含三级分类数据）
        public IActionResult Football()
        {
            // 查询Football二级分类（Id=3）
            var footballCategory = _dbContext.Categories.FirstOrDefault(c => c.Id == 3);
            if (footballCategory == null)
            {
                return NotFound();
            }

            // 查询Football下的三级分类（Serie A、La Liga、Premier League）
            var subCategories = _dbContext.Categories
                .Where(c => c.ParentId == 3)
                .OrderBy(c => c.SortOrder)
                .ToList();

            // 传递二级分类和三级分类数据到视图
            ViewData["SubCategories"] = subCategories;
            // 将父分类作为模型传递给视图
            return View(footballCategory);
        }

        // 三级分类页面（示例：NBA页面）
        public IActionResult SubCategory(int id)
        {
            var subNBA = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (subNBA == null)
            {
                return NotFound();
            }
            var cards = _dbContext.Cards
               .Where(c => c.CategoryId == id) // 通过 CategoryId 关联三级分类
               .ToList();
            return View(cards);
        }

        public IActionResult SubSerieA(int id)
        {
            // 1. 查询三级分类（确保分类存在）
            var subSerieA = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (subSerieA == null)
            {
                return NotFound();
            }

            // 2. 查询该三级分类下的所有球星卡（假设 Card 表有 CategoryId 外键关联分类）
            var cards = _dbContext.Cards
                .Where(c => c.CategoryId == id) // 通过 CategoryId 关联三级分类
                .ToList();

            // 3. 将卡片数据传递到视图
            return View(cards);
        }

        // 球星卡详情页面
        public async Task<IActionResult> Details(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(p => p.Id == id);
            if (card == null)
                return NotFound();

            // explicitly render the CardDetails view (existing file Views/Category/CardDetails.cshtml)
            return View("CardDetails", card);
        }

        // 新增商品（可选，用于测试数据录入）
        public IActionResult New()
        {
            return View();
        }




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

        [HttpGet("Category/SubLaLiga")]
        public async Task<IActionResult> SubSerieA() // 方法名改为 SubLaLiga
        {
            // 2. 修改内部逻辑：将查询条件改为"La Liga"（确保数据库中分类名匹配）
            var serieaCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Serie A");

            if (serieaCategory == null)
            {
                return NotFound("意甲分类不存在"); // 提示信息同步修改
            }

            // 获取西甲下的所有子分类（逻辑不变，仅关联的分类ID变化）
            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == serieaCategory.Id)
                .ToListAsync();

            // 3. 视图关联：若使用默认视图名，需创建 SubLaLiga.cshtml 视图
            // （若想复用原有视图，可指定视图名：return View("SubCategory", subCategories);）
            return View(subCategories);
        }
        public async Task<IActionResult> SubLaLiga() // 方法名改为 SubLaLiga
        {
            // 2. 修改内部逻辑：将查询条件改为"La Liga"（确保数据库中分类名匹配）
            var laLigaCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "La Liga");

            if (laLigaCategory == null)
            {
                return NotFound("西甲分类不存在"); // 提示信息同步修改
            }

            // 获取西甲下的所有子分类（逻辑不变，仅关联的分类ID变化）
            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == laLigaCategory.Id)
                .ToListAsync();

            // 3. 视图关联：若使用默认视图名，需创建 SubLaLiga.cshtml 视图
            // （若想复用原有视图，可指定视图名：return View("SubCategory", subCategories);）
            return View(subCategories);
        }
        public async Task<IActionResult> SubPremierLeague() // 方法名改为 SubLaLiga
        {
            // 2. 修改内部逻辑：将查询条件改为"Premier League"（确保数据库中分类名匹配）
            var premierleagueCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Premier League");

            if (premierleagueCategory == null)
            {
                return NotFound("英超分类不存在"); // 提示信息同步修改
            }

            // 获取西甲下的所有子分类（逻辑不变，仅关联的分类ID变化）
            var subCategories = await _dbContext.SubCategories
                .Where(sc => sc.CategoryId == premierleagueCategory.Id)
                .ToListAsync();

            // 3. 视图关联：若使用默认视图名，需创建 SubLaLiga.cshtml 视图
            // （若想复用原有视图，可指定视图名：return View("SubCategory", subCategories);）
            return View(subCategories);
        }
    }
}