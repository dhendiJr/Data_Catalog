using DataCatalog.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// A catalog file in a temporary folder, deleted when the test finishes. Every test
/// gets its own, so tests never see each other's data and can run in any order.
/// </summary>
internal sealed class TemporaryCatalogFile : IDisposable
{
    public TemporaryCatalogFile()
    {
        Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"datacatalog-test-{Guid.NewGuid():N}.db");
    }

    public string Path { get; }

    public bool Exists => File.Exists(Path);

    public CatalogDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite($"Data Source={Path}")
            .Options;

        return new CatalogDbContext(options);
    }

    public void WriteGarbage(byte[] contents) => File.WriteAllBytes(Path, contents);

    public byte[] ReadRawBytes() => File.ReadAllBytes(Path);

    /// <summary>
    /// SQLite pools its connections, so the file stays locked after the context is
    /// disposed. Tests that read the file directly call this first.
    /// </summary>
    public void ReleaseFileLocks() => SqliteConnection.ClearAllPools();

    public void Dispose()
    {
        // SQLite keeps connections pooled; without this the file is still locked.
        SqliteConnection.ClearAllPools();

        foreach (var path in new[] { Path, Path + "-shm", Path + "-wal" })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
