using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TunaMusicTunes.Models;

public class Song
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Album { get; set; }
    public int Length { get; set; }
    public int? ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public List<Song_Genre>? SongGenres { get; set; } = new List<Song_Genre>();
}
