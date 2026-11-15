using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RUNE.Data;
using RUNE.Models.ViewModels;

namespace RUNE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AdminController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _applicationDbContext.Games
            .AsNoTracking()
            .Include(game => game.Genre)
            .OrderByDescending(game => game.Id)
            .ToListAsync();

        var genres = await _applicationDbContext.Genres
            .AsNoTracking()
            .Include(genre => genre.Games)
            .OrderBy(genre => genre.Name)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            Games = games,
            Genres = genres,
            TotalGames = games.Count,
            PublishedGames = games.Count(game => game.IsPublished),
            TotalGenres = genres.Count
        };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
