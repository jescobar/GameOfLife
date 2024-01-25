using System.Text.Json;
using System.Text.Json.Serialization;
using LifeApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LifeApi.Data;

public class LifeApiDbContext : DbContext
{
    private readonly IConfiguration configuration;
    public DbSet<BoardState> BoardStates { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardGeneration> BoardGenerations { get; set; }

    public string DbPath { get; }

    public LifeApiDbContext()
    {
    }

    public LifeApiDbContext(DbContextOptions<LifeApiDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("LifeApiContext"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardGeneration>()
            .HasIndex(e => new { e.BoardId, e.IsLatest })
            .IsUnique()
            .HasFilter("IsLatest = 1");

        modelBuilder.Entity<Board>()
            .Property(e => e.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<BoardData>(v, (JsonSerializerOptions)null)
            );

        modelBuilder.Entity<BoardGeneration>()
            .Property(e => e.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<BoardData>(v, (JsonSerializerOptions)null)
            );
    }

}