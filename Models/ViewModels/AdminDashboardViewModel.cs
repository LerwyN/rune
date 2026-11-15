using System;
using RUNE.Models;
using RUNE.Models.Entities;

namespace RUNE.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalGames { get; set; }
    public int PublishedGames { get; set; }
    public int TotalGenres { get; set; }
    public List<Game> Games { get; set; } = [];
    public List<Genre> Genres { get; set; } = [];
}
