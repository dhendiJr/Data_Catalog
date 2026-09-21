using DataCatalog.Core;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// Gets the catalog file ready before the app opens, following the two decisions the
/// Product Owner made in request 001:
///
///   Missing file (Q10)  - create a new empty one and carry on. This is also the very
///                         first run, so it has to work anyway.
///   Damaged file (Q11)  - refuse to start, say where the file is, and leave it exactly
///                         as it was. A damaged file may still be recoverable; an
///                         overwritten one never is.
/// </summary>
public static class CatalogDatabase
{
    /// <summary>
    /// Makes sure the catalog file exists and can be read.
    /// </summary>
    /// <exception cref="CatalogFileDamagedException">
    /// The file was already there and could not be read. It has not been touched.
    /// </exception>
    public static void Prepare(CatalogDbContext context, string catalogFilePath)
    {
        var fileExistedBeforeWeTouchedIt = File.Exists(catalogFilePath);

        try
        {
            context.Database.EnsureCreated();

            // Reading from it is the only honest proof it is readable. Creating a
            // connection on its own succeeds even when the file is not a database.
            _ = context.Entries.Count();
        }
        catch (Exception exception) when (fileExistedBeforeWeTouchedIt)
        {
            throw new CatalogFileDamagedException(catalogFilePath, exception);
        }
    }
}
