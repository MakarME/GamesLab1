using System;
using System.Collections.Generic;
using GamesDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace GamesInfrastructure;

public partial class DbgamesContext : DbContext
{
    public DbgamesContext()
    {
    }

    public DbgamesContext(DbContextOptions<DbgamesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-VR9K3M2\\SQLEXPRESS; Database=DBGames; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.Text).HasColumnType("text");

            entity.HasOne(d => d.Game).WithMany(p => p.Comments)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Games1");

            entity.HasOne(d => d.Player).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Players");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();

            entity.HasOne(d => d.Developer).WithMany(p => p.Games)
                .HasForeignKey(d => d.DeveloperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Games_Developers");

            entity.HasMany(d => d.Genres).WithMany(p => p.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "GameGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_GameGenres_Genres"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_GameGenres_Games"),
                    j =>
                    {
                        j.HasKey("GameId", "GenreId");
                        j.ToTable("GameGenres");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Info).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rating1)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("Rating");
            entity.Property(e => e.RatingDate).HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Games");

            entity.HasOne(d => d.Player).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Players");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
