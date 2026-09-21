using DataCatalog.Core.Models;
using DataCatalog.Core.Services;

namespace DataCatalog.Core.UnitTests;

/// <summary>
/// The rules of the catalog, tested without a browser and without a catalog file.
/// Every decision the Product Owner made in request 002 has a test here.
/// </summary>
public class CatalogServiceTests
{
    private readonly FakeCatalogRepository _storage = new();
    private readonly FixedClock _clock = new(new DateTimeOffset(2026, 9, 21, 14, 0, 0, TimeSpan.Zero));
    private readonly CatalogService _catalog;

    public CatalogServiceTests()
    {
        _catalog = new CatalogService(_storage, _clock);
    }

    private static CatalogEntry AnEntry(string name = "Attendance") => new()
    {
        Name = name,
        Description = "Daily attendance taken by teachers.",
        SourceSystem = "Student Information System",
        Owner = "Student Services",
        Contact = "Chris Example",
        Sensitivity = "Restricted",
        UpdateFrequency = "Nightly"
    };

    // ---- Adding ------------------------------------------------------------

    [Fact]
    public async Task AddAsync_WithEverythingFilledIn_IsSaved()
    {
        var result = await _catalog.AddAsync(AnEntry());

        Assert.True(result.WasSaved);
        Assert.Equal(SaveProblem.None, result.Problem);
        Assert.Single(await _catalog.GetActiveAsync());
    }

    [Fact]
    public async Task AddAsync_WithOnlyAName_IsSaved()
    {
        // Only the name is required (Q5): a data source somebody knows the name of is
        // worth recording even when nothing else is known yet.
        var result = await _catalog.AddAsync(new CatalogEntry { Name = "Transport routing" });

        Assert.True(result.WasSaved);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("  \t \n ")]
    public async Task AddAsync_WithABlankName_IsRefused(string blankName)
    {
        var result = await _catalog.AddAsync(new CatalogEntry { Name = blankName });

        Assert.False(result.WasSaved);
        Assert.Equal(SaveProblem.NameMissing, result.Problem);
        Assert.Empty(await _catalog.GetActiveAsync());
    }

    [Fact]
    public async Task AddAsync_WithABlankName_RecordsNothingInTheHistory()
    {
        await _catalog.AddAsync(new CatalogEntry { Name = "   " });

        Assert.Empty(await _catalog.GetHistoryAsync(1));
    }

    [Fact]
    public async Task AddAsync_RecordsThatItWasAdded()
    {
        var result = await _catalog.AddAsync(AnEntry());

        var history = await _catalog.GetHistoryAsync(result.Entry!.Id);

        var recorded = Assert.Single(history);
        Assert.Equal(CatalogChangeKind.Added, recorded.Kind);
        Assert.Equal(_clock.GetUtcNow(), recorded.RecordedAt);
    }

    // ---- Duplicate names ---------------------------------------------------

    [Fact]
    public async Task AddAsync_WithANameThatAlreadyExists_ReportsItWithoutSaving()
    {
        var first = await _catalog.AddAsync(AnEntry("Attendance"));

        var second = await _catalog.AddAsync(AnEntry("Attendance"));

        Assert.False(second.WasSaved);
        Assert.Equal(SaveProblem.DuplicateName, second.Problem);
        Assert.Equal(first.Entry!.Id, second.ExistingWithSameName!.Id);
        Assert.Single(await _catalog.GetActiveAsync());
    }

    [Theory]
    [InlineData("ATTENDANCE")]
    [InlineData("attendance")]
    [InlineData("  Attendance  ")]
    public async Task AddAsync_TreatsNamesAsTheSameIgnoringCaseAndSpaces(string nearlyTheSame)
    {
        await _catalog.AddAsync(AnEntry("Attendance"));

        var second = await _catalog.AddAsync(AnEntry(nearlyTheSame));

        Assert.Equal(SaveProblem.DuplicateName, second.Problem);
    }

    [Fact]
    public async Task AddAsync_WhenToldToSaveAnyway_AllowsTheDuplicate()
    {
        // The Product Owner chose warn-but-allow (Q7): a district really can have two
        // things reasonably called the same thing.
        await _catalog.AddAsync(AnEntry("Attendance"));

        var second = await _catalog.AddAsync(AnEntry("Attendance"), saveAnyway: true);

        Assert.True(second.WasSaved);
        Assert.Equal(2, (await _catalog.GetActiveAsync()).Count);
    }

    [Fact]
    public async Task AddAsync_DoesNotCountRemovedDataSourcesAsDuplicates()
    {
        // Something removed is not one the person would find if they went looking, so
        // warning about it would only confuse them.
        var first = await _catalog.AddAsync(AnEntry("Attendance"));
        await _catalog.RemoveAsync(first.Entry!.Id);

        var second = await _catalog.AddAsync(AnEntry("Attendance"));

        Assert.True(second.WasSaved);
    }

