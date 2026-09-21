namespace DataCatalog.Core.Text;

/// <summary>
/// Wording the Product Owner chose, kept in one place so the page, the tests and any
/// future screen cannot drift apart (P21). None of this is invented by an agent -
/// every string here was approved word for word in request 001.
/// </summary>
public static class PageText
{
    /// <summary>What the catalog is called, everywhere (request 001, Q2).</summary>
    public const string CatalogName = "Data Catalog";

    /// <summary>The status line on the only page (request 001, Q4).</summary>
    public const string RunningStatus = "The catalog is running. No data sources have been added yet.";
}
