using Microsoft.EntityFrameworkCore;
using TunaMusicTunes.Models;

public class TunaMusicDbContext : DbContext
{
    public TunaMusicDbContext(DbContextOptions<TunaMusicDbContext> context) : base(context)
    {
    }
    public DbSet<Song> Song { get; set; }
    public DbSet<Artist> Artist { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Song_Genre> Song_Genres { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Auto generate the primary key
        modelBuilder.Entity<Song>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Artist>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Genre>()
            .Property(g => g.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Song_Genre>()
            .Property(sg => sg.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Song>()
            .HasOne(s => s.Artist)
            .WithMany(a => a.Songs)
            .HasForeignKey(s => s.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define the many-to-many relationship between Song and Genre
        modelBuilder.Entity<Song_Genre>()
        .HasKey(sg => new { sg.SongId, sg.GenreId });

        modelBuilder.Entity<Song_Genre>()
            .HasOne(sg => sg.Song)
            .WithMany(s => s.SongGenres)
            .HasForeignKey(sg => sg.SongId);

        modelBuilder.Entity<Song_Genre>()
            .HasOne(sg => sg.Genre)
            .WithMany(g => g.SongGenres)
            .HasForeignKey(sg => sg.GenreId);
    }
}


