using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataCatalog.Infrastructure;

/// <summary>
/// Used only by the dotnet-ef tool when generating a migration. It never runs when the
/// catalog is running, and the file path below is never opened - the tool only needs a
/// context it can inspect.
///
/// This exists rather than letting the tool start the web application, because
/// Program.cs catches every exception to show a plain-English message, and would
/// swallow the signal the tool uses to stop the app before it starts serving.
/// </summary>
public class DesignTimeCatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite("Data Source=design-time-only.db")
            .Options;

        return new CatalogDbContext(options);
    }
}
