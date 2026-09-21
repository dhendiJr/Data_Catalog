namespace DataCatalog.Core.Models;

/// <summary>What sort of thing happened to a data source (request 002, Q17).</summary>
public enum CatalogChangeKind
{
    Added,
    FieldChanged,
    Removed,
    PutBack
}
