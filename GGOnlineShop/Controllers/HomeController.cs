using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace GGOnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var cards = await _dbContext.Cards.Include(c => c.Category).ToListAsync();
            return View(cards);
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var cardsQuery = _dbContext.Cards.Include(c => c.Category).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                cardsQuery = cardsQuery.Where(c =>
                    c.PlayerName.Contains(keyword) ||
                    c.Team.Contains(keyword) ||
                    c.Category.Name.Contains(keyword)
                );
            }

            ViewData["Keyword"] = keyword;
            var cards = await cardsQuery.ToListAsync();
            return View(cards);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

