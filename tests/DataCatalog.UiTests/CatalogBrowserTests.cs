using DataCatalog.Core.Text;
using Microsoft.Playwright;

namespace DataCatalog.UiTests;

/// <summary>
/// Adding, viewing, editing and removing a data source, in a real browser, the way a
/// person would. Everything is found by what is on the screen rather than by internal
/// identifiers, so these survive a MudBlazor upgrade.
/// </summary>
public class CatalogBrowserTests : IClassFixture<RunningCatalogApp>, IAsyncLifetime
{
    private readonly RunningCatalogApp _catalog;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public CatalogBrowserTests(RunningCatalogApp catalog)
    {
        _catalog = catalog;
    }

    public async Task InitializeAsync()
    {
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

    /// <summary>
    /// Opens the catalog and waits until Blazor has connected, so clicks are not sent to
    /// a page that cannot yet respond to them.
    /// </summary>
    private async Task<IPage> OpenTheListAsync()
    {
        var page = await _browser!.NewPageAsync();
        await GoAndWaitUntilTheCatalogCanRespondAsync(page, _catalog.Address);

        return page;
    }

    /// <summary>
    /// Goes to a page and waits until the catalog can actually respond to a click.
    /// Without this, a test clicks the first, non-responding drawing of the page and
    /// nothing happens - see the note in MainLayout.razor.
    /// </summary>
    private static async Task GoAndWaitUntilTheCatalogCanRespondAsync(IPage page, string url)
    {
        await page.GotoAsync(url);

        await page.Locator("#catalog-is-interactive")
            .WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });
    }

    /// <summary>
    /// Loads the page the browser is already on, from scratch, and waits until it can
    /// respond. Needed after following a link: moving between pages inside the catalog
    /// does not redraw the frame around them, so the readiness marker is already there
    /// from the previous page and proves nothing about this one.
    /// </summary>
    private static async Task WaitUntilThisPageCanRespondAsync(IPage page) =>
        await GoAndWaitUntilTheCatalogCanRespondAsync(page, page.Url);

    /// <summary>
    /// Opens the add form directly and waits until it can be typed into. Getting there by
    /// clicking is covered by its own test; using the link here as well would make every
    /// other test fail for a reason that has nothing to do with what it is checking.
    /// </summary>
    private async Task<ILocator> OpenTheAddFormAsync(IPage page)
    {
        await GoAndWaitUntilTheCatalogCanRespondAsync(page, $"{_catalog.Address}/data-sources/add");

        var nameBox = page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Name));
        await Assertions.Expect(nameBox).ToBeEditableAsync(new() { Timeout = 30_000 });

        return nameBox;
    }

    private async Task AddADataSourceAsync(IPage page, string name, string owner = "Student Services")
    {
        var nameBox = await OpenTheAddFormAsync(page);

        await nameBox.FillAsync(name);
        await page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Owner)).FillAsync(owner);

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.Save, Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = name }))
            .ToBeVisibleAsync(new() { Timeout = 15_000 });
    }

    [Fact]
    public async Task TheAddButtonOnTheList_OpensTheForm()
    {
        var page = await OpenTheListAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = PageText.AddDataSource }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = PageText.AddDataSource }))
            .ToBeVisibleAsync(new() { Timeout = 15_000 });
    }

    [Fact]
    public async Task AddingADataSource_PutsItOnTheList()
    {
        var page = await OpenTheListAsync();

        await AddADataSourceAsync(page, "Attendance records");

        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = "Attendance records" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task AddingADataSource_WithoutAName_SaysWhatIsMissingAndSavesNothing()
    {
        var page = await OpenTheListAsync();

        await OpenTheAddFormAsync(page);
        await page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Owner)).FillAsync("Somebody");
        await page.GetByRole(AriaRole.Button, new() { Name = PageText.Save, Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText(PageText.NameRequired)).ToBeVisibleAsync();

        // And what was already typed is still there.
        await Assertions.Expect(page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Owner)))
            .ToHaveValueAsync("Somebody");
    }

    [Fact]
    public async Task OpeningADataSource_ShowsEverythingAboutIt()
    {
        var page = await OpenTheListAsync();
        await AddADataSourceAsync(page, "Food service balances", owner: "Nutrition Services");

        await page.GetByRole(AriaRole.Link, new() { Name = "Food service balances" }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Food service balances" }))
            .ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("Nutrition Services")).ToBeVisibleAsync();

        // Fields nobody filled in say so rather than showing a gap.
        await Assertions.Expect(page.GetByText(PageText.NotRecordedYet).First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task EditingADataSource_ChangesItAndRecordsWhatChanged()
    {
        var page = await OpenTheListAsync();
        await AddADataSourceAsync(page, "Transport routing", owner: "Transportation");

        await page.GetByRole(AriaRole.Link, new() { Name = "Transport routing" }).ClickAsync();
        await WaitUntilThisPageCanRespondAsync(page);

        await page.GetByRole(AriaRole.Link, new() { Name = PageText.Edit }).ClickAsync();
        await WaitUntilThisPageCanRespondAsync(page);

        await page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Owner)).FillAsync("Operations");
        await page.GetByRole(AriaRole.Button, new() { Name = PageText.SaveChanges }).ClickAsync();

        await Assertions.Expect(page.GetByText("Operations").First).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("Owner changed from")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task RemovingADataSource_TakesItOffTheListAndPuttingItBackReturnsIt()
    {
        var page = await OpenTheListAsync();
        await AddADataSourceAsync(page, "Library circulation");

        await page.GetByRole(AriaRole.Link, new() { Name = "Library circulation" }).ClickAsync();
        await WaitUntilThisPageCanRespondAsync(page);

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.Remove }).ClickAsync();

        // The confirmation box has to be on screen before its buttons can be told apart
        // from the page's own Remove button. Waiting for its wording checks that too.
        await Assertions.Expect(page.GetByText(PageText.ConfirmRemove("Library circulation")))
            .ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Button, new() { Name = PageText.KeepIt }))
            .ToBeVisibleAsync();

        // Click the Remove inside the box, not the page's own. They share a word, and the
        // box is drawn before the page content, so neither first nor last is safe.
        var confirmationBox = page.GetByRole(AriaRole.Dialog);
        await confirmationBox.GetByRole(AriaRole.Button, new() { Name = PageText.Remove }).ClickAsync();

        // Back on the list, and no longer on it.
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = PageText.AddDataSource }))
            .ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = "Library circulation" }))
            .Not.ToBeVisibleAsync();

        await GoAndWaitUntilTheCatalogCanRespondAsync(page, $"{_catalog.Address}/data-sources/removed");

        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = "Library circulation" }))
            .ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.PutBack }).ClickAsync();

        await GoAndWaitUntilTheCatalogCanRespondAsync(page, _catalog.Address);
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = "Library circulation" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task AddingADataSourceWithANameAlreadyUsed_WarnsButAllowsIt()
    {
        var page = await OpenTheListAsync();
        await AddADataSourceAsync(page, "Gradebook");

        await OpenTheAddFormAsync(page);
        await page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Name)).FillAsync("Gradebook");
        await page.GetByRole(AriaRole.Button, new() { Name = PageText.Save, Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText(PageText.DuplicateName("Gradebook"))).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.SaveAnyway }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Link, new() { Name = "Gradebook" }))
            .ToHaveCountAsync(2);
    }

    [Fact]
    public async Task LeavingAHalfFilledForm_AsksBeforeThrowingItAway()
    {
        var page = await OpenTheListAsync();

        await OpenTheAddFormAsync(page);
        await page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Name)).FillAsync("Half typed");

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.Cancel }).ClickAsync();

        await Assertions.Expect(page.GetByText(PageText.DiscardQuestion)).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = PageText.KeepEditing }).ClickAsync();

        // Still on the form, with what was typed intact.
        await Assertions.Expect(page.GetByLabel(PageText.LabelFor(Core.Models.CatalogField.Name)))
            .ToHaveValueAsync("Half typed");
    }

    [Fact]
    public async Task OpeningALinkToSomethingThatNeverExisted_SaysSo()
    {
        var page = await _browser!.NewPageAsync();

        await page.GotoAsync($"{_catalog.Address}/data-sources/987654");

        await Assertions.Expect(page.GetByText(PageText.CouldNotBeFound)).ToBeVisibleAsync();
    }
}
