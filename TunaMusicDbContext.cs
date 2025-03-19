using Microsoft.EntityFrameworkCore;
using TunaMusicTunes.Models;

public class TunaMusicDbContext : DbContext
{
    public TunaMusicDbContext(DbContextOptions<TunaMusicDbContext> context) : base(context)
    {
    }
    public DbSet<Song> Song { get; set; }
    public DbSet<Artist> Artist { get; set; }
    public DbSet<Genre> Genre { get; set; }
    
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

        // Set up the many-to-many relationship between Song and Genre
        modelBuilder.Entity<Song>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Songs)
            .UsingEntity<Dictionary<string, object>>(
                "SongGenre",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .HasConstraintName("FK_SongGenre_Genre"),
                j => j
                    .HasOne<Song>()
                    .WithMany()
                    .HasForeignKey("SongId")
                    .HasConstraintName("FK_SongGenre_Song"),
                j =>
                {
                    j.HasKey("SongId", "GenreId");
                });
    }
}


