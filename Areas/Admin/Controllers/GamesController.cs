using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RUNE.Data;
using RUNE.Models.Entities;
using RUNE.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> Create()
        {
            var genres = await _applicationDbContext.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();

            ViewBag.Genres = new MultiSelectList(
                genres,
                "Id",
                "Name"
            );

            var tags = await _applicationDbContext.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();

            ViewBag.Tags = new MultiSelectList(
                "Id",
                "Name"
            );
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
            var existingGame = await _applicationDbContext.Games
                .Include(game => game.SystemRequirement)
                .Include(game => game.Genre)
                .Include(game => game.Tag)
                .Include(game => game.Images)
                .Include(game => game.Videos)
                .FirstOrDefaultAsync(game => game.Id == id);
            if (existingGame == null)
            {
                return NotFound();
            }
            await _applicationDbContext.Genres
                .OrderBy(genre => genre.Name)
                .ToListAsync();

            existingGame.SystemRequirement ??= new GameSystemRequirement();
            var genres = await _applicationDbContext.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();

            var tags = await _applicationDbContext.Tags
                .OrderBy(tag => tag.Name)
                .ToListAsync();

            ViewBag.Genres = new MultiSelectList(
                genres,
                "Id",
                "Name",
                existingGame.Genre.Select(g => g.Id)
            );
            ViewBag.Tags = new MultiSelectList(
                tags,
                "Id",
                "Name",
                existingGame.Tag.Select(g => g.Id)
            );
            return View(existingGame);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game, List<int> genreIds, List<int> tagIds)
        {
            var existingGame = await _applicationDbContext.Games
                .Include(g => g.SystemRequirement)
                .Include(g => g.Genre)
                .Include(g => g.Tag)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (existingGame == null) return NotFound();

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

                var selectedGenres = await _applicationDbContext.Genres
                    .Where(genre => genreIds.Contains(genre.Id))
                    .ToListAsync();

                existingGame.Genre.Clear();

                foreach (var genre in selectedGenres)
                {
                    existingGame.Genre.Add(genre);
                }

                // Seçilen etiketleri getir
                var selectedTags = await _applicationDbContext.Tags
                    .Where(tag => tagIds.Contains(tag.Id))
                    .ToListAsync();

                // Oyunun mevcut etiketlerini temizle
                existingGame.Tag.Clear();

                // Yeni seçilen etiketleri ekle
                foreach (var tag in selectedTags)
                {
                    existingGame.Tag.Add(tag);
                }

                if (game.SystemRequirement != null)
                {
                    existingGame.SystemRequirement ??= new GameSystemRequirement
                    {
                        GameId = existingGame.Id
                    };

                    existingGame.SystemRequirement.MinimumOS =
                        game.SystemRequirement.MinimumOS;
                    existingGame.SystemRequirement.MinimumProcessor =
                        game.SystemRequirement.MinimumProcessor;
                    existingGame.SystemRequirement.MinimumMemory =
                        game.SystemRequirement.MinimumMemory;
                    existingGame.SystemRequirement.MinimumGraphics =
                        game.SystemRequirement.MinimumGraphics;
                    existingGame.SystemRequirement.MinimumStorage =
                        game.SystemRequirement.MinimumStorage;
                    existingGame.SystemRequirement.RecommendedOS =
                        game.SystemRequirement.RecommendedOS;
                    existingGame.SystemRequirement.RecommendedProcessor =
                        game.SystemRequirement.RecommendedProcessor;
                    existingGame.SystemRequirement.RecommendedMemory =
                        game.SystemRequirement.RecommendedMemory;
                    existingGame.SystemRequirement.RecommendedGraphics =
                        game.SystemRequirement.RecommendedGraphics;
                    existingGame.SystemRequirement.RecommendedStorage =
                        game.SystemRequirement.RecommendedStorage;
                }

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
            if (game == null) return NotFound();
            _applicationDbContext.Remove(game);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
