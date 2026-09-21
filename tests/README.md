# Tests

_Last updated: 2026-09-21 — request 001_

## What this area is for

Proving the catalog does what the Product Owner approved, and catching it when it stops. The standards these tests follow are in `docs/TESTING_STANDARDS.md`; this file says how they are arranged here.

## What lives here

| Project | What it tests | How it runs |
|---|---|---|
| `DataCatalog.Core.UnitTests` | Rules, wording, and storage round-trips | xUnit, no browser, no web server |
| `DataCatalog.Web.ComponentTests` | Blazor components on their own | bUnit, no browser |
| `DataCatalog.UiTests` | The real catalog in a real browser | Playwright, starts the app for itself |

Run everything with:

```
dotnet test
```

## How to add a test

1. Put it in the project that matches how much of the system it needs. Prefer the cheapest one that genuinely proves the point.
2. Name it `Thing_Condition_ExpectedResult` — a sentence that explains itself when it fails.
3. Cover the applicable rows of the edge case matrix, not only the happy path.
4. **Watch it fail before you make it pass.** A test that has never failed proves nothing.

## The rules of this area

- **Never weaken a test to get to green.** If a test fails, the code is wrong until proven otherwise. If you believe the test itself is wrong, that is a stop — ask the Product Owner.
- **No real people.** Test data is invented and obviously fake: "Northside Elementary", "Chris Example". Never a real student, family member, or staff member.
- **Tests are independent.** Each gets its own temporary catalog file and its own port. Any test can run alone, in any order.
- **Find things the way a person would.** In browser tests, by visible text and role — never by internal identifiers.

## Things that have already caught people out

- **SQLite keeps the catalog file locked** after the context is disposed, because connections are pooled. A test that reads the file directly must call `TemporaryCatalogFile.ReleaseFileLocks()` first. This failed exactly once, for reasons that looked nothing like the real cause.
- **bUnit needs MudBlazor's services** registered and `JSInterop.Mode = JSRuntimeMode.Loose`, or components fail to render in ways that look like your markup is wrong.
- **`Program` is ambiguous in `DataCatalog.UiTests`**, because that project references the web app and its top-level `Program` collides with Playwright's. Write `Microsoft.Playwright.Program.Main` in full.
- **Playwright downloads a browser** the first time the UI tests run. That run is slow and needs an internet connection; later runs do not.
- **The UI tests run the built application**, not `dotnet run`. They look upward for `DataCatalog.slnx` to find the project folder, then start `src/DataCatalog.Web/bin/Debug/net10.0/DataCatalog.Web.dll`. If they complain the catalog has not been built, run `dotnet build` first.

## Known rough edges

- `DataCatalog.Core.UnitTests` also holds the storage tests, which touch `DataCatalog.Infrastructure` and a real file on disk. Strictly those are not unit tests. They live there because splitting them into a fourth project buys nothing today — but the name promises slightly less than it delivers, and `docs/ARCHITECTURE.md` says so rather than pretending otherwise.
