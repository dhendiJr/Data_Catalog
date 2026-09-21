using DataCatalog.Core.Models;
using DataCatalog.Core.Services;

namespace DataCatalog.Web.ComponentTests;

/// <summary>
/// A stand-in for the catalog, so a page can be rendered with whatever it needs to show.
/// The pages only ever talk to ICatalogService, which is exactly why they can be tested
/// like this - no database, no browser (docs/ARCHITECTURE.md).
/// </summary>
internal sealed class StubCatalogService : ICatalogService
{
    public List<CatalogEntry> Active { get; } = [];

    public List<CatalogEntry> Removed { get; } = [];

    public List<CatalogChange> History { get; } = [];

    public CatalogEntry? TheOneBeingLookedAt { get; set; }

    public List<int> PutBackWasCalledFor { get; } = [];

    public Task<IReadOnlyList<CatalogEntry>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CatalogEntry>>(Active);

    public Task<IReadOnlyList<CatalogEntry>> GetRemovedAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CatalogEntry>>(Removed);

    public Task<CatalogEntry?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(TheOneBeingLookedAt);

    public Task<IReadOnlyList<CatalogChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CatalogChange>>(History);

    public Task<SaveResult> AddAsync(CatalogEntry entry, bool saveAnyway = false, CancellationToken cancellationToken = default) =>
        Task.FromResult(SaveResult.Saved(entry));

    public Task<SaveResult> UpdateAsync(CatalogEntry entry, bool saveAnyway = false, CancellationToken cancellationToken = default) =>
        Task.FromResult(SaveResult.Saved(entry));

    public Task RemoveAsync(int id, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task PutBackAsync(int id, CancellationToken cancellationToken = default)
    {
        PutBackWasCalledFor.Add(id);
        return Task.CompletedTask;
    }
}
