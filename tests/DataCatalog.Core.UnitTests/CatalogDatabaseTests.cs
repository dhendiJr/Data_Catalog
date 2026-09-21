using DataCatalog.Infrastructure;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// The two decisions the Product Owner made about the catalog file: a missing one is
/// replaced, a damaged one is never touched (request 001, Q10 and Q11).
/// </summary>
public class CatalogDatabaseTests
{
    [Fact]
    public void Prepare_WhenCatalogFileIsMissing_CreatesAnEmptyOne()
    {
        using var catalogFile = new TemporaryCatalogFile();
        Assert.False(catalogFile.Exists);

        using var context = catalogFile.CreateContext();
        CatalogDatabase.Prepare(context, catalogFile.Path);

        Assert.True(catalogFile.Exists);
        Assert.Empty(context.Entries);
    }

    [Fact]
    public void Prepare_WhenCatalogFileIsMissing_DoesNotThrow()
    {
        using var catalogFile = new TemporaryCatalogFile();
        using var context = catalogFile.CreateContext();

        var exception = Record.Exception(() => CatalogDatabase.Prepare(context, catalogFile.Path));

        Assert.Null(exception);
    }

    [Fact]
    public void Prepare_WhenCatalogFileIsDamaged_RefusesToStart()
    {
        using var catalogFile = new TemporaryCatalogFile();
        catalogFile.WriteGarbage("this is not a catalog file"u8.ToArray());

        using var context = catalogFile.CreateContext();

        var damaged = Assert.Throws<CatalogFileDamagedException>(
            () => CatalogDatabase.Prepare(context, catalogFile.Path));

        Assert.Equal(catalogFile.Path, damaged.CatalogFilePath);
    }

    [Fact]
    public void Prepare_WhenCatalogFileIsDamaged_LeavesTheFileExactlyAsItWas()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var originalContents = "this is not a catalog file"u8.ToArray();
        catalogFile.WriteGarbage(originalContents);

        using (var context = catalogFile.CreateContext())
        {
            Assert.Throws<CatalogFileDamagedException>(
                () => CatalogDatabase.Prepare(context, catalogFile.Path));
        }

        catalogFile.ReleaseFileLocks();

        // The whole point of refusing to start: a damaged file may still be
        // recoverable, so not one byte of it may change.
        Assert.True(catalogFile.Exists);
        Assert.Equal(originalContents, catalogFile.ReadRawBytes());
    }

    [Fact]
    public void Prepare_WhenCatalogFileIsEmptyButPresent_IsTreatedAsUsable()
    {
        // A zero-length file is what a half-finished copy leaves behind. SQLite treats
        // it as a new empty database rather than damage, and this test records that.
        using var catalogFile = new TemporaryCatalogFile();
        catalogFile.WriteGarbage([]);

        using var context = catalogFile.CreateContext();
        CatalogDatabase.Prepare(context, catalogFile.Path);

        Assert.Empty(context.Entries);
    }

    [Fact]
    public void Prepare_RunTwiceOnTheSameFile_KeepsWhatIsAlreadyThere()
    {
        using var catalogFile = new TemporaryCatalogFile();

        using (var firstRun = catalogFile.CreateContext())
        {
            CatalogDatabase.Prepare(firstRun, catalogFile.Path);
            firstRun.Entries.Add(new Models.CatalogEntry { Name = "Attendance" });
            firstRun.SaveChanges();
        }

        using var secondRun = catalogFile.CreateContext();
        CatalogDatabase.Prepare(secondRun, catalogFile.Path);

        var entry = Assert.Single(secondRun.Entries);
        Assert.Equal("Attendance", entry.Name);
    }
}
