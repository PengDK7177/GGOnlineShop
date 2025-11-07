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
        // Modified AddToCart action: enforce stock limits and return proper JSON or redirect.
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

            // Load card to check stock
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Card not found." });
                return NotFound();
            }

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CardId == cardId && ci.UserId == userId.Value);

            if (existing != null)
            {
                // Prevent adding beyond available stock
                if (existing.Quantity >= card.CardStock)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Cannot add more than available stock." });

                    TempData["CartMessage"] = "Cannot add more than available stock.";
                    return RedirectToAction(nameof(Index));
                }

                existing.Quantity++;
                _context.Update(existing);
            }
            else
            {
                if (card.CardStock <= 0)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Out of stock." });

                    TempData["CartMessage"] = "Out of stock.";
                    return RedirectToAction(nameof(Index));
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

            // Attempt to create order and decrement stock inside a transaction
            try
            {
                await SaveOrderToDatabaseAsync(cartItems, model, userId.Value);
            }
            catch (InvalidOperationException ex)
            {
                // business validation (e.g. insufficient stock)
                ModelState.AddModelError("", ex.Message);

                // recompute totals and return view
                var existingItems = await _context.CartItems
                    .Include(ci => ci.Card)
                    .Where(ci => ci.UserId == userId.Value)
                    .ToListAsync();

                decimal subtotal = existingItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
                model.TotalAmount = subtotal + 5.5m;
                return View(model);
            }
            catch (Exception ex)
            {
                // unexpected
                ModelState.AddModelError("", "An error occurred while processing the order. Please try again.");
                // log if you have logger (not shown here)
                var existingItems = await _context.CartItems
                    .Include(ci => ci.Card)
                    .Where(ci => ci.UserId == userId.Value)
                    .ToListAsync();

                decimal subtotal = existingItems.Sum(i => (decimal)i.Card.CardPrice * i.Quantity);
                model.TotalAmount = subtotal + 5.5m;
                return View(model);
            }

            // Redirect to payment success
            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        // ---------- 以下是购物车的辅助方法 ----------

        private async Task SaveOrderToDatabaseAsync(List<CartItem> items, CheckoutViewModel model, int userId)
        {
            if (items == null || !items.Any())
                throw new InvalidOperationException("Your cart is empty.");

            // Use a transaction to ensure stock update + cart removal are atomic
            await using var tx = await _context.Database.BeginTransactionAsync();

            // Reload cards with tracking in a single query to minimize concurrency issues
            var cardIds = items.Select(i => i.CardId).Distinct().ToList();
            var cards = await _context.Cards
                .Where(c => cardIds.Contains(c.Id))
                .ToListAsync();

            // Validate stock and decrement
            foreach (var item in items)
            {
                var card = cards.FirstOrDefault(c => c.Id == item.CardId);
                if (card == null)
                    throw new InvalidOperationException($"Card (Id={item.CardId}) no longer exists.");

                if (card.CardStock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for \"{card.PlayerName}\". Available: {card.CardStock}, required: {item.Quantity}.");

                card.CardStock -= item.Quantity;
                _context.Cards.Update(card);
            }

            // TODO: create Order/OrderItem records here if you want order history.
            // For now, persist stock changes and remove cart items.

            await _context.SaveChangesAsync();

            // remove cart items for user (only those processed)
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
        }

        private List<CartItem> GetCartItemsFromSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<CartItem>();
            }

            var cartBytes = session.TryGetValue("Cart", out var bytes) ? bytes : null;
            if (cartBytes == null)
            {
                return new List<CartItem>();
            }

            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartBytes) ?? new List<CartItem>();
        }

        private void ClearCartSession()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("Cart");
        }
    }
}