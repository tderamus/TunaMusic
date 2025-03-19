using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TunaMusicTunes.Models;

public class Song
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Album { get; set; }
    public int ArtistId { get; set; }
    public int Length { get; set; }

    // Navigation property
    public List<Genre> Genres { get; set; } = new List<Genre>();
}
