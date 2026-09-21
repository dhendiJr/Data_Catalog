namespace DataCatalog.Core;

/// <summary>
/// Thrown when the catalog file exists but cannot be read. The file is never changed
/// or replaced when this happens - the Product Owner chose a visible failure over a
/// silent replacement, because a damaged file may still be recoverable (request 001, Q11).
/// </summary>
public class CatalogFileDamagedException : Exception
{
    public CatalogFileDamagedException(string catalogFilePath, Exception innerException)
        : base($"The catalog file at {catalogFilePath} could not be read.", innerException)
    {
        CatalogFilePath = catalogFilePath;
    }

    /// <summary>Full path of the file, so the person is told exactly which file to keep.</summary>
    public string CatalogFilePath { get; }
}
