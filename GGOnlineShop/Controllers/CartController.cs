using System;
using System.Linq;
using System.Threading.Tasks;
using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGOnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fix CS0019: Convert User.Identity.Name (string) to int for comparison with UserId (int)
            if (!int.TryParse(User.Identity?.Name, out int userId))
            {
                return BadRequest("Invalid user identifier.");
            }

            // Fix CS8602: Add null check for User.Identity
            var cartItems = await _context.CartItems
                .Include(ci => ci.Card) // 关联球星卡
                .Where(ci => ci.UserId == userId) // 假设用户已登录，用Username标识
                .ToListAsync();

            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var card = await _context.Cards.FindAsync(id); // 查找球星卡

            if (card == null)
            {
                return NotFound();
            }

            // Fix CS0019: Convert User.Identity.Name (string) to int for comparison with UserId (int)
            if (!int.TryParse(User.Identity?.Name, out int userId))
            {
                return BadRequest("Invalid user identifier.");
            }

            // Fix CS8602: Add null check for User.Identity
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == id && ci.UserId == userId);

            if (existingItem != null)
            {
                existingItem.Quantity++; // 已有则数量+1
            }
            else
            {
                var cartItem = new CartItem
                {
                    CardId = card.Id,
                    UserId = userId,
                    Quantity = 1
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Fix CS0019: Convert User.Identity.Name (string) to int for comparison with UserId (int)
            if (!int.TryParse(User.Identity?.Name, out int userId))
            {
                return BadRequest("Invalid user identifier.");
            }

            // Fix CS8602: Add null check for User.Identity
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == id && ci.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            // 结算逻辑（可后续扩展）
            return View();
        }
    }
}