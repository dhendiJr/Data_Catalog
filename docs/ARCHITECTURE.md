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
src/
  DataCatalog.Core/            models, services, repository interfaces — no UI, no storage
    Models/
    Services/
    Abstractions/              the interfaces repositories implement
  DataCatalog.Infrastructure/  the concrete repositories and storage
  DataCatalog.Web/             the Blazor Server app
    Components/
      Layout/
      Catalog/                 grouped by area, each with its own README.md
    Pages/
    Program.cs                 dependency injection registrations
tests/
  DataCatalog.Core.UnitTests/         xUnit — rules and logic
  DataCatalog.Web.ComponentTests/     bUnit — components without a browser
  DataCatalog.UiTests/                Playwright — the real app in a browser
docs/                          the rulebook
approvals/                     signed approvals
requests/                      one file per request
```

**This structure is a proposal until request 001 is approved.** It is written here so the plan for 001 has something concrete to agree or disagree with — not because it is settled.

## Dependency injection

Everything crosses layers through interfaces registered in `Program.cs`. Components receive services; services receive repositories. Nothing constructs its own dependencies (`P22`).

This is the single most load-bearing decision in the project. It is what lets bUnit test a component with a fake repository and no database, and it is why adding a second storage mechanism later would not require touching a single component.

## Still open — decide in request 001, do not assume

These are genuinely undecided. An agent that picks one without asking has violated `P1`.

| Open question | Why it matters |
|---|---|
| **How entries are stored** — a database file, a plain file, something else | Shapes the Infrastructure layer entirely |
| **How the edit history is kept** (required by `D-005`) | May change the shape of the stored data |
| **Whether Core and Infrastructure are separate projects** or one to start with | Simpler is not always fewer projects, but often is |
| **How the app is started** by the PO on their computer | Affects what request 001 must deliver |

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
