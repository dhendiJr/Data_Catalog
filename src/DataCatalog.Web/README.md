# DataCatalog.Web

_Last updated: 2026-09-21 — request 001_

## What this area is for

The Data Catalog as a person experiences it: the pages, the layout, and starting the app up. It shows things and captures clicks. It does not decide anything.

## What lives here

| File | What it does |
|---|---|
| `Program.cs` | Starts the catalog, wires the layers together, and handles every way starting can fail |
| `Components/Pages/Home.razor` | The only page. Shows the catalog name and the status line |
| `Components/Layout/MainLayout.razor` | MudBlazor theme and popover providers, and the page frame |
| `Components/App.razor` | The HTML document, including MudBlazor's stylesheet and script |
| `Components/_Imports.razor` | Shared `@using` lines, including MudBlazor and the approved wording |

## How it fits the rest of the project

Top layer. Depends on Core and Infrastructure and is depended on by nothing except the tests. Services are registered in `Program.cs` and handed to components by dependency injection — a component never constructs its own.

## How to add something here

1. Add the `.razor` file under `Components/Pages/` (a page) or `Components/` (a piece of a page), named for what it shows.
2. Get wording from the Product Owner and put it in `DataCatalog.Core/Text/`, never inline in the markup.
3. Inject the services it needs; never `new` one up.
4. Add a bUnit test in `tests/DataCatalog.Web.ComponentTests` and a Playwright test in `tests/DataCatalog.UiTests`.

## The rules of this area

- **No business rules in a `.razor` file.** If you are writing a decision about what is allowed, it belongs in a service in Core.
- **No wording in the markup.** Every string a person reads comes from `PageText` or `StartupMessages`, so that the page and the tests cannot drift and nobody can reword something the Product Owner chose without a test failing.
- **Do not prefix our own components with `Mud`.** That prefix belongs to MudBlazor.
- **No external links.** No web fonts, no CDNs. The catalog must work with no internet connection, and nothing about it should leave this computer (`D-006`).
- **Never let a raw error reach the console.** Every failure path in `Program.cs` prints an approved plain-English message with the technical detail underneath.

## How the catalog is started and configured

The Product Owner double-clicks `Start Data Catalog.cmd` in the project folder. Three environment variables exist so the tests can drive the app without disturbing anything:

| Variable | Default | Why it exists |
|---|---|---|
| `DATACATALOG_URL` | `http://localhost:5199` | Browser tests use a free port of their own |
| `DATACATALOG_FILE` | `datacatalog.db` in the content root | The start file points this at the project folder; tests point it at a throwaway file |
| `DATACATALOG_NO_BROWSER` | not set | Set to `1` so tests do not open a browser window on every run |

## How this area is tested

- `tests/DataCatalog.Web.ComponentTests` — bUnit, no browser. Needs `Services.AddMudServices()` and `JSInterop.Mode = JSRuntimeMode.Loose`, or MudBlazor components will not render.
- `tests/DataCatalog.UiTests` — Playwright, real browser, real app. Finds things by visible text and role rather than by internal identifiers, so tests survive a MudBlazor upgrade.

## Decisions that shaped this area

| Decision | What it means here |
|---|---|
| D-008 | .NET 10, Blazor Server, MudBlazor |
| D-006 | Localhost only. Never make this reachable from anywhere else without a request |
| D-021 | Double-click start file; console window stays open on failure |
| Request 001, Q2, Q4, Q13 | Every word on screen and in the failure messages is fixed and test-guarded |

## Known rough edges

- **`MudText` uses `HtmlTag`, not `Component`, to choose its HTML tag.** `Component="h1"` compiles, silently becomes a stray HTML attribute, and leaves the page with no heading at all. The component test caught it; it would not have been obvious by looking.
- **`UseHttpsRedirection` was removed** from the template's `Program.cs`. The catalog listens on plain HTTP on localhost only, and the redirect had nowhere to send anyone.

---

## Updated for request 002 — 2026-09-21

### The pages

| File | Route | What it is |
|---|---|---|
| `Components/Pages/Home.razor` | `/` | The list: name, system, owner, sensitivity |
| `Components/Pages/AddDataSource.razor` | `/data-sources/add` | The add form |
| `Components/Pages/DataSourceDetail.razor` | `/data-sources/{Id:int}` | All seven fields, the history, Edit and Remove |
| `Components/Pages/EditDataSource.razor` | `/data-sources/{Id:int}/edit` | The same form, filled in |
| `Components/Pages/RemovedDataSources.razor` | `/data-sources/removed` | What has been removed, and Put back |
| `Components/DataSourceForm.razor` | — | The one form both adding and editing use |

The int constraint on `{Id:int}` is what keeps `/data-sources/removed` from being read as a data source called "removed".

### Rules this area learned the hard way

- **Never wrap navigation in a catch-all.** Blazor signals navigation by throwing. Guard the save, then navigate outside the `try` — otherwise a successful save leaves the person on a form they have already saved, with no explanation.
- **Do not read possibly-null state inside another component's content.** What sits inside `<MudAlert>` is not drawn when the surrounding `@if` runs; MudAlert draws it later, by which point the value may be gone. Read it into a local inside the `@if` first. Getting this wrong crashed the page the instant "Save anyway" was pressed, and the `@if` above it looked like it made that impossible.
- **`MudText` uses `HtmlTag`, not `Component`.** The wrong one compiles and silently becomes a stray HTML attribute.

## Known rough edges

- **`<Routes @rendermode="InteractiveServer" />` in `App.razor` is load-bearing.** Without it the whole catalog is static HTML: every button that does something simply does not respond. The template left it off, and request 001 never noticed because its one page had nothing to click.
- **`#catalog-is-interactive` in `MainLayout.razor` exists for the browser tests.** Empty, invisible, no runtime cost. See `tests/README.md` for why it is needed.
- **The catalog runs in Development.** `dotnet run` picks that up from `launchSettings.json`, which is also what serves MudBlazor's own stylesheet and script. Running the built file directly does not, and the catalog then dies on the first click.
