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
    }
    
    #endregion
}
