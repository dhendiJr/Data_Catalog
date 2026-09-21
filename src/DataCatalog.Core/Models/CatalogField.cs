namespace DataCatalog.Core.Models;

/// <summary>
/// The seven fields of a catalog entry (D-010), as something the history can refer to.
/// Stored rather than the label, so that if the Product Owner ever renames a field the
/// whole history renames with it instead of becoming a mix of old and new wording.
/// </summary>
public enum CatalogField
{
    Name,
    Description,
    SourceSystem,
    Owner,
    Contact,
    Sensitivity,
    UpdateFrequency
}
