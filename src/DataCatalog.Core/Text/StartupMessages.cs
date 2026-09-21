namespace DataCatalog.Core.Text;

/// <summary>
/// What the console window says when the catalog cannot start. Approved word for word
/// at request 001, question 13. Tests check for these words, so they cannot be reworded
/// without a test failing - which is the point.
/// </summary>
public static class StartupMessages
{
    /// <summary>
    /// Shown when something else already holds the address the catalog needs. Starting
    /// the catalog twice by accident lands here too.
    /// </summary>
    public static string AddressAlreadyInUse(string technicalDetail) =>
        $"""
        Data Catalog could not start.

        Another program on this computer is already using the address
        the catalog needs. Close that program, then close this window
        and try again.

        Technical detail for whoever helps you:
        {technicalDetail}
        """;

    /// <summary>
    /// Shown when the catalog file exists but cannot be read. The "do not delete" line
    /// is deliberate: deleting the file is the one action that would lose the entries.
    /// </summary>
    public static string CatalogFileDamaged(string catalogFilePath, string technicalDetail) =>
        $"""
        Data Catalog could not start.

        The catalog file appears to be damaged. It has NOT been changed
        or replaced.

        File location: {catalogFilePath}

        Do not delete this file - someone may be able to recover your
        entries from it.

        Technical detail for whoever helps you:
        {technicalDetail}
        """;

    /// <summary>
    /// Anything else that stops the catalog starting. The person still gets a plain
    /// sentence first and the technical detail underneath, never a bare stack trace.
    /// </summary>
    public static string CouldNotStart(string technicalDetail) =>
        $"""
        Data Catalog could not start.

        Something went wrong before the catalog could open. Close this
        window and try again.

        Technical detail for whoever helps you:
        {technicalDetail}
        """;
}
