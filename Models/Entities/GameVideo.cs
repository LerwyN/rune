using System;

namespace RUNE.Models.Entities;

public class GameVideo
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public string VideoUrl { get; set; } = string.Empty;
    public string VideoType { get; set; } = string.Empty;
    public string MediaSource { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
