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

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Not logged in -> redirect to login page
                return RedirectToAction("Login", "UserInfo");
            }

            var cartItems = await _context.CartItems
                .Include(ci => ci.Card)
                .Where(ci => ci.UserId == userId.Value)
                .ToListAsync();

            decimal subtotal = cartItems.Sum(ci => (decimal)ci.Card.CardPrice * ci.Quantity);
            ViewData["Subtotal"] = subtotal;

            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, needLogin = true, message = "Please log in." });
                }
                return RedirectToAction("Login", "UserInfo");
            }

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == cardId && ci.UserId == userId.Value);

            if (existing != null)
            {
                existing.Quantity++;
                _context.Update(existing);
            }
            else
            {
                var card = await _context.Cards.FindAsync(cardId);
                if (card == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Card not found." });
                    }
                    return NotFound();
                }

                var cartItem = new CartItem
                {
                    CardId = card.Id,
                    UserId = userId.Value,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return JSON for AJAX callers (include new quantity if useful)
                var qty = existing != null ? existing.Quantity : 1;
                return Json(new { success = true, message = "Added to cart", quantity = qty });
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Cart/RemoveFromCart/5  (decrease quantity by 1 or remove)
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "UserInfo");
            }

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == id && ci.UserId == userId.Value);

            if (existing != null)
            {
                if (existing.Quantity > 1)
                {
                    existing.Quantity--;
                    _context.Update(existing);
                }
                else
                {
                    _context.CartItems.Remove(existing);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/RemoveFromCart (full remove via form)
        [HttpPost, ActionName("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCartPost(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "UserInfo");
            }

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == id && ci.UserId == userId.Value);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Optional: checkout view
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "UserInfo");
            }

            return View();
        }
    }
    
}