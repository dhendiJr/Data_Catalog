# DataCatalog.Infrastructure

_Last updated: 2026-09-21 — request 001_

## What this area is for

Putting catalog entries in the catalog file and getting them back out. Nothing else. No business rules, no validation, no decisions about what is allowed — those belong in services.

## What lives here

| File | What it does |
|---|---|
| `CatalogDbContext.cs` | The shape of the catalog file: one table, the seven fields plus an id |
| `SqliteCatalogRepository.cs` | Reads and writes entries. Implements `ICatalogRepository` |
| `CatalogDatabase.cs` | Gets the file ready at startup, and decides missing versus damaged |

## How it fits the rest of the project

Depends on `DataCatalog.Core` and on Entity Framework Core. `DataCatalog.Web` never refers to `SqliteCatalogRepository` by name — it asks for `ICatalogRepository` and is handed this. That indirection is what lets components be tested without a database.

## How to add something here

1. Add the property to `CatalogEntry` in Core first — the shape is Core's, not this project's.
2. Add it to `OnModelCreating` in `CatalogDbContext` if it needs anything beyond the default.
3. **Changing the shape of an existing catalog file needs a plan and approval.** See the rough edge below before assuming it is easy.
4. Add tests in `tests/DataCatalog.Core.UnitTests` using `TemporaryCatalogFile`.

## The rules of this area

- **No business rules.** If you are writing an `if` that decides whether something is allowed, it belongs in a service.
- **Never delete or overwrite the catalog file.** `CatalogDatabase` deliberately refuses to start rather than replacing a file it cannot read. A damaged file may still be recoverable; an overwritten one never is (request 001, Q11).
- **Only treat a failure as damage when the file already existed.** Otherwise a genuine first run would be reported as a damaged file.
- **Reading is the only proof of readability.** Opening a connection succeeds even on a file that is not a database, which is why `Prepare` runs a count.

## How this area is tested

`tests/DataCatalog.Core.UnitTests`:

- `CatalogDatabaseTests` — missing file, damaged file, empty file, running twice
- `SqliteCatalogRepositoryTests` — round-trip of all seven fields, and the awkward content from the edge case matrix

`TemporaryCatalogFile` gives each test its own real file in the temp folder. It calls `SqliteConnection.ClearAllPools()` before reading a file directly, because SQLite keeps the file locked after the context is disposed — without that, tests fail for reasons that have nothing to do with the code.

## Decisions that shaped this area

| Decision | What it means here |
|---|---|
| D-019 | Entries live in a single SQLite file, read and written through Entity Framework Core |
| D-020 | Missing file is created; damaged file refuses to start and is left untouched |

## Reshaping the catalog file

Settled in request 002 (`D-034`). The file's shape is changed through **migrations**, using the `dotnet-ef` tool that lives in the project folder (`dotnet-tools.json`). `CatalogDatabase.Prepare` calls `Migrate()` at startup, so a catalog holding real entries gains a new field without losing anything.

To add a field:

1. Add it to `CatalogEntry` in Core.
2. Map it in `OnModelCreating` if it needs anything beyond the default.
3. `dotnet dotnet-ef migrations add <WhatChanged> --project src/DataCatalog.Infrastructure --startup-project src/DataCatalog.Infrastructure`
4. Read the generated migration before trusting it.
5. It applies itself the next time the catalog starts.

`DesignTimeCatalogDbContextFactory` exists so the tool never starts the web application — `Program.cs` catches every exception to show a plain-English message and would swallow the signal the tool uses to stop the app.

## Known rough edges

- **A catalog file created by request 001 predates migrations.** It has the tables but no record of which reshapings it has had, so migrating it fails with "table already exists" — which looks exactly like damage and is not. `UpgradeFileFromBeforeReshapingExisted` handles it: if such a file is **provably empty** its old table is dropped and the correct shape is built; if it holds anything at all, nothing is touched and the catalog refuses to start with an explanation. This code will not be the reason somebody loses data.
