using System.ComponentModel.DataAnnotations;
namespace TunaMusicTunes.Models;

public class Song_Genre
{
    [Key]
    public int Id { get; set; }
    public int SongId { get; set; }
    public int GenreId { get; set; }
}
