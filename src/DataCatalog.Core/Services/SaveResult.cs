using DataCatalog.Core.Models;

namespace DataCatalog.Core.Services;

/// <summary>Why a save did or did not happen. The pages turn this into what people read.</summary>
public enum SaveProblem
{
    None,
    NameMissing,
    DuplicateName
}

/// <summary>
/// The outcome of trying to save a data source. A duplicate name is not a failure the
/// person cannot get past - it carries the entry they may have meant, so the page can
/// offer to show it, and saving anyway is always allowed (Q7).
/// </summary>
public sealed class SaveResult
{
    private SaveResult(SaveProblem problem, CatalogEntry? entry, CatalogEntry? existingWithSameName)
    {
        Problem = problem;
        Entry = entry;
        ExistingWithSameName = existingWithSameName;
    }

    public SaveProblem Problem { get; }

    /// <summary>The saved entry, when it was saved.</summary>
    public CatalogEntry? Entry { get; }

    /// <summary>The entry that already had this name, when that was the problem.</summary>
    public CatalogEntry? ExistingWithSameName { get; }

    public bool WasSaved => Problem == SaveProblem.None;

    public static SaveResult Saved(CatalogEntry entry) => new(SaveProblem.None, entry, null);

    public static SaveResult NameMissing() => new(SaveProblem.NameMissing, null, null);

    public static SaveResult DuplicateName(CatalogEntry existing) =>
        new(SaveProblem.DuplicateName, null, existing);
}
