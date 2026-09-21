using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// Reads and writes catalog entries and their history in the catalog file. Holds no
/// rules: it does not know what "removed" means, does not sort, and does not decide
/// what is allowed. That is CatalogService's job (docs/ARCHITECTURE.md).
/// </summary>
public class SqliteCatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public SqliteCatalogRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CatalogEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Entries
            .AsNoTracking()
            .OrderBy(entry => entry.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<CatalogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Entries
            .AsNoTracking()
            .FirstOrDefaultAsync(entry => entry.Id == id, cancellationToken);
    }

    public async Task<CatalogEntry> AddAsync(CatalogEntry entry, CancellationToken cancellationToken = default)
    {
        _context.Entries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task UpdateAsync(CatalogEntry entry, CancellationToken cancellationToken = default)
    {
        // Entries are read without tracking, so what arrives here is a detached object.
        _context.Entries.Update(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddChangesAsync(
        IEnumerable<CatalogChange> changes,
        CancellationToken cancellationToken = default)
    {
        _context.Changes.AddRange(changes);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogChange>> GetChangesAsync(
        int catalogEntryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Changes
            .AsNoTracking()
            .Where(change => change.CatalogEntryId == catalogEntryId)
            .ToListAsync(cancellationToken);
    }
}
