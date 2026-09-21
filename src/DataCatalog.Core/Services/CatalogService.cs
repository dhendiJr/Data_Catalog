using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Models;

namespace DataCatalog.Core.Services;

/// <summary>
/// The rules of the catalog, in one place, testable without a browser or a database.
/// Every decision about what is allowed lives here - not in a page, not in storage.
/// </summary>
public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _repository;
    private readonly TimeProvider _clock;

    public CatalogService(ICatalogRepository repository, TimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<IReadOnlyList<CatalogEntry>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var all = await _repository.GetAllAsync(cancellationToken);
        return SortedByName(all.Where(entry => !entry.IsRemoved));
    }

    public async Task<IReadOnlyList<CatalogEntry>> GetRemovedAsync(CancellationToken cancellationToken = default)
    {
        var all = await _repository.GetAllAsync(cancellationToken);
        return SortedByName(all.Where(entry => entry.IsRemoved));
    }

    public Task<CatalogEntry?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public async Task<IReadOnlyList<CatalogChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var changes = await _repository.GetChangesAsync(id, cancellationToken);

        // Newest first: what happened most recently is what someone is usually asking about.
        return changes
            .OrderByDescending(change => change.RecordedAt)
            .ThenByDescending(change => change.Id)
            .ToList();
    }

    public async Task<SaveResult> AddAsync(
        CatalogEntry entry,
        bool saveAnyway = false,
        CancellationToken cancellationToken = default)
    {
        if (IsBlank(entry.Name))
        {
            return SaveResult.NameMissing();
        }

        if (!saveAnyway)
        {
            var existing = await FindByNameAsync(entry.Name, exceptId: null, cancellationToken);
            if (existing is not null)
            {
                return SaveResult.DuplicateName(existing);
            }
        }

        var saved = await _repository.AddAsync(entry, cancellationToken);

        await _repository.AddChangesAsync(
            [NewChange(saved.Id, CatalogChangeKind.Added)],
            cancellationToken);

        return SaveResult.Saved(saved);
    }

    public async Task<SaveResult> UpdateAsync(
        CatalogEntry entry,
        bool saveAnyway = false,
        CancellationToken cancellationToken = default)
    {
        if (IsBlank(entry.Name))
        {
            return SaveResult.NameMissing();
        }

        if (!saveAnyway)
        {
            var existing = await FindByNameAsync(entry.Name, exceptId: entry.Id, cancellationToken);
            if (existing is not null)
            {
                return SaveResult.DuplicateName(existing);
            }
        }

        var before = await _repository.GetByIdAsync(entry.Id, cancellationToken)
            ?? throw new InvalidOperationException($"No data source has the id {entry.Id}.");

        var changes = DescribeFieldChanges(before, entry).ToList();

        await _repository.UpdateAsync(entry, cancellationToken);

        if (changes.Count > 0)
        {
            await _repository.AddChangesAsync(changes, cancellationToken);
        }

        return SaveResult.Saved(entry);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default) =>
        await SetRemovedAsync(id, isRemoved: true, CatalogChangeKind.Removed, cancellationToken);

    public async Task PutBackAsync(int id, CancellationToken cancellationToken = default) =>
        await SetRemovedAsync(id, isRemoved: false, CatalogChangeKind.PutBack, cancellationToken);

    private async Task SetRemovedAsync(
        int id,
        bool isRemoved,
        CatalogChangeKind kind,
        CancellationToken cancellationToken)
    {
        var entry = await _repository.GetByIdAsync(id, cancellationToken);

        // Nothing there, or already in the state asked for: do nothing and record nothing,
        // rather than filling the history with events that did not happen.
        if (entry is null || entry.IsRemoved == isRemoved)
        {
            return;
        }

        entry.IsRemoved = isRemoved;

        await _repository.UpdateAsync(entry, cancellationToken);
        await _repository.AddChangesAsync([NewChange(id, kind)], cancellationToken);
    }

    private async Task<CatalogEntry?> FindByNameAsync(
        string name,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var all = await _repository.GetAllAsync(cancellationToken);

        // Only data sources on the list count. Something that was removed is not one the
        // person would find if they went looking, so warning about it would be confusing.
        return all.FirstOrDefault(entry =>
            !entry.IsRemoved &&
            entry.Id != exceptId &&
            string.Equals(entry.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private IEnumerable<CatalogChange> DescribeFieldChanges(CatalogEntry before, CatalogEntry after)
    {
        foreach (var (field, oldValue, newValue) in EveryFieldOf(before, after))
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
            {
                var change = NewChange(after.Id, CatalogChangeKind.FieldChanged);
                change.Field = field;
                change.OldValue = oldValue;
                change.NewValue = newValue;
                yield return change;
            }
        }
    }

    private static IEnumerable<(CatalogField Field, string Old, string New)> EveryFieldOf(
        CatalogEntry before,
        CatalogEntry after) =>
    [
        (CatalogField.Name, before.Name, after.Name),
        (CatalogField.Description, before.Description, after.Description),
        (CatalogField.SourceSystem, before.SourceSystem, after.SourceSystem),
        (CatalogField.Owner, before.Owner, after.Owner),
        (CatalogField.Contact, before.Contact, after.Contact),
        (CatalogField.Sensitivity, before.Sensitivity, after.Sensitivity),
        (CatalogField.UpdateFrequency, before.UpdateFrequency, after.UpdateFrequency)
    ];

    private CatalogChange NewChange(int catalogEntryId, CatalogChangeKind kind) => new()
    {
        CatalogEntryId = catalogEntryId,
        Kind = kind,
        RecordedAt = _clock.GetUtcNow()
    };

    private static IReadOnlyList<CatalogEntry> SortedByName(IEnumerable<CatalogEntry> entries) =>
        entries.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase).ToList();

    /// <summary>A name of only spaces counts as blank (Q6).</summary>
    private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);
}
