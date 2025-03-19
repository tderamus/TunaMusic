using System.ComponentModel.DataAnnotations;
namespace TunaMusicTunes.Models;

public class Artist
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Bio { get; set; }
}
