using DataCatalog.Core.Models;

namespace DataCatalog.Core.Abstractions;

/// <summary>
/// Reading and writing catalog entries. Deliberately small: anything a caller does not
/// need does not belong here, and every implementation - including the fakes used in
/// tests - must behave the same way (P22).
/// </summary>
public interface ICatalogRepository
{
    /// <summary>Every entry. Returns an empty list when there are none, never null.</summary>
    Task<IReadOnlyList<CatalogEntry>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>The entry with this id, or null when no entry has it.</summary>
    Task<CatalogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Stores a new entry and returns it with its assigned id.</summary>
    Task<CatalogEntry> AddAsync(CatalogEntry entry, CancellationToken cancellationToken = default);
}
