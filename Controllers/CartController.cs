using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RUNE.Data;
using RUNE.Models;
using RUNE.Models.Entities;

namespace RUNE.Controllers
{
    [Authorize] // giriş yapmayan birisi sepete erişemez onu giriş sayfasına yönlentir
    public class CartController : Controller
    {
        private readonly ApplicationDbContext  _applicationDbContext;
        private string UserId => (User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        public CartController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _applicationDbContext.CartItems
                .Where(c => c.UserId == UserId)
                .Include(c => c.Game)
                .ToListAsync();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int gameId)
        {
            var gameExists = await _applicationDbContext.Games.AnyAsync(g => g.Id == gameId);
            var inCart = await _applicationDbContext.CartItems.AnyAsync(c => c.UserId == UserId && c.GameId == gameId);

            if (gameExists && !inCart)
            {
                var cartItem = new CartItem
                {
                    UserId = UserId,
                    GameId = gameId
                };
                _applicationDbContext.Add(cartItem);
                await _applicationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int gameId)
        {
            var item = await _applicationDbContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == UserId && c.GameId == gameId);

            if (item != null)
            {
                _applicationDbContext.CartItems.Remove(item);
                await _applicationDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
