using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace RUNE.Models.Entities;

public class GameSystemRequirement
{
    public int Id { get; set; }

        public int GameId { get; set; }

        [ValidateNever]
        public Game Game { get; set; } = null!;

        // minimum gereksinimler
        public string? MinimumOS { get; set; }
        public string? MinimumProcessor { get; set; }
        public string? MinimumMemory { get; set; }
        public string? MinimumGraphics { get; set; }
        public string? MinimumStorage { get; set; }

        // önerilen gereksinimler
        public string? RecommendedOS { get; set; }
        public string? RecommendedProcessor { get; set; }
        public string? RecommendedMemory { get; set; }
        public string? RecommendedGraphics { get; set; }
        public string? RecommendedStorage { get; set; }
}
