# DataCatalog.Core

_Last updated: 2026-09-21 — request 001_

## What this area is for

The things the catalog is about, and the rules for them. This project knows nothing about screens, browsers, MudBlazor, or where data is stored. It is the layer everything else depends on, and it depends on nothing.

If a rule can only be tested by clicking through the app, it is in the wrong place — it belongs here.

## What lives here

| File | What it does |
|---|---|
| `Models/CatalogEntry.cs` | One data source: the seven fields fixed by `D-010` |
| `Abstractions/ICatalogRepository.cs` | The promise that entries can be stored and read back. Storage implements it |
| `CatalogFileDamagedException.cs` | Raised when the catalog file exists but cannot be read |
| `Text/PageText.cs` | Wording the Product Owner chose for the screen |
| `Text/StartupMessages.cs` | Wording shown when the catalog will not start |

## How it fits the rest of the project

Nothing above it exists as far as this project is concerned. `DataCatalog.Infrastructure` implements `ICatalogRepository`; `DataCatalog.Web` uses both through dependency injection. See `docs/ARCHITECTURE.md`.

## How to add something here

1. Decide it genuinely belongs here — no screens, no storage, no MudBlazor.
2. Add the file to `Models/`, `Abstractions/`, or `Text/`, or propose a new folder in the plan first.
3. Name it per `docs/NAMING_CONVENTIONS.md`.
4. Add xUnit tests in `tests/DataCatalog.Core.UnitTests`, covering the applicable rows of the edge case matrix in `docs/TESTING_STANDARDS.md`.

## The rules of this area

- **No package references.** This project has none and should keep none. A package here would leak into every layer.
- **No knowledge of storage.** Interfaces describe what is needed; how it happens is somebody else's job.
- **No user-facing wording invented here.** Everything in `Text/` was approved word for word by the Product Owner, with the request and question number in a comment. Changing a string means asking first.
- **Do not add an eighth field to `CatalogEntry`** without a request. The seven are `D-010`.

## How this area is tested

`tests/DataCatalog.Core.UnitTests` — xUnit, no browser, no web server. Storage round-trip tests live there too, using a real temporary catalog file rather than a fake, because the thing worth proving is that entries really survive on disk.

## Decisions that shaped this area

| Decision | What it means here |
|---|---|
| D-010 | Seven fields, no more, until a request says otherwise |
| D-018 | `Sensitivity` is free text until the district's levels are settled |
| Request 001, Q13 | The startup messages are fixed wording with tests guarding them |

## Known rough edges

- **`CatalogEntry.SourceSystem` is shown to people as "System".** A C# property called `System` would shadow the `System` namespace inside the class and cause confusing errors for whoever edits it next. The screen label stays "System"; only the code name differs. This is the one place where code and the Product Owner's word are deliberately not identical.
