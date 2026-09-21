namespace DataCatalog.Core.Models;

/// <summary>
/// One data source in the catalog. The seven fields are fixed by decision D-010;
/// adding an eighth is a request, not a code change made in passing.
/// </summary>
public class CatalogEntry
{
    public int Id { get; set; }

    /// <summary>What people call this data source.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>What it is, in one or two plain sentences.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Which software or platform it lives in. Shown to people as "System".
    /// Named SourceSystem in code because a property called System would shadow
    /// the System namespace inside this class - see this area's README.
    /// </summary>
    public string SourceSystem { get; set; } = string.Empty;

    /// <summary>Which department or role is responsible for it.</summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>Who to ask about it.</summary>
    public string Contact { get; set; } = string.Empty;

    /// <summary>
    /// How careful people need to be with it. Free text until the district's own
    /// levels are settled, which happens when the screen for picking one is built (D-018).
    /// </summary>
    public string Sensitivity { get; set; } = string.Empty;

    /// <summary>How often it changes - live, nightly, weekly, once a year.</summary>
    public string UpdateFrequency { get; set; } = string.Empty;

    /// <summary>
    /// Taken off the list but not destroyed. Removing is always recoverable (request 002,
    /// Q8): with no logins, one bad day could otherwise empty the catalog with no way to
    /// tell what was lost. Nothing in this project ever deletes a catalog entry outright.
    /// </summary>
    public bool IsRemoved { get; set; }
}
