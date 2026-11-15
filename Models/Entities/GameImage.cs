using System;

namespace RUNE.Models.Entities;

public class GameImage
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageType { get; set; } = string.Empty;
    public string MediaSource { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
