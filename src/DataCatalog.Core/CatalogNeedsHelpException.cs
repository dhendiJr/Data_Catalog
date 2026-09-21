namespace DataCatalog.Core;

/// <summary>
/// The catalog file is readable but in a state this code will not change on its own,
/// because doing so could lose someone's entries. The person gets the explanation and
/// nothing is touched.
/// </summary>
public class CatalogNeedsHelpException : Exception
{
    public CatalogNeedsHelpException(string message) : base(message)
    {
    }
}
