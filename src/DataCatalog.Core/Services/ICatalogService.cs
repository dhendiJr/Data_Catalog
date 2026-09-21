using DataCatalog.Core.Models;

namespace DataCatalog.Core.Services;

/// <summary>
/// Everything the catalog is allowed to do, and the rules about it. The pages ask this;
/// they never reach storage themselves and never decide anything for themselves.
/// </summary>
public interface ICatalogService
{
    /// <summary>The data sources on the list, sorted by name, ignoring case.</summary>
    Task<IReadOnlyList<CatalogEntry>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>The removed data sources, sorted the same way.</summary>
    Task<IReadOnlyList<CatalogEntry>> GetRemovedAsync(CancellationToken cancellationToken = default);

    /// <summary>One data source, removed or not, or null when nothing has that id.</summary>
    Task<CatalogEntry?> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Everything that has happened to one data source, newest first (Q17).</summary>
    Task<IReadOnlyList<CatalogChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a data source. Refuses when the name is blank or only spaces (Q6). Reports a
    /// duplicate name without saving, unless <paramref name="saveAnyway"/> says the person
    /// has seen the warning and chosen to continue (Q7).
    /// </summary>
    Task<SaveResult> AddAsync(CatalogEntry entry, bool saveAnyway = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to a data source, recording every field that changed. Same name rules
    /// as adding, except that an entry never counts as a duplicate of itself.
    /// </summary>
    Task<SaveResult> UpdateAsync(CatalogEntry entry, bool saveAnyway = false, CancellationToken cancellationToken = default);

    /// <summary>Takes a data source off the list. Nothing is destroyed (Q8).</summary>
    Task RemoveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Puts a removed data source back on the list.</summary>
    Task PutBackAsync(int id, CancellationToken cancellationToken = default);
}
