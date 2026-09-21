using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// Reads and writes catalog entries in the catalog file. Holds no business rules -
/// those belong in services (docs/ARCHITECTURE.md).
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
}
