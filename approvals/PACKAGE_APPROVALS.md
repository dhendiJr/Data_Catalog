# Approved NuGet Packages

This is the ledger. **A NuGet package may exist in this project only if it has a row in this table.**

Adding a row requires the Product Owner's explicit approval, obtained per `docs/DEPENDENCY_POLICY.md`. An agent never adds a row on its own authority, and never installs a package before its row exists.

Before reporting any work done, compare every `PackageReference` in every `.csproj` against this table. Anything in the project that is not in this table is a stop — report it immediately.

---

## Approved

| # | Package | Version | What it does, in plain English | Licence | Approved by | Date | Request |
|---|---|---|---|---|---|---|---|
| 1 | MudBlazor | _pin at install_ | The set of ready-made screen pieces — buttons, tables, forms, menus — that the catalog's pages are built from, so we don't design every control by hand | MIT (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 2 | xunit | _pin at install_ | The framework that runs the automated checks on the project's logic | Apache 2.0 (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 3 | xunit.runner.visualstudio | _pin at install_ | Lets those checks run from `dotnet test` and from Visual Studio | Apache 2.0 (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 4 | Microsoft.NET.Test.Sdk | _pin at install_ | The plumbing .NET needs to run any tests at all | MIT (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 5 | bunit | _pin at install_ | Lets us test individual pieces of a screen quickly, without opening a browser | MIT (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 6 | Microsoft.Playwright | _pin at install_ | Opens the real app in a real browser and clicks through it the way a person would, automatically | MIT (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |
| 7 | Microsoft.Playwright.NUnit _or_ .Xunit | _pin at install_ | Connects the browser tests to the test runner so they run with everything else | MIT (free) | Product Owner | 2026-09-21 | Pre-approved at project setup |

**Note on versions:** the exact version of each founding package is set when the project is first created, and must be written into this table at that moment. Until then, "pin at install" means: pin the exact version in the `.csproj`, then record it here before doing anything else. Never use a floating or "latest" version.

## Requested and declined

| Package | Requested for | Why declined | Decided by | Date |
|---|---|---|---|---|
| _none yet_ | | | | |

## Removed

| Package | Why removed | Removed by | Date |
|---|---|---|---|
| _none yet_ | | | | |
