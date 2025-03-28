﻿using System.ComponentModel.DataAnnotations;

namespace TunaMusicTunes.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }
    required
    public string Description { get; set; }
    public List<Song_Genre>? SongGenres { get; set; }
}
