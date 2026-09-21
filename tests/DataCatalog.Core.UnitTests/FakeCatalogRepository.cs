using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Models;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// Storage that lives in memory, so the rules can be tested without a catalog file.
///
/// It hands back copies rather than the objects it holds, because the real repository
/// reads without tracking and does the same. A fake that behaves differently from the
/// real thing proves nothing about the real thing (docs/CODE_STANDARDS.md, Liskov).
/// </summary>
internal sealed class FakeCatalogRepository : ICatalogRepository
{
    private readonly List<CatalogEntry> _entries = [];
    private readonly List<CatalogChange> _changes = [];

    private int _nextEntryId = 1;
    private int _nextChangeId = 1;

    /// <summary>Set to make the next write fail, for testing what the person sees.</summary>
    public Exception? FailNextWriteWith { get; set; }

    public Task<IReadOnlyList<CatalogEntry>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CatalogEntry>>(_entries.Select(Copy).ToList());

    public Task<CatalogEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var found = _entries.FirstOrDefault(entry => entry.Id == id);
        return Task.FromResult(found is null ? null : Copy(found));
    }

    public Task<CatalogEntry> AddAsync(CatalogEntry entry, CancellationToken cancellationToken = default)
    {
        ThrowIfAskedTo();

        entry.Id = _nextEntryId++;
        _entries.Add(Copy(entry));

        return Task.FromResult(entry);
    }

    public Task UpdateAsync(CatalogEntry entry, CancellationToken cancellationToken = default)
    {
        ThrowIfAskedTo();

        var index = _entries.FindIndex(existing => existing.Id == entry.Id);

        if (index < 0)
        {
            throw new InvalidOperationException($"No data source has the id {entry.Id}.");
        }

        _entries[index] = Copy(entry);

        return Task.CompletedTask;
    }

    public Task AddChangesAsync(IEnumerable<CatalogChange> changes, CancellationToken cancellationToken = default)
    {
        ThrowIfAskedTo();

        foreach (var change in changes)
        {
            change.Id = _nextChangeId++;
            _changes.Add(change);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<CatalogChange>> GetChangesAsync(
        int catalogEntryId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CatalogChange>>(
            _changes.Where(change => change.CatalogEntryId == catalogEntryId).ToList());

    private void ThrowIfAskedTo()
    {
        if (FailNextWriteWith is null)
        {
            return;
        }

        var failure = FailNextWriteWith;
        FailNextWriteWith = null;
        throw failure;
    }

    private static CatalogEntry Copy(CatalogEntry entry) => new()
    {
        Id = entry.Id,
        Name = entry.Name,
        Description = entry.Description,
        SourceSystem = entry.SourceSystem,
        Owner = entry.Owner,
        Contact = entry.Contact,
        Sensitivity = entry.Sensitivity,
        UpdateFrequency = entry.UpdateFrequency,
        IsRemoved = entry.IsRemoved
    };
}

/// <summary>A clock that does not move unless a test moves it.</summary>
internal sealed class FixedClock : TimeProvider
{
    private DateTimeOffset _now;

    public FixedClock(DateTimeOffset now) => _now = now;

    public override DateTimeOffset GetUtcNow() => _now;

    public void Advance(TimeSpan by) => _now = _now.Add(by);
}
