using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RUNE.Models;
using RUNE.Models.Entities;
using RUNE.Models.ReleaseNotes;

namespace RUNE.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) //Veritabanı bağlatı zinciri
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<ReleaseNote> Notes => Set<ReleaseNote>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<GameImage> GameImages =>  Set<GameImage>();
    public DbSet<GameVideo> GameVideos => Set<GameVideo>();
    public DbSet<GameSystemRequirement> GameSystemRequirements => Set<GameSystemRequirement>();

    #region 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Game>()
            .HasIndex(game => game.Slug)
            .IsUnique();

        // Aynı oyun aynı kullanıcının sepetine iki kez eklenemesin
        modelBuilder.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.GameId })
            .IsUnique();

        modelBuilder.Entity<GameImage>()
            .HasOne(image => image.Game) // Her GameImage kaydının bir tane Game kaydı olsun
            .WithMany(game => game.Images) // Bir Game kaydının birden fazla GameImage kaydı olabilir
            .HasForeignKey(image => image.GameId) // İki tabloyu GameId alanı üzerinden birbirine bağla Bu alan görselin hangi oyuna ait olduğunu belirtir
            .OnDelete(DeleteBehavior.Cascade); // Bir oyun silinirse ona bağlı görseller de otomatik silinsin

        modelBuilder.Entity<GameVideo>()
            .HasOne(video => video.Game)
            .WithMany(game => game.Videos)
            .HasForeignKey(video => video.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Game>()
            .HasOne(game=> game.SystemRequirement)
            .WithOne(requirement => requirement.Game)
            .HasForeignKey<GameSystemRequirement>(requirement => requirement.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    #endregion
}
