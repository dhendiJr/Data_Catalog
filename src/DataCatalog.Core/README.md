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

---

## Updated for request 002 — 2026-09-21

### Added here

| File | What it does |
|---|---|
| `Models/CatalogChange.cs` | One thing that happened to a data source. Never trimmed (`D-027`) |
| `Models/CatalogChangeKind.cs` | Added, FieldChanged, Removed, PutBack |
| `Models/CatalogField.cs` | The seven fields, as something the history can point at |
| `Services/ICatalogService.cs` | Everything the catalog is allowed to do |
| `Services/CatalogService.cs` | **The rules.** Names, duplicates, removal, history |
| `Services/SaveResult.cs` | Why a save did or did not happen |
| `CatalogNeedsHelpException.cs` | The file is readable but in a state this code will not change on its own |

`CatalogEntry` gained `IsRemoved`. Removing hides; nothing here ever deletes an entry.

### Why the history stores a field rather than a label

`CatalogChange.Field` holds `CatalogField.Owner`, not the word "Owner". Labels are rendered through `PageText.LabelFor` at the moment of display, so if the Product Owner ever renames a field, its entire history renames with it instead of becoming a mixture of old and new wording.

### Where the rules live

All of them in `CatalogService`, none in a page and none in storage. That is what lets 71 tests cover the catalog's behavior without a browser or a database.

### Wording marked DRAFTED

`PageText` now distinguishes wording the Product Owner chose word for word from wording an agent drafted and they approved as part of a plan. Anything marked DRAFTED is a candidate for review if the catalog's voice is ever revisited.
