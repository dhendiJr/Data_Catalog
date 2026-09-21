# Architecture

**This is a living document.** It describes how the project is actually put together, and it is updated in the same change that alters the structure — never afterward, never "when we get a chance."

An architecture document that has drifted from the code is worse than none at all, because the next agent will trust it and be wrong.

---

## Maintenance rule

> **If a change adds a project, a layer, a folder, a dependency between layers, or a new kind of thing — this file is updated in that same piece of work, and the update is named in the plan.**

The Definition of Done checks for it. An agent that adds a service layer and does not describe it here has not finished.

Structural changes are also recorded as decisions in `docs/DECISIONS.md`, so the *why* survives even when the structure later changes again.

---

## The shape of the system

The catalog is one Blazor Server application running on a single computer, reachable only from that computer (`D-006`). A person opens it in a browser. The pages run on the machine hosting the app, and the browser shows the results.

There are four layers. **Dependencies point in one direction only** — downward in this table. Nothing below ever knows about anything above it.

| Layer | What lives here | Knows about | Must never |
|---|---|---|---|
| **Components** | Blazor pages and components (`.razor`), MudBlazor usage, everything a person sees | Services, Models | Contain business rules, or read and write storage directly |
| **Services** | Business rules, validation, decisions about what is allowed | Repositories (as interfaces), Models | Know that Blazor or MudBlazor exist |
| **Repositories** | Reading and writing stored data | Models | Contain business rules or validation |
| **Models** | The shape of a catalog entry and its related types | Nothing | Depend on any layer above |

This is `P22` made concrete. The test that it is working: **every business rule can be tested without a browser and without storage.** If a rule can only be exercised by clicking through the app, it is in the wrong layer.

### Why this shape

Because of `D-005` — no logins, anyone can edit — the catalog needs an edit history and reliable validation. Both are business rules. Keeping them in services rather than in pages means they hold no matter which screen, form, or future import path reaches them. A rule enforced only in a form is a rule that a second form will forget.

## Folder structure

```
DataCatalog.slnx               the solution
Start Data Catalog.cmd         what the Product Owner double-clicks
datacatalog.db                 the catalog file - their data, kept out of version control
src/
  DataCatalog.Core/            models, rules, interfaces - no UI, no storage, no packages
    Models/                    CatalogEntry: the seven fields of D-010
    Abstractions/              ICatalogRepository
    Text/                      wording the Product Owner approved, word for word
    README.md
  DataCatalog.Infrastructure/  the catalog file and how it is read and written
    CatalogDbContext.cs        the shape of the file
    SqliteCatalogRepository.cs implements ICatalogRepository
    CatalogDatabase.cs         missing file versus damaged file, at startup
    README.md
  DataCatalog.Web/             the Blazor Server app
    Components/
      Layout/
      Pages/                   Home.razor - the only page today
    Program.cs                 startup, dependency injection, every failure path
    README.md
tests/
  DataCatalog.Core.UnitTests/         xUnit - rules, wording, storage round-trips
  DataCatalog.Web.ComponentTests/     bUnit - components without a browser
  DataCatalog.UiTests/                Playwright - the real app in a real browser
  README.md
docs/                          the rulebook
approvals/                     signed approvals
requests/                      one file per request
```

**This is what exists, built and tested under request 001.** Each project carries its own `README.md` explaining how to work in it.

Note that `DataCatalog.Core.UnitTests` also holds the storage round-trip tests, which use a real temporary file and therefore reach into `DataCatalog.Infrastructure`. Strictly they are not unit tests. A fourth test project would buy nothing today, so the name promises slightly less than it delivers — recorded here rather than left to be discovered.

## Dependency injection

Everything crosses layers through interfaces registered in `Program.cs`. Components receive services; services receive repositories. Nothing constructs its own dependencies (`P22`).

This is the single most load-bearing decision in the project. It is what lets bUnit test a component with a fake repository and no database, and it is why adding a second storage mechanism later would not require touching a single component.

## How entries are stored

One SQLite file, `datacatalog.db`, sitting in the project folder next to the start file, read and written through Entity Framework Core (`D-019`). One table, `CatalogEntries`, holding the seven fields of `D-010` plus an id.

The file is deliberately excluded from version control. It is the district's data, not the software — which means **backing it up is copying that one file**, and nothing else does it for them.

At startup (`D-020`):

- **File missing** — a new empty one is created and the catalog starts. This is also the very first run.
- **File damaged** — the catalog refuses to start, says where the file is, warns against deleting it, and leaves it byte for byte untouched. A test asserts the bytes are unchanged, because the failure that matters here is silent replacement.

Today nothing is required and nothing is validated. Validation is a rule for a later request, and the tests record what is true now rather than what someone assumes.

## Still open — do not assume

| Open question | Why it matters |
|---|---|
| **How the edit history is kept** (required by `D-005`) | Nothing records changes yet. The storage has room for it; the shape is undecided |
| **The district's sensitivity levels** | Stored as free text until then (`D-018`) |
| **Whether duplicate names are allowed** | Today they are, and a test records that. Nobody has decided they should be |
| **Migrations** | The schema is created directly rather than through migrations, because generating them needs a tool nobody has approved installing. See `src/DataCatalog.Infrastructure/README.md` |

Record each answer in `docs/DECISIONS.md` and update this file in the same change.

## Rules for extending the architecture

### Adding a new kind of thing
Find the layer it belongs to using the table above. If it does not fit any layer, **stop and ask** — a thing that fits nowhere usually means the design needs a conversation, not a new folder.

### Adding a new folder or area
The folder gets a `README.md` explaining how to work in it, from `docs/AREA_GUIDE_TEMPLATE.md`. This is required, not encouraged (`P28`).

### Adding a dependency between layers
If you need a lower layer to know about a higher one, the design is inverted. Do not add the reference. Stop and ask — this is almost always solved by an interface in the lower layer that the higher one implements.

### Adding a NuGet package
`docs/DEPENDENCY_POLICY.md`. Approval and a ledger row before anything is installed.

### Changing the structure
Update this file in the same change. Record a decision. Name the structural change in the plan so the PO approved it explicitly, in plain English — "we would split the project in two so the rules can be tested on their own" rather than "refactor into Core and Infrastructure."

## What this architecture is deliberately not

- **Not distributed.** One app, one machine, one process (`D-006`).
- **Not multi-tenant.** One school district, one catalog.
- **Not built for authentication.** There are no logins (`D-005`). Do not add seams "ready for" a login system that nobody has asked for — that is a `P24` violation. If logins are ever wanted, that is a request, and it will change this document.
- **Not an API for other systems.** Nothing else talks to this app.

Every one of those could change. Each would be a request, refined and approved, and each would land here.
