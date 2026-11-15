using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RUNE.Data;
using RUNE.Models;

namespace RUNE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GamesController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ActionResult> Index()
        {
            var games = await _applicationDbContext.Games.ToListAsync();
            return View(games);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Game game)
        {
            if (ModelState.IsValid)
            {
                await _applicationDbContext.Games.AddAsync(game);
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var game =  await _applicationDbContext.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game)
        {
            var existingGame = await _applicationDbContext.Games.FindAsync(id);
            if(existingGame == null) return NotFound();
            if (ModelState.IsValid)
            {
                existingGame.Title = game.Title;
                existingGame.Price = game.Price;
                existingGame.Description = game.Description;
                existingGame.Developer = game.Developer;
                existingGame.Publisher = game.Publisher;
                existingGame.Slug = game.Slug;
                existingGame.ReleaseDate = game.ReleaseDate;
                existingGame.DisplayOrder = game.DisplayOrder;
                existingGame.IsPublished = game.IsPublished;
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _applicationDbContext.Games.FindAsync(id);
            if(game == null) return NotFound();
            _applicationDbContext.Remove(game);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
