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

        // 商品详情 - 从数据库查询单个商品
        public async Task<IActionResult> Details(int id)
        {
            var card = await _dbContext.Cards.FirstOrDefaultAsync(p => p.Id == id);
            if (card == null)
                return NotFound();
            return View(card);
        }

        // 新增商品（可选，用于测试数据录入）
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Card card)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Cards.Add(card);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return View(card);
        }
    }
}