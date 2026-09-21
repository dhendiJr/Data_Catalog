# Naming Conventions

Names are the first documentation anyone reads. A consistent name tells a reader what a thing is before they open it.

These conventions are not negotiable and not a matter of taste. If you think one is wrong, that is a conversation with the Product Owner, not a decision you make while typing.

The guiding rule underneath all of them: **full words, no abbreviations, no invented shorthand.** `sensitivityLevel`, never `sensLvl` or `sl`. The only abbreviations permitted are ones the district itself uses out loud, and each one must be defined in `docs/GLOSSARY.md` the first time it appears.

---

## C# code

| Thing | Convention | Example |
|---|---|---|
| Class, record, struct | PascalCase, a noun | `CatalogEntry`, `SensitivityLevel` |
| Interface | PascalCase, prefixed `I` | `ICatalogRepository` |
| Method | PascalCase, a verb phrase | `GetAllEntries`, `ValidateName` |
| Async method | PascalCase, verb phrase, suffixed `Async` | `SaveEntryAsync` |
| Public property | PascalCase, a noun | `UpdateFrequency` |
| Local variable, parameter | camelCase | `entryName`, `sensitivityLevel` |
| Private field | camelCase with leading underscore | `_repository` |
| Constant | PascalCase | `MaximumDescriptionLength` |
| Enum type | PascalCase, singular | `SensitivityLevel` — not `SensitivityLevels` |
| Enum member | PascalCase | `Public`, `Internal`, `Restricted` |
| Boolean | reads as a statement | `isArchived`, `hasContact`, `canEdit` |

**Banned words in type names:** `Helper`, `Manager`, `Util`, `Utils`, `Common`, `Misc`, `Data`, `Info`, `Processor`, `Handler` when used vaguely. Each is a sign that the class has no single job (`P22`). If you cannot name a class without one of these words, the class is wrong, not the name.

**Class names do not repeat their namespace.** Inside a `Catalog` namespace, the class is `Entry` or `CatalogEntry` — never `CatalogCatalogEntry`.

## Blazor components

| Thing | Convention | Example |
|---|---|---|
| Component file | PascalCase, `.razor` | `CatalogEntryForm.razor` |
| Page component | PascalCase, named for what it shows | `CatalogList.razor`, `CatalogEntryDetail.razor` |
| Code-behind | Component name plus `.razor.cs` | `CatalogEntryForm.razor.cs` |
| Component parameter | PascalCase | `[Parameter] public CatalogEntry Entry` |
| Event callback | `On` plus what happened | `OnEntrySaved`, `OnDeleteRequested` |
| Route | lower case, hyphenated, plural for lists | `/catalog`, `/catalog/add`, `/catalog/{id}` |

**Component names say what they show, not what they are made of.** `SensitivityBadge`, not `MudChipWrapper`. If MudBlazor is ever swapped out, the name should still make sense.

**Never prefix your own components with `Mud`.** That prefix belongs to MudBlazor. A component of ours called `MudCatalogTable` will confuse every future reader about who owns it.

## Files and folders

| Thing | Convention | Example |
|---|---|---|
| C# file | one public type per file, named for it | `CatalogEntry.cs` |
| Folder | PascalCase, matching the namespace | `Services/`, `Repositories/` |
| Project | `DataCatalog.<Area>` | `DataCatalog.Web`, `DataCatalog.Core` |
| Test project | the project under test plus `.Tests` | `DataCatalog.Core.UnitTests` |

Folders group by **what the code is about**, not by what kind of code it is, once an area grows. Prefer `Catalog/Services/EntryValidator.cs` over a single project-wide `Services/` folder holding twenty unrelated services.

## Tests

Test names are sentences. They describe the behavior in plain words so that a failure message reads like a statement of what broke.

```
MethodOrFeature_Condition_ExpectedResult
```

| Good | Why |
|---|---|
| `SaveEntry_WithBlankName_ShowsValidationMessage` | Reads as a sentence; the failure explains itself |
| `Search_WithNoMatches_ShowsEmptyMessage` | States the edge case being covered |
| `GetAll_WhenCatalogIsEmpty_ReturnsEmptyList` | Names the boundary condition |

| Bad | Why |
|---|---|
| `Test1`, `TestSave`, `SaveWorks` | Tells the reader nothing when it fails |
| `SaveEntryTest` | The word "test" adds nothing; it is in a test project |

Test class: the thing under test plus `Tests` — `CatalogEntryServiceTests`.

## Markdown and project records

| Thing | Convention | Example |
|---|---|---|
| Request file | `NNN-short-name.md`, lower case, hyphenated | `requests/004-add-search.md` |
| Branch | `feature/NNN-short-name` matching the request | `feature/004-add-search` |
| Decision | `D-NNN`, sequential, never reused | `D-014` |
| Rulebook document | UPPER_SNAKE_CASE `.md` in `docs/` | `docs/NAMING_CONVENTIONS.md` |
| Area guide | `README.md` inside the area folder | `src/DataCatalog.Web/Components/Catalog/README.md` |

Request numbers, branch names, and decision IDs are the threads that tie a piece of code back to the conversation that authorized it. Keep them aligned.

## Things people see on screen

User-facing wording is **never invented by an agent** (`Question Playbook`, category 6). Button labels, page titles, error messages, and field names come from the Product Owner and are written into the approved plan.

Where the PO has chosen a word, that word is used everywhere — on screen, in code, in tests, in the glossary. If the district calls it a "data source", nothing anywhere in this project calls it a "dataset", an "asset", or a "resource".

## When a convention does not cover your case

Look for the nearest existing example in the codebase and match it. If there genuinely is no precedent, choose the option a stranger would guess, use it consistently, and raise it with the PO at the end so the convention can be added here.

Do not invent a second way of naming something that already has one way.
