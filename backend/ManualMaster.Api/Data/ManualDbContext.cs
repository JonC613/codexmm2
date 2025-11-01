using ManualMaster.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ManualMaster.Api.Data;

public class ManualDbContext : DbContext
{
    public ManualDbContext(DbContextOptions<ManualDbContext> options) : base(options)
    {
    }

    public DbSet<Manual> Manuals => Set<Manual>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<Manual>(entity =>
        {
            entity.ToTable("manuals");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Tags).HasColumnName("tags").HasColumnType("text[]");
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.FileData).HasColumnName("file_data");
            entity.Property(e => e.FileType).HasColumnName("file_type").HasMaxLength(50);
            entity.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(255);
            entity.Property(e => e.SourceUrl).HasColumnName("source_url").HasMaxLength(500);
            entity.Property(e => e.SearchQuery).HasColumnName("search_query").HasMaxLength(255);
            entity.Property(e => e.UploadDate).HasColumnName("upload_date").HasDefaultValueSql("NOW()");
            entity.Property(e => e.Size).HasColumnName("size");
        });
    }
}
