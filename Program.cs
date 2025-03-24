using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TunaMusicTunes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNpgsql<TunaMusicDbContext>(builder.Configuration["TunaMusicDbConnectionString"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//******************************************************************** POST API ENDPOINTS *********************************************************************

// Create a new song
app.MapPost("api/song", (TunaMusicDbContext db, Song song) =>
{
    if (song.Id == 0)
    {
        // Auto-generate the primary key
        song.Id = db.Song.Any() ? db.Song.Max(s => s.Id) + 1 : 1;
    }

    // Add the song to the database
    db.Song.Add(song);
    db.SaveChanges();

    // Add the song to the Artist songs
    var artist = db.Artist.Include(a => a.Songs).FirstOrDefault(a => a.Id == song.ArtistId);
    if (artist != null)
    {
        artist.Songs.Add(song);
        db.SaveChanges();
    }

    // Add the genres to the Song_Genre table
    if (song.SongGenres != null)
    {
        foreach (var songGenre in song.SongGenres)
        { 
        bool songGenreExists = db.Song_Genres.Any(sg => sg.SongId == song.Id && sg.GenreId == songGenre.GenreId);

            {
                songGenre.SongId = song.Id;
                db.Set<Song_Genre>().Add(songGenre);
            }
        }
        db.SaveChanges();
    }

    return Results.Created($"/api/song/{song.Id}", song);
});

// Create a new artist
app.MapPost("api/artist", (TunaMusicDbContext db, Artist artist) =>
{
    if (artist.Id == 0)
    {
        // Auto-generate the primary key
        artist.Id = db.Artist.Any() ? db.Artist.Max(a => a.Id) + 1 : 1;
    }
    db.Artist.Add(artist);
    db.SaveChanges();
    return Results.Created($"/api/artist/{artist.Id}", artist);

});

// Create a new genre
app.MapPost("api/genre", (TunaMusicDbContext db, Genre genre) =>
{
    if (genre.Id == 0)
    {
        // Auto-generate the primary key
        genre.Id = db.Genres.Any() ? db.Genres.Max(g => g.Id) + 1 : 1;
    }
    db.Genres.Add(genre);
    db.SaveChanges();
    return Results.Created($"/api/genre/{genre.Id}", genre);
});

//******************************************************************** GET API ENDPOINTS *********************************************************************


// Get all songs
app.MapGet("api/song", (TunaMusicDbContext db) =>
{
    var songs = db.Song;
        
    return Results.Ok(songs);
});

// Get a song by ID
app.MapGet("api/song/{id}", (TunaMusicDbContext db, int id) =>
{
    var song = db.Song
        .Include(s => s.Artist)
        .Include(s => s.SongGenres)
        .ThenInclude(sg => sg.Genre)
        .FirstOrDefault(s => s.Id == id);
    if (song == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(song);
});

// Get all artists
app.MapGet("api/artist", (TunaMusicDbContext db) =>
{
    var artists = db.Artist.ToList();
    return Results.Ok(artists);
});

// Get an artist by ID with their songs
app.MapGet("api/artist/{id}", (TunaMusicDbContext db, int id) =>
{
    var artist = db.Artist
        .Include(a => a.Songs)
        .FirstOrDefault(a => a.Id == id);
    if (artist == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(artist);
});

// Get all genres
app.MapGet("api/genre", (TunaMusicDbContext db) =>
{
    var genres = db.Genres.ToList();
    return Results.Ok(genres);
});


//******************************************************************** UPDATE API ENDPOINTS *********************************************************************

// Update a song
app.MapPut("api/song/{id}", (int id, TunaMusicDbContext db, Song updatedSong) =>
{
    var song = db.Song.Include(s => s.SongGenres).FirstOrDefault(s => s.Id == id);
    if (song == null)
    {
        return Results.NotFound();
    }

    song.Title = updatedSong.Title;
    song.Album = updatedSong.Album;
    song.Length = updatedSong.Length;
    song.ArtistId = updatedSong.ArtistId;
    song.Artist = db.Artist.FirstOrDefault(a => a.Id == updatedSong.ArtistId);

    // Update the Artist
    var artist = db.Artist.FirstOrDefault(a => a.Id == updatedSong.ArtistId);
    if (artist != null)
    {
       song.Artist = artist;
        if (!artist.Songs.Any(s => s.Id == song.Id))
        {
            artist.Songs.Add(song);
        }
    }

    // Update the song genres if they do not exist
    if (updatedSong.SongGenres != null)
    {
        foreach (var songGenre in updatedSong.SongGenres)
        {
            if (!song.SongGenres.Any(sg => sg.GenreId == songGenre.GenreId))
            {
                song.SongGenres.Add(songGenre);
            }
        }
    }


    db.SaveChanges();
    return Results.Ok(updatedSong);
});


// Update an artist
app.MapPut("api/artist/{id}", (TunaMusicDbContext db, int id, Artist updatedArtist) =>
{
    var artist = db.Artist.Find(id);
    if (artist == null)
    {
        return Results.NotFound();
    }
    artist.Name = updatedArtist.Name;
    artist.Age = updatedArtist.Age;
    artist.Bio = updatedArtist.Bio;
    db.SaveChanges();
    return Results.Ok(artist);
});

// Update a genre
app.MapPut("api/genre/{id}", (TunaMusicDbContext db, int id, Genre updatedGenre) =>
{
    var genre = db.Genres.Find(id);
    if (genre == null)
    {
        return Results.NotFound();
    }
    genre.Description = updatedGenre.Description;
    db.SaveChanges();
    return Results.Ok(genre);
});

//******************************************************************** DELETE API ENDPOINTS *********************************************************************

// Delete a song
app.MapDelete("api/song/{id}", (TunaMusicDbContext db, int id) =>
{
    var song = db.Song.Find(id);
    if (song == null)
    {
        return Results.NotFound();
    }
    db.Song.Remove(song);
    db.SaveChanges();
    return Results.NoContent();
});

// Delete an artist
app.MapDelete("api/artist/{id}", (TunaMusicDbContext db, int id) =>
{
    var artist = db.Artist.Find(id);
    if (artist == null)
    {
        return Results.NotFound();
    }
    db.Artist.Remove(artist);
    db.SaveChanges();
    return Results.NoContent();
});

// Delete a genre
app.MapDelete("api/genre/{id}", (TunaMusicDbContext db, int id) =>
{
    var genre = db.Genres.Find(id);
    if (genre == null)
    {
        return Results.NotFound();
    }
    db.Genres.Remove(genre);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();

