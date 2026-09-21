using DataCatalog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// The shape of the catalog file. One table, the seven fields of D-010 plus an id.
/// </summary>
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogEntry> Entries => Set<CatalogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entry = modelBuilder.Entity<CatalogEntry>();

        entry.ToTable("CatalogEntries");
        entry.HasKey(e => e.Id);

        // No maximum lengths and no required fields. Validation is a rule for a later
        // request; today the storage accepts whatever it is given so that nothing is
        // silently rejected before the Product Owner has decided the rules (P24).
        entry.Property(e => e.Name);
        entry.Property(e => e.Description);
        entry.Property(e => e.SourceSystem);
        entry.Property(e => e.Owner);
        entry.Property(e => e.Contact);
        entry.Property(e => e.Sensitivity);
        entry.Property(e => e.UpdateFrequency);
    }
}
