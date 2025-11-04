using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GGOnlineShop.Data;
using GGOnlineShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace GGOnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Single constructor that receives all required services
        public CartController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

        // GET: /Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "UserInfo");
            }

            // Load cart items from DB for the logged-in user
            var cartItems = await _context.CartItems
                .Include(ci => ci.Card)
                .Where(ci => ci.UserId == userId.Value)
                .ToListAsync();

            decimal subtotal = cartItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
            decimal deliveryCost = 5.5m; // fixed delivery cost
            decimal total = subtotal + deliveryCost;

            var viewModel = new CheckoutViewModel
            {
                TotalAmount = total
            };

            return View(viewModel);
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "UserInfo");
            }

            // Validate model first
            if (!ModelState.IsValid)
            {
                // Recompute totals to redisplay view with correct totals
                var existingItems = await _context.CartItems
                    .Include(ci => ci.Card)
                    .Where(ci => ci.UserId == userId.Value)
                    .ToListAsync();

                decimal subtotal = existingItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
                decimal deliveryCost = 5.5m;
                model.TotalAmount = subtotal + deliveryCost;

                return View(model);
            }

            // Validate credit card info for card payments
            if (model.SelectedPaymentMethod is CheckoutViewModel.PaymentMethod.Mastercard or CheckoutViewModel.PaymentMethod.Visa)
            {
                if (string.IsNullOrEmpty(model.CardNumber) ||
                    string.IsNullOrEmpty(model.CardholderName) ||
                    model.ExpiryMonth < 1 || model.ExpiryMonth > 12 ||
                    model.ExpiryYear < DateTime.Now.Year ||
                    string.IsNullOrEmpty(model.Cvc))
                {
                    ModelState.AddModelError("CardInfo", "信用卡信息不完整或无效");
                    // Recompute totals for view
                    var existingItems = await _context.CartItems
                        .Include(ci => ci.Card)
                        .Where(ci => ci.UserId == userId.Value)
                        .ToListAsync();
                    decimal subtotal = existingItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
                    decimal deliveryCost = 5.5m;
                    model.TotalAmount = subtotal + deliveryCost;
                    return View(model);
                }
            }

            // Load cart items from DB
            var cartItems = await _context.CartItems
                .Include(ci => ci.Card)
                .Where(ci => ci.UserId == userId.Value)
                .ToListAsync();

            // Compute totals (for safety / order record)
            decimal subtotalFinal = cartItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
            decimal deliveryCostFinal = 5.5m;
            decimal totalFinal = subtotalFinal + deliveryCostFinal;

            // Save order to database (implement your own logic here)
            SaveOrderToDatabase(cartItems, model);

            // Clear user's cart in DB
            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            // Also clear any session-based cart (if used)
            _httpContextAccessor.HttpContext?.Session.Remove("Cart");

            // Redirect to payment success
            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        // ---------- 以下是购物车的辅助方法（假设你已有类似逻辑） ----------
        private List<CartItem> GetCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<CartItem>();
            }

            // ISession does not have a generic Get<T> method.
            // You need to use Get/Set with byte[] and serialize/deserialize your objects.
            // For demonstration, let's assume you use JSON serialization for CartItem list.

            var cartBytes = session.TryGetValue("Cart", out var bytes) ? bytes : null;
            if (cartBytes == null)
            {
                return new List<CartItem>();
            }

            // Use System.Text.Json for deserialization
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartBytes) ?? new List<CartItem>();
        }

        private void SaveOrderToDatabase(List<CartItem> items, CheckoutViewModel model)
        {
            // TODO: Implement order creation & persistence.
            // For now this is a placeholder to be replaced by real order saving logic.
        }

        private void ClearCartSession()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("Cart");
        }
    }
}