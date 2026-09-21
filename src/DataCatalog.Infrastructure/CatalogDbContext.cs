using DataCatalog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// The shape of the catalog file: the data sources, and everything that has happened
/// to them.
/// </summary>
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogEntry> Entries => Set<CatalogEntry>();

    public DbSet<CatalogChange> Changes => Set<CatalogChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entry = modelBuilder.Entity<CatalogEntry>();

        entry.ToTable("CatalogEntries");
        entry.HasKey(e => e.Id);

        // No maximum lengths and nothing required at this level. The one rule that exists -
        // a data source must have a name - is enforced in CatalogService, where it can be
        // tested without a database and where the person gets a sentence rather than a crash.
        entry.Property(e => e.Name);
        entry.Property(e => e.Description);
        entry.Property(e => e.SourceSystem);
        entry.Property(e => e.Owner);
        entry.Property(e => e.Contact);
        entry.Property(e => e.Sensitivity);
        entry.Property(e => e.UpdateFrequency);
        entry.Property(e => e.IsRemoved);

        var change = modelBuilder.Entity<CatalogChange>();

        change.ToTable("CatalogChanges");
        change.HasKey(c => c.Id);
        change.Property(c => c.CatalogEntryId);
        change.Property(c => c.RecordedAt);
        change.Property(c => c.Kind).HasConversion<int>();
        change.Property(c => c.Field).HasConversion<int?>();
        change.Property(c => c.OldValue);
        change.Property(c => c.NewValue);
    }
}
