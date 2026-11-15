using System;
using RUNE.Models.Entities;

namespace RUNE.Models;
public class Game
{   
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Slug { get; set; } 
    public string Developer { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public string? CoverImagePath { get; set; }
    public string? BannerImagePath { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<Genre> Genre { get; set; } = [];
    public ICollection<Tag> Tag { get; set; } = [];
    public ICollection<GameImage> Images { get; set; } = [];
    public ICollection<GameVideo> Videos { get; set; } = [];
    public GameSystemRequirement? SystemRequirement { get; set; }
}
