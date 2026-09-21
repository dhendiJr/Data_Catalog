# Approved NuGet Packages

This is the ledger. **A NuGet package may exist in this project only if it has a row in this table.**

Adding a row requires the Product Owner's explicit approval, obtained per `docs/DEPENDENCY_POLICY.md`. An agent never adds a row on its own authority, and never installs a package before its row exists.

Before reporting any work done, compare every `PackageReference` in every `.csproj` against this table. Anything in the project that is not in this table is a stop — report it immediately.

---

## Approved and installed

Versions below are the exact ones pinned in the `.csproj` files as of request 001.

| # | Package | Version | Where | What it does, in plain English | Licence | Approved by | Date | Request |
|---|---|---|---|---|---|---|---|---|
| 1 | MudBlazor | 9.10.0 | Web | The ready-made screen pieces — buttons, tables, forms, menus — the catalog's pages are built from | MIT | Product Owner | 2026-09-21 | Project setup |
| 2 | Microsoft.EntityFrameworkCore.Sqlite | 10.0.12 | Infrastructure | Reads and writes the catalog file | MIT | Product Owner | 2026-09-21 | 001 |
| 3 | Microsoft.EntityFrameworkCore.Design | 10.0.12 | Infrastructure | Handles reshaping the catalog file safely when fields are added later | MIT | Product Owner | 2026-09-21 | 001 |
| 4 | xunit | 2.9.3 | all 3 test projects | Runs the automated checks | Apache 2.0 | Product Owner | 2026-09-21 | Project setup |
| 5 | xunit.runner.visualstudio | 3.1.4 | all 3 test projects | Lets those checks run from `dotnet test` and Visual Studio | Apache 2.0 | Product Owner | 2026-09-21 | Project setup |
| 6 | Microsoft.NET.Test.Sdk | 17.14.1 | all 3 test projects | The plumbing .NET needs to run any tests at all | MIT | Product Owner | 2026-09-21 | Project setup |
| 7 | bunit | 2.11.3 | ComponentTests | Tests individual pieces of a screen without opening a browser | MIT | Product Owner | 2026-09-21 | Project setup |
| 8 | Microsoft.Playwright | 1.62.0 | UiTests | Opens the real catalog in a real browser and clicks through it automatically | MIT | Product Owner | 2026-09-21 | Project setup |

### Arriving as dependencies of the above

Listed because the policy counts them, not because anything references them directly.

| Package | Version | Comes with | Notes |
|---|---|---|---|
| Microsoft.EntityFrameworkCore | 10.0.12 | rows 2 and 3 | The core of Entity Framework |
| Microsoft.EntityFrameworkCore.Relational | 10.0.12 | row 2 | Shared database plumbing |
| Microsoft.Data.Sqlite.Core | 10.0.12 | row 2 | The SQLite driver. Used directly in tests to release file locks |
| SQLitePCLRaw.\* | as resolved by row 2 | row 2 | The native SQLite engine |

**Playwright also downloads a copy of Chromium** the first time the UI tests run. That is browser software, not a NuGet package, and it is stored outside the project in the user's own Playwright cache. It was disclosed in the request 001 plan and approved with it.

## Requested and declined

| Package | Requested for | Why declined | Decided by | Date |
|---|---|---|---|---|
| coverlet.collector 6.0.4 | Nothing — added automatically by the `dotnet new xunit` template | Never presented, never approved, and not needed by anything in the plan. Removed from all three test projects at the moment it was noticed, per `docs/DEPENDENCY_POLICY.md` | Agent, under the standing policy | 2026-09-21 |
| Microsoft.Playwright.NUnit / .Xunit | Connecting browser tests to the test runner | Turned out not to be needed — plain `Microsoft.Playwright` with an xUnit fixture does the job. One package fewer | Agent, under `P24` | 2026-09-21 |

## Removed

| Package | Why removed | Removed by | Date |
|---|---|---|---|
| _none yet_ | | | |

## Not installed, and why it matters

`dotnet-ef`, the tool that generates migrations, is **not** installed. Installing software is a hard stop (`docs/GUARDRAILS.md`) and nobody has approved it. The consequence is recorded in `src/DataCatalog.Infrastructure/README.md`: the first field added to a catalog that already holds entries needs that tool approved and the database baselined first.

## Approved tools

Not NuGet packages installed into a project, but software the project carries and uses.

| Tool | Version | Where | What it does, in plain English | Approved by | Date | Request |
|---|---|---|---|---|---|---|
| dotnet-ef | 10.0.12 | `dotnet-tools.json` in the project folder | Works out how to reshape the catalog file when fields are added, so entries already in it are not lost | Product Owner | 2026-09-21 | 002, question 18 |

This was an explicit exception to the hard stop on installing software (`docs/GUARDRAILS.md`). It lives inside the project folder, not on the machine, and comes back with `dotnet tool restore`.

**Note on where it lives:** .NET 10 writes the manifest to `dotnet-tools.json` at the project root rather than the `.config/dotnet-tools.json` older guidance describes.
