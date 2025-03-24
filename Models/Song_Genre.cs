using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TunaMusicTunes.Models;

public class Song_Genre
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
   
    public int SongId { get; set; }
    public Song Song { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
}