    // ---- Editing -----------------------------------------------------------

    [Fact]
    public async Task UpdateAsync_RecordsEveryFieldThatChanged()
    {
        var added = await _catalog.AddAsync(AnEntry());
        var edited = added.Entry!;

        edited.Owner = "Technology Services";
        edited.Contact = "Sam Example";

        _clock.Advance(TimeSpan.FromMinutes(5));
        await _catalog.UpdateAsync(edited);

        var history = await _catalog.GetHistoryAsync(edited.Id);
        var fieldChanges = history.Where(change => change.Kind == CatalogChangeKind.FieldChanged).ToList();

        Assert.Equal(2, fieldChanges.Count);

        var owner = Assert.Single(fieldChanges, change => change.Field == CatalogField.Owner);
        Assert.Equal("Student Services", owner.OldValue);
        Assert.Equal("Technology Services", owner.NewValue);
    }

    [Fact]
    public async Task UpdateAsync_WithNothingChanged_RecordsNothing()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.UpdateAsync(added.Entry!);

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);
        Assert.DoesNotContain(history, change => change.Kind == CatalogChangeKind.FieldChanged);
    }

    [Fact]
    public async Task UpdateAsync_WithABlankName_IsRefusedAndChangesNothing()
    {
        var added = await _catalog.AddAsync(AnEntry());
        var edited = added.Entry!;
        edited.Name = "   ";
        edited.Owner = "Somebody else";

        var result = await _catalog.UpdateAsync(edited);

        Assert.Equal(SaveProblem.NameMissing, result.Problem);

        var stored = await _catalog.GetAsync(added.Entry!.Id);
        Assert.Equal("Attendance", stored!.Name);
        Assert.Equal("Student Services", stored.Owner);
    }

    [Fact]
    public async Task UpdateAsync_DoesNotTreatAnEntryAsADuplicateOfItself()
    {
        var added = await _catalog.AddAsync(AnEntry("Attendance"));
        var edited = added.Entry!;
        edited.Owner = "Technology Services";

        var result = await _catalog.UpdateAsync(edited);

        Assert.True(result.WasSaved);
    }

    [Fact]
    public async Task UpdateAsync_ReportsADuplicateOfADifferentEntry()
    {
        await _catalog.AddAsync(AnEntry("Attendance"));
        var second = await _catalog.AddAsync(AnEntry("Transport"));

        var edited = second.Entry!;
        edited.Name = "Attendance";

        var result = await _catalog.UpdateAsync(edited);

        Assert.Equal(SaveProblem.DuplicateName, result.Problem);
    }

    [Fact]
    public async Task UpdateAsync_RecordsARenameLikeAnyOtherFieldChange()
    {
        var added = await _catalog.AddAsync(AnEntry("Attendance"));
        var edited = added.Entry!;
        edited.Name = "Daily attendance";

        await _catalog.UpdateAsync(edited);

        var history = await _catalog.GetHistoryAsync(edited.Id);
        var rename = Assert.Single(history, change => change.Field == CatalogField.Name);
        Assert.Equal("Attendance", rename.OldValue);
        Assert.Equal("Daily attendance", rename.NewValue);
    }

    // ---- Removing and putting back -----------------------------------------

    [Fact]
    public async Task RemoveAsync_TakesItOffTheListWithoutDestroyingIt()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.RemoveAsync(added.Entry!.Id);

        Assert.Empty(await _catalog.GetActiveAsync());
        Assert.Single(await _catalog.GetRemovedAsync());
        Assert.NotNull(await _catalog.GetAsync(added.Entry!.Id));
    }

    [Fact]
    public async Task RemoveAsync_KeepsEveryFieldSoNothingIsLost()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.RemoveAsync(added.Entry!.Id);

        var stored = await _catalog.GetAsync(added.Entry!.Id);
        Assert.Equal("Daily attendance taken by teachers.", stored!.Description);
        Assert.Equal("Chris Example", stored.Contact);
    }

    [Fact]
    public async Task RemoveAsync_RecordsThatItWasRemoved()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.RemoveAsync(added.Entry!.Id);

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);
        Assert.Contains(history, change => change.Kind == CatalogChangeKind.Removed);
    }

    [Fact]
    public async Task RemoveAsync_TwiceRecordsOnlyOneRemoval()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.RemoveAsync(added.Entry!.Id);
        await _catalog.RemoveAsync(added.Entry!.Id);

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);
        Assert.Single(history, change => change.Kind == CatalogChangeKind.Removed);
    }

    [Fact]
    public async Task RemoveAsync_OnSomethingThatDoesNotExist_DoesNothing()
    {
        await _catalog.RemoveAsync(999);

        Assert.Empty(await _catalog.GetActiveAsync());
        Assert.Empty(await _catalog.GetHistoryAsync(999));
    }

    [Fact]
    public async Task PutBackAsync_ReturnsItToTheList()
    {
        var added = await _catalog.AddAsync(AnEntry());
        await _catalog.RemoveAsync(added.Entry!.Id);

        await _catalog.PutBackAsync(added.Entry!.Id);

        Assert.Single(await _catalog.GetActiveAsync());
        Assert.Empty(await _catalog.GetRemovedAsync());
    }

    [Fact]
    public async Task PutBackAsync_RecordsThatItWasPutBack()
    {
        var added = await _catalog.AddAsync(AnEntry());
        await _catalog.RemoveAsync(added.Entry!.Id);

        await _catalog.PutBackAsync(added.Entry!.Id);

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);
        Assert.Contains(history, change => change.Kind == CatalogChangeKind.PutBack);
    }

    [Fact]
    public async Task PutBackAsync_OnSomethingNotRemoved_RecordsNothing()
    {
        var added = await _catalog.AddAsync(AnEntry());

        await _catalog.PutBackAsync(added.Entry!.Id);

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);
        Assert.DoesNotContain(history, change => change.Kind == CatalogChangeKind.PutBack);
    }

    // ---- Lists and history order -------------------------------------------

    [Fact]
    public async Task GetActiveAsync_WhenNothingHasBeenAdded_IsEmptyNotNull()
    {
        var list = await _catalog.GetActiveAsync();

        Assert.NotNull(list);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetActiveAsync_SortsByNameIgnoringCase()
    {
        await _catalog.AddAsync(AnEntry("transport"));
        await _catalog.AddAsync(AnEntry("Attendance"));
        await _catalog.AddAsync(AnEntry("food service"));

        var names = (await _catalog.GetActiveAsync()).Select(entry => entry.Name).ToList();

        Assert.Equal(["Attendance", "food service", "transport"], names);
    }

    [Fact]
    public async Task GetHistoryAsync_PutsTheNewestFirst()
    {
        var added = await _catalog.AddAsync(AnEntry());

        _clock.Advance(TimeSpan.FromHours(1));
        await _catalog.RemoveAsync(added.Entry!.Id);

        _clock.Advance(TimeSpan.FromHours(1));
        await _catalog.PutBackAsync(added.Entry!.Id);

        var kinds = (await _catalog.GetHistoryAsync(added.Entry!.Id))
            .Select(change => change.Kind)
            .ToList();

        Assert.Equal(
            [CatalogChangeKind.PutBack, CatalogChangeKind.Removed, CatalogChangeKind.Added],
            kinds);
    }

    [Fact]
    public async Task GetHistoryAsync_KeepsEverythingRatherThanTrimming()
    {
        var added = await _catalog.AddAsync(AnEntry());

        for (var change = 1; change <= 60; change++)
        {
            var edited = (await _catalog.GetAsync(added.Entry!.Id))!;
            edited.Owner = $"Owner {change}";
            _clock.Advance(TimeSpan.FromMinutes(1));
            await _catalog.UpdateAsync(edited);
        }

        var history = await _catalog.GetHistoryAsync(added.Entry!.Id);

        Assert.Equal(61, history.Count);
    }

    // ---- Awkward content ---------------------------------------------------

    [Theory]
    [InlineData("O'Brien's gradebook")]
    [InlineData("\"Quoted\" source")]
    [InlineData("Café niño über")]
    [InlineData("Transport 🚌 routes")]
    [InlineData("<script>alert('hi')</script>")]
    [InlineData("Robert'); DROP TABLE CatalogEntries;--")]
    public async Task AddAsync_WithAwkwardText_KeepsItExactly(string awkwardName)
    {
        var result = await _catalog.AddAsync(new CatalogEntry { Name = awkwardName });

        Assert.True(result.WasSaved);

        var stored = await _catalog.GetAsync(result.Entry!.Id);
        Assert.Equal(awkwardName, stored!.Name);
    }

    [Fact]
    public async Task AddAsync_WithATenThousandCharacterDescription_KeepsAllOfIt()
    {
        var veryLong = new string('x', 10_000);

        var result = await _catalog.AddAsync(new CatalogEntry { Name = "Long one", Description = veryLong });

        var stored = await _catalog.GetAsync(result.Entry!.Id);
        Assert.Equal(10_000, stored!.Description.Length);
    }

    // ---- When saving fails --------------------------------------------------

    [Fact]
    public async Task AddAsync_WhenStorageFails_LetsTheFailureThroughSoThePersonIsTold()
    {
        // The page turns this into the message from Q19 and keeps everything on screen.
        // What must never happen is a failure that looks like a success.
        _storage.FailNextWriteWith = new IOException("There is not enough space on the disk.");

        var failure = await Assert.ThrowsAsync<IOException>(() => _catalog.AddAsync(AnEntry()));

        Assert.Equal("There is not enough space on the disk.", failure.Message);
        Assert.Empty(await _catalog.GetActiveAsync());
    }
}
