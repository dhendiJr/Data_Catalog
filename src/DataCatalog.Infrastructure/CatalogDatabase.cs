using DataCatalog.Core;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Infrastructure;

/// <summary>
/// Gets the catalog file ready before the app opens, following the decisions the
/// Product Owner made:
///
///   Missing file (001 Q10)  - create a new empty one and carry on. This is also the
///                             very first run, so it has to work anyway.
///   Damaged file (001 Q11)  - refuse to start, say where the file is, and leave it
///                             exactly as it was.
///   Reshaping   (002 Q18)   - apply any change of shape the file has not had yet, so
///                             a catalog holding real entries can gain a field safely.
/// </summary>
public static class CatalogDatabase
{
    /// <summary>
    /// Makes sure the catalog file exists, has the right shape, and can be read.
    /// </summary>
    /// <exception cref="CatalogFileDamagedException">
    /// The file was already there and could not be read. It has not been touched.
    /// </exception>
    public static void Prepare(CatalogDbContext context, string catalogFilePath)
    {
        var fileExistedBeforeWeTouchedIt = File.Exists(catalogFilePath);

        try
        {
            if (fileExistedBeforeWeTouchedIt)
            {
                UpgradeFileFromBeforeReshapingExisted(context);
            }

            context.Database.Migrate();

            // Reading from it is the only honest proof it is readable. Creating a
            // connection on its own succeeds even when the file is not a database.
            _ = context.Entries.Count();
        }
        catch (CatalogNeedsHelpException)
        {
            // Not damage. Let this through with its own explanation rather than
            // reporting a readable file as unreadable.
            throw;
        }
        catch (Exception exception) when (fileExistedBeforeWeTouchedIt)
        {
            throw new CatalogFileDamagedException(catalogFilePath, exception);
        }
    }

    /// <summary>
    /// Request 001 created the catalog file directly, before reshaping existed. Such a
    /// file has the tables but no record of which reshapings it has had, so migrating it
    /// would fail with "table already exists" - which would look like damage and is not.
    ///
    /// A file in that state cannot contain anything, because request 001 gave nobody any
    /// way to add an entry. So when it is provably empty its old table is dropped and the
    /// correct shape is built. If it somehow holds entries, nothing is touched and the
    /// catalog refuses to start: this code will not be the reason somebody loses data.
    /// </summary>
    private static void UpgradeFileFromBeforeReshapingExisted(CatalogDbContext context)
    {
        if (!TableExists(context, "CatalogEntries") || TableExists(context, "__EFMigrationsHistory"))
        {
            return;
        }

        var entriesInTheOldFile = CountRowsIn(context, "CatalogEntries");

        if (entriesInTheOldFile > 0)
        {
            throw new CatalogNeedsHelpException(
                $"This catalog file was made before the catalog could reshape itself, and it holds " +
                $"{entriesInTheOldFile} data source(s). Nothing has been changed. Someone needs to move " +
                $"those entries across by hand before the catalog can start.");
        }

        context.Database.ExecuteSqlRaw("DROP TABLE CatalogEntries");
    }

    private static bool TableExists(CatalogDbContext context, string tableName)
    {
        var connection = context.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;

        if (wasClosed)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $name";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "$name";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            return Convert.ToInt64(command.ExecuteScalar()) > 0;
        }
        finally
        {
            if (wasClosed)
            {
                connection.Close();
            }
        }
    }

    private static long CountRowsIn(CatalogDbContext context, string tableName)
    {
        var connection = context.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;

        if (wasClosed)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();

            // The table name is not user input - it is a constant from this file.
            command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

            return Convert.ToInt64(command.ExecuteScalar());
        }
        finally
        {
            if (wasClosed)
            {
                connection.Close();
            }
        }
    }
}
