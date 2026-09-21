using DataCatalog.Core.Text;
using Microsoft.Playwright;

namespace DataCatalog.UiTests;

/// <summary>
/// The real catalog, in a real browser, looked at the way a person would look at it.
/// Finds things by what is on the screen rather than by internal identifiers, so these
/// tests survive a MudBlazor version change.
/// </summary>
public class HomePageBrowserTests : IClassFixture<RunningCatalogApp>, IAsyncLifetime
{
    private readonly RunningCatalogApp _catalog;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public HomePageBrowserTests(RunningCatalogApp catalog)
    {
        _catalog = catalog;
    }

    public async Task InitializeAsync()
    {
        // Playwright needs its own copy of a browser. This downloads it the first time
        // and does nothing on later runs.
        var installExitCode = Microsoft.Playwright.Program.Main(["install", "chromium", "--with-deps"]);
        if (installExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Playwright could not install its browser (exit code {installExitCode}).");
        }

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    private async Task<IPage> OpenTheCatalogAsync()
    {
        var page = await _browser!.NewPageAsync();
        await page.GotoAsync(_catalog.Address);
        return page;
    }

    [Fact]
    public async Task OpeningTheCatalog_ShowsTheCatalogNameAsTheHeading()
    {
        var page = await OpenTheCatalogAsync();

        var heading = page.GetByRole(AriaRole.Heading, new() { Name = PageText.CatalogName });

        await Assertions.Expect(heading).ToBeVisibleAsync();
    }

    [Fact]
    public async Task OpeningTheCatalog_ShowsTheStatusLineWordForWord()
    {
        var page = await OpenTheCatalogAsync();

        var statusLine = page.GetByText(PageText.RunningStatus);

        await Assertions.Expect(statusLine).ToBeVisibleAsync();
    }

    [Fact]
    public async Task OpeningTheCatalog_PutsTheCatalogNameInTheBrowserTab()
    {
        var page = await OpenTheCatalogAsync();

        await Assertions.Expect(page).ToHaveTitleAsync(PageText.CatalogName);
    }

    [Fact]
    public async Task AskingForAPageThatDoesNotExist_DoesNotShowTheRunningMessage()
    {
        // A failure path, not just the happy one. Whatever a wrong address shows, it
        // must not claim the catalog page is what the person is looking at.
        var page = await _browser!.NewPageAsync();

        await page.GotoAsync($"{_catalog.Address}/a-page-that-does-not-exist");

        var body = await page.InnerTextAsync("body");
        Assert.DoesNotContain(PageText.RunningStatus, body);
    }
}
