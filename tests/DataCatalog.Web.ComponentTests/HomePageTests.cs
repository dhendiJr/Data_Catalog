using Bunit;
using DataCatalog.Core.Services;
using DataCatalog.Core.Text;
using DataCatalog.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace DataCatalog.Web.ComponentTests;

/// <summary>
/// The only page, tested without a browser. Checks the exact wording the Product Owner
/// approved in request 001, questions 2 and 4.
/// </summary>
public class HomePageTests : BunitContext
{
    public HomePageTests()
    {
        Services.AddMudServices();

        // Since request 002 the front page asks the catalog what to show. These tests
        // still check exactly what they checked before: the wording of request 001.
        Services.AddSingleton<ICatalogService>(new StubCatalogService());

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void HomePage_ShowsTheCatalogName()
    {
        var page = Render<Home>();

        Assert.Contains(PageText.CatalogName, page.Markup);
    }

    [Fact]
    public void HomePage_ShowsTheStatusLineWordForWord()
    {
        var page = Render<Home>();

        Assert.Contains("The catalog is running. No data sources have been added yet.", page.Markup);
    }

    [Fact]
    public void HomePage_PutsTheCatalogNameInATopLevelHeading()
    {
        var page = Render<Home>();

        var heading = page.Find("h1");

        Assert.Equal(PageText.CatalogName, heading.TextContent.Trim());
    }

    [Fact]
    public void HomePage_DoesNotClaimAnythingHasBeenAdded()
    {
        // The page must never imply there are entries when there are none. If a future
        // change makes this page list things, it has to be a deliberate change here too.
        var page = Render<Home>();

        Assert.Contains("No data sources have been added yet", page.Markup);
    }
}
