using Microsoft.EntityFrameworkCore;
using TunaMusicTunes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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


//******************************************************************** CREATE API ENDPOINTS *********************************************************************

// Create a new song
app.MapPost("api/song", (TunaMusicDbContext db, Song song) =>
{
    if (song.Id == 0)
    {
        // Auto-generate the primary key
        song.Id = db.Song.Any() ? db.Song.Max(s => s.Id) + 1 : 1;
        
    }
    db.Song.Add(song);
    db.SaveChanges();
    return Results.Created($"/api/song/{song.Id}", song);
});

//******************************************************************** GET API ENDPOINTS *********************************************************************


// Get all songs
app.MapGet("api/song", (TunaMusicDbContext db) =>
{
    var songs = db.Song.Include(s => s.Genres).ToList();
    return Results.Ok(songs);
});

// Get a song by ID
app.MapGet("api/song/{id}", (TunaMusicDbContext db, int id) =>
{
    var song = db.Song.Include(s => s.Genres).FirstOrDefault(s => s.Id == id);
    if (song == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(song);
});


//******************************************************************** UPDATE API ENDPOINTS *********************************************************************

// Update a song
app.MapPut("api/song/{id}", (TunaMusicDbContext db, int id, Song updatedSong) =>
{
    var song = db.Song.Find(id);
    if (song == null)
    {
        return Results.NotFound();
    }
    song.Title = updatedSong.Title;
    song.Album = updatedSong.Album;
    song.Length = updatedSong.Length;
    song.ArtistId = updatedSong.ArtistId;
    db.SaveChanges();
    return Results.Ok(song);
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

app.Run();
