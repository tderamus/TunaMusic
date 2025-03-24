using System.ComponentModel.DataAnnotations;
namespace TunaMusicTunes.Models;

public class Artist
{
    [Key]
    public int Id { get; set; }
    required
    public string Name { get; set; }
    public int Age { get; set; }
    public string? Bio { get; set; }
    public List<Song>? Songs { get; set; } = new List<Song>();
}
