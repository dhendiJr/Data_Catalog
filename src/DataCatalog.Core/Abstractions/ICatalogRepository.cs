using DataCatalog.Core.Models;

namespace DataCatalog.Core.Abstractions;

/// <summary>
/// Reading and writing catalog entries and their history. Deliberately dumb: it holds
/// no rules about what is allowed, no sorting preferences and no idea what "removed"
/// means. Those belong to the service (docs/ARCHITECTURE.md).
/// </summary>
public interface ICatalogRepository
{
    /// <summary>Every entry, removed ones included. Empty list when there are none, never null.</summary>
    Task<IReadOnlyList<CatalogEntry>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>The entry with this id, removed or not, or null when no entry has it.</summary>
    Task<CatalogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Stores a new entry and returns it with its assigned id.</summary>
    Task<CatalogEntry> AddAsync(CatalogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Saves changes to an entry that already exists.</summary>
    Task UpdateAsync(CatalogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Records things that happened to an entry.</summary>
    Task AddChangesAsync(IEnumerable<CatalogChange> changes, CancellationToken cancellationToken = default);

    /// <summary>Everything recorded about one entry. Empty list when nothing is, never null.</summary>
    Task<IReadOnlyList<CatalogChange>> GetChangesAsync(int catalogEntryId, CancellationToken cancellationToken = default);
}
