using System.ComponentModel.DataAnnotations;

namespace TunaMusicTunes.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }
    public string Description { get; set; }
    public List<Song> Songs { get; set; } = new List<Song>();
}
