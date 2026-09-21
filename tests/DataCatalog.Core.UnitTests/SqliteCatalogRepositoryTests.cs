using DataCatalog.Core.Abstractions;
using DataCatalog.Core.Models;
using DataCatalog.Infrastructure;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// Writing an entry to the catalog file and reading it back unchanged - the thing
/// request 001 exists to prove, plus the awkward content from the edge case matrix
/// in docs/TESTING_STANDARDS.md.
/// </summary>
public class SqliteCatalogRepositoryTests
{
    private static CatalogEntry AnEntryWithEverySevenFieldsFilled() => new()
    {
        Name = "Attendance",
        Description = "Daily attendance taken by teachers in each classroom.",
        SourceSystem = "Student Information System",
        Owner = "Student Services",
        Contact = "Chris Example",
        Sensitivity = "Restricted",
        UpdateFrequency = "Nightly"
    };

    private static async Task<(ICatalogRepository Repository, CatalogDbContext Context)> ReadyRepositoryAsync(
        TemporaryCatalogFile catalogFile)
    {
        var context = catalogFile.CreateContext();
        CatalogDatabase.Prepare(context, catalogFile.Path);
        await Task.CompletedTask;
        return (new SqliteCatalogRepository(context), context);
    }

    [Fact]
    public async Task AddAsync_ThenReadBack_KeepsAllSevenFieldsExactly()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var saved = await repository.AddAsync(AnEntryWithEverySevenFieldsFilled());
        var readBack = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(readBack);
        Assert.Equal("Attendance", readBack.Name);
        Assert.Equal("Daily attendance taken by teachers in each classroom.", readBack.Description);
        Assert.Equal("Student Information System", readBack.SourceSystem);
        Assert.Equal("Student Services", readBack.Owner);
        Assert.Equal("Chris Example", readBack.Contact);
        Assert.Equal("Restricted", readBack.Sensitivity);
        Assert.Equal("Nightly", readBack.UpdateFrequency);
    }

    [Fact]
    public async Task AddAsync_SurvivesRestarting_SoEntriesAreReallyOnDisk()
    {
        using var catalogFile = new TemporaryCatalogFile();

        using (var firstRun = catalogFile.CreateContext())
        {
            CatalogDatabase.Prepare(firstRun, catalogFile.Path);
            await new SqliteCatalogRepository(firstRun).AddAsync(AnEntryWithEverySevenFieldsFilled());
        }

        using var secondRun = catalogFile.CreateContext();
        CatalogDatabase.Prepare(secondRun, catalogFile.Path);
        var entries = await new SqliteCatalogRepository(secondRun).GetAllAsync();

        var entry = Assert.Single(entries);
        Assert.Equal("Attendance", entry.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenCatalogIsEmpty_ReturnsAnEmptyListNotNull()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var entries = await repository.GetAllAsync();

        Assert.NotNull(entries);
        Assert.Empty(entries);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNothingHasThatId_ReturnsNull()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        Assert.Null(await repository.GetByIdAsync(1));
        Assert.Null(await repository.GetByIdAsync(0));
        Assert.Null(await repository.GetByIdAsync(-1));
        Assert.Null(await repository.GetByIdAsync(int.MaxValue));
    }

    [Fact]
    public async Task AddAsync_WithEveryFieldBlank_IsStoredAsIs()
    {
        // Today nothing is required. Validation is a rule for a later request, and this
        // test records what is true now rather than what someone assumes is true.
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var saved = await repository.AddAsync(new CatalogEntry());
        var readBack = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(readBack);
        Assert.Equal(string.Empty, readBack.Name);
        Assert.Equal(string.Empty, readBack.Sensitivity);
    }

    [Fact]
    public async Task AddAsync_WithWhitespaceOnlyName_KeepsTheWhitespaceRatherThanTrimmingIt()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var saved = await repository.AddAsync(new CatalogEntry { Name = "   \t  " });
        var readBack = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(readBack);
        Assert.Equal("   \t  ", readBack.Name);
    }

    [Fact]
    public async Task AddAsync_WithTenThousandCharacterDescription_StoresAllOfIt()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var veryLongDescription = new string('x', 10_000);

        var saved = await repository.AddAsync(new CatalogEntry { Description = veryLongDescription });
        var readBack = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(readBack);
        Assert.Equal(10_000, readBack.Description.Length);
        Assert.Equal(veryLongDescription, readBack.Description);
    }

    [Theory]
    [InlineData("O'Brien's gradebook")]
    [InlineData("\"Quoted\" source")]
    [InlineData("Café niño über")]
    [InlineData("Transport 🚌 routes")]
    [InlineData("<script>alert('hi')</script>")]
    [InlineData("Robert'); DROP TABLE CatalogEntries;--")]
    [InlineData("line one\nline two\r\nline three")]
    public async Task AddAsync_WithAwkwardText_StoresItLiterallyAndRunsNothing(string awkwardName)
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var saved = await repository.AddAsync(new CatalogEntry { Name = awkwardName });
        var readBack = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(readBack);
        Assert.Equal(awkwardName, readBack.Name);

        // If the text had been treated as a database command rather than as text,
        // the table would be gone and this would fail.
        Assert.Single(await repository.GetAllAsync());
    }

    [Fact]
    public async Task AddAsync_WithTheSameNameTwice_AllowsBoth()
    {
        // Rejecting duplicates is a rule nobody has asked for yet. This test records
        // that today they are allowed, so that changing it later is a deliberate act.
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        await repository.AddAsync(new CatalogEntry { Name = "Attendance" });
        await repository.AddAsync(new CatalogEntry { Name = "Attendance" });

        Assert.Equal(2, (await repository.GetAllAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_GivesEachEntryItsOwnId()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        var first = await repository.AddAsync(new CatalogEntry { Name = "Attendance" });
        var second = await repository.AddAsync(new CatalogEntry { Name = "Transport" });

        Assert.NotEqual(first.Id, second.Id);
        Assert.NotEqual(0, first.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithManyEntries_ReturnsEveryOne()
    {
        using var catalogFile = new TemporaryCatalogFile();
        var (repository, context) = await ReadyRepositoryAsync(catalogFile);
        using var _ = context;

        for (var number = 1; number <= 500; number++)
        {
            await repository.AddAsync(new CatalogEntry { Name = $"Data source {number}" });
        }

        Assert.Equal(500, (await repository.GetAllAsync()).Count);
    }
}
