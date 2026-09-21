namespace DataCatalog.Core.Models;

/// <summary>
/// One thing that happened to a data source. Kept forever and never trimmed (Q17):
/// a history that quietly deletes itself is worse than none, because it is trusted
/// right up until the moment it has thrown away what was needed.
///
/// There is no "who". The catalog has no logins (D-005), so recording a name would
/// mean inventing one.
/// </summary>
public class CatalogChange
{
    public int Id { get; set; }

    public int CatalogEntryId { get; set; }

    /// <summary>When it happened.</summary>
    public DateTimeOffset RecordedAt { get; set; }

    public CatalogChangeKind Kind { get; set; }

    /// <summary>Which field changed. Only set when <see cref="Kind"/> is FieldChanged.</summary>
    public CatalogField? Field { get; set; }

    /// <summary>What the field held before. Only set when Kind is FieldChanged.</summary>
    public string? OldValue { get; set; }

    /// <summary>What the field holds now. Only set when Kind is FieldChanged.</summary>
    public string? NewValue { get; set; }
}
