using Bunit;
using DataCatalog.Core.Models;
using DataCatalog.Core.Services;
using DataCatalog.Core.Text;
using DataCatalog.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace DataCatalog.Web.ComponentTests;

/// <summary>
/// The pages, without a browser. Checks the wording the Product Owner approved and the
/// behavior that is easy to get quietly wrong - a blank field rendering as a gap, or a
/// removed data source still offering an Edit button.
/// </summary>
public class CatalogPageTests : BunitContext
{
    private readonly StubCatalogService _catalog = new();

    public CatalogPageTests()
    {
        Services.AddMudServices();
        Services.AddSingleton<ICatalogService>(_catalog);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static CatalogEntry AnEntry(int id = 1, string name = "Attendance") => new()
    {
        Id = id,
        Name = name,
        Description = "Daily attendance taken by teachers.",
        SourceSystem = "Student Information System",
        Owner = "Student Services",
        Contact = "Chris Example",
        Sensitivity = "Restricted",
        UpdateFrequency = "Nightly"
    };

    // ---- The list ----------------------------------------------------------

    [Fact]
    public void List_WhenTheCatalogIsEmpty_ShowsTheWordingFromRequest001()
    {
        var page = Render<Home>();

        Assert.Contains(PageText.RunningStatus, page.Markup);
    }

    [Fact]
    public void List_WhenTheCatalogIsEmpty_DoesNotShowATable()
    {
        var page = Render<Home>();

        Assert.Empty(page.FindAll("table"));
    }

    [Fact]
    public void List_ShowsTheFourColumnsTheProductOwnerChose()
    {
        _catalog.Active.Add(AnEntry());

        var page = Render<Home>();

        Assert.Contains(PageText.LabelFor(CatalogField.Name), page.Markup);
        Assert.Contains(PageText.LabelFor(CatalogField.SourceSystem), page.Markup);
        Assert.Contains(PageText.LabelFor(CatalogField.Owner), page.Markup);
        Assert.Contains(PageText.LabelFor(CatalogField.Sensitivity), page.Markup);
    }

    [Fact]
    public void List_DoesNotShowTheDescriptionColumn()
    {
        // Deliberately left off the list (Q4): descriptions are long and would make the
        // table either very tall or scroll sideways.
        _catalog.Active.Add(AnEntry());

        var page = Render<Home>();

        Assert.DoesNotContain("Daily attendance taken by teachers.", page.Markup);
    }

    [Fact]
    public void List_LinksEachDataSourceToItsOwnPage()
    {
        _catalog.Active.Add(AnEntry(id: 7));

        var page = Render<Home>();

        Assert.Contains("/data-sources/7", page.Markup);
    }

    [Fact]
    public void List_ShowsNotRecordedYetRatherThanAnEmptyGap()
    {
        _catalog.Active.Add(new CatalogEntry { Id = 1, Name = "Transport routing" });

        var page = Render<Home>();

        Assert.Contains(PageText.NotRecordedYet, page.Markup);
    }

    [Fact]
    public void List_OffersAddingADataSource()
    {
        var page = Render<Home>();

        Assert.Contains(PageText.AddDataSource, page.Markup);
    }

    // ---- One data source ---------------------------------------------------

    [Fact]
    public void DataSource_ShowsAllSevenFieldLabels()
    {
        _catalog.TheOneBeingLookedAt = AnEntry();

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        foreach (var field in Enum.GetValues<CatalogField>())
        {
            Assert.Contains(PageText.LabelFor(field), page.Markup);
        }
    }

    [Fact]
    public void DataSource_ShowsNotRecordedYetForEachFieldNobodyFilledIn()
    {
        _catalog.TheOneBeingLookedAt = new CatalogEntry { Id = 1, Name = "Transport routing" };

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        Assert.Contains(PageText.NotRecordedYet, page.Markup);
    }

    [Fact]
    public void DataSource_OffersEditAndRemove()
    {
        _catalog.TheOneBeingLookedAt = AnEntry();

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        Assert.Contains(PageText.Edit, page.Markup);
        Assert.Contains(PageText.Remove, page.Markup);
    }

    [Fact]
    public void DataSource_WhenRemoved_SaysSoAndOffersToPutItBack()
    {
        _catalog.TheOneBeingLookedAt = AnEntry();
        _catalog.TheOneBeingLookedAt.IsRemoved = true;

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        Assert.Contains(PageText.RemovedNotice, page.Markup);
        Assert.Contains(PageText.PutBack, page.Markup);
    }

    [Fact]
    public void DataSource_WhenRemoved_DoesNotOfferEditing()
    {
        // Editing a removed data source is not allowed until it is put back (Q16).
        _catalog.TheOneBeingLookedAt = AnEntry();
        _catalog.TheOneBeingLookedAt.IsRemoved = true;

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        Assert.DoesNotContain("/data-sources/1/edit", page.Markup);
    }

    [Fact]
    public void DataSource_WhenNothingHasThatId_SaysItCouldNotBeFound()
    {
        _catalog.TheOneBeingLookedAt = null;

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 99));

        Assert.Contains(PageText.CouldNotBeFound, page.Markup);
    }

    [Fact]
    public void DataSource_ShowsItsHistoryInPlainEnglish()
    {
        _catalog.TheOneBeingLookedAt = AnEntry();
        _catalog.History.Add(new CatalogChange
        {
            Id = 1,
            CatalogEntryId = 1,
            Kind = CatalogChangeKind.FieldChanged,
            Field = CatalogField.Owner,
            OldValue = "IT",
            NewValue = "Student Services",
            RecordedAt = new DateTimeOffset(2026, 9, 21, 13, 42, 0, TimeSpan.Zero)
        });

        var page = Render<DataSourceDetail>(parameters => parameters.Add(p => p.Id, 1));

        Assert.Contains(PageText.History, page.Markup);
        Assert.Contains("Owner changed from", page.Markup);
        Assert.Contains("Student Services", page.Markup);
    }

    // ---- Removed data sources ----------------------------------------------

    [Fact]
    public void RemovedPage_WhenNothingHasBeenRemoved_SaysSo()
    {
        var page = Render<RemovedDataSources>();

        Assert.Contains(PageText.NothingHasBeenRemoved, page.Markup);
    }

    [Fact]
    public void RemovedPage_ListsWhatWasRemovedAndOffersToPutItBack()
    {
        _catalog.Removed.Add(AnEntry(id: 3, name: "Food service"));

        var page = Render<RemovedDataSources>();

        Assert.Contains("Food service", page.Markup);
        Assert.Contains(PageText.PutBack, page.Markup);
    }

    // ---- Adding ------------------------------------------------------------

    [Fact]
    public void AddPage_ShowsABoxForEveryOneOfTheSevenFields()
    {
        var page = Render<AddDataSource>();

        foreach (var field in Enum.GetValues<CatalogField>())
        {
            Assert.Contains(PageText.LabelFor(field), page.Markup);
        }
    }

    [Fact]
    public void AddPage_OffersSaveAndCancel()
    {
        var page = Render<AddDataSource>();

        Assert.Contains(PageText.Save, page.Markup);
        Assert.Contains(PageText.Cancel, page.Markup);
    }
}
