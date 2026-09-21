# Request 001 — Project Setup

**Status:** Building
**Opened:** 2026-09-21
**Product Owner:** Product Owner
**Branch:** `feature/001-project-setup` — created after approval, left unmerged
**Status of branch:** building

---

## What was asked for

> now start request 001 to set up the project

Prior context from the setup conversation: request 001 was identified as "creating the .NET 10 solution, the Blazor Server app, the three test projects, and pinning the real version numbers into the package ledger," plus `git init` and a `.gitignore` (`docs/BRANCHING.md`).

## What I understand this to be

Build the empty shell of the catalog application — the folders, the projects, the testing setup, and version control — so that from the next request onward there is somewhere for real features to go. Nothing a staff member could use yet.

---

## What the machine already has

Checked before asking, so these are facts rather than questions:

| Thing | Found |
|---|---|
| .NET SDK | 10.0.401 — satisfies `D-008` |
| Older SDK also present | 8.0.425 |
| git | 2.55.0 |
| Repository | Does not exist yet |

---

## Questions and answers

| # | Question asked | PO's answer | Date |
|---|---|---|---|
| 1 | How much should request 001 deliver — bare skeleton, or a skeleton plus one visible page? | Shell plus one visible page. Proves the whole toolchain, including browser testing, before real features are built on it. No catalog features. | 2026-09-21 |
| 2 | What should the catalog be called, on screen? | **Data Catalog.** Used everywhere from now on — page title, code, tests, glossary (P27). | 2026-09-21 |
| 3 | How should the PO start the app on their computer? | A file in the project folder they double-click. A console window stays open while it runs; the browser opens by itself; closing the window stops it. | 2026-09-21 |
| 4 | What should the one visible page actually say? | Heading **Data Catalog**; below it **The catalog is running. No data sources have been added yet.** Exact wording — the browser test checks for it. | 2026-09-21 |
| 5 | What should happen if it fails to start? | The window stays open with a plain-English explanation and what to try, with the technical detail printed underneath. Most likely cause is the port already being in use. | 2026-09-21 |
| 6 | Should the project's history stay only on this computer, or be backed up online? | This computer only. Nothing is ever sent anywhere. Online backup would be its own request with its own questions. | 2026-09-21 |
| 7 | Boundaries check — is anything expected from setup that has not been asked about? | **Data storage is to be settled in this request too.** Everything else on the out-of-scope list stays out. Storage questions follow from Q8. | 2026-09-21 |
| 8 | How should catalog entries be stored? | A single database file in the project folder. Backed up by copying one file; safe against interrupted or simultaneous saves; has room for the edit history required by D-005. Accepted tradeoff: not readable by hand outside the catalog. | 2026-09-21 |
| 9 | Does 001 build the storage, or only settle the decision? | Build it. Writing an entry and reading it back must work, proven by automated tests. The page wording stays exactly as agreed in Q4 — no status line invented to show it off. | 2026-09-21 |
| 10 | What should happen if the database file is missing when the catalog starts? | Create a new empty one and start normally — this is also the first-run path. Accepted risk: an accidentally deleted file shows as an empty catalog rather than an error. | 2026-09-21 |
| 11 | What should happen if the database file exists but is damaged or unreadable? | Refuse to start. The window explains the file appears damaged and says where it is. The file is never overwritten or replaced. | 2026-09-21 |
| 12 | The seven fields (D-010) include sensitivity. How is it stored before the district's levels are known? | Free text for now. The actual list of levels is settled when the screen for picking one is built — cheap to change while the catalog is empty, expensive once entries exist. | 2026-09-21 |
| 13 | What exact words should the two failure messages use? | The drafted wording, used exactly as written, including the "do not delete this file" warning and the file location. Tests check for these words. | 2026-09-21 |
| 14 | Which add-on is used to talk to the database file? | Entity Framework Core — the one that manages reshaping the data file when fields are added later. More machinery than needed today, but hand-written schema changes on a catalog that already holds entries is where real data gets lost. | 2026-09-21 |

---

## The Plan

_Version 1 — 2026-09-21_

### What you will be able to do when this is finished

You will be able to double-click a file on your computer and watch the Data Catalog open in your browser. It will say it is running and that nothing has been added yet. Behind that page, the place where your data sources will be stored will exist and will have been proven to work.

Nobody can add or look up a data source yet. This request builds the foundation and proves every part of it works before any real feature is built on top.

### What changes that you can see

- A file in the project folder you double-click to start the catalog
- A black console window that stays open while the catalog is running, and stops it when closed
- Your browser opening by itself to the catalog
- One page, headed **Data Catalog**, reading **The catalog is running. No data sources have been added yet.**
- A catalog file appearing in the project folder the first time you run it — this is where your entries will live

### What happens step by step

1. You double-click the start file.
2. A console window opens and stays open.
3. The catalog checks for its data file. If there is not one, it creates a new empty one.
4. Your browser opens to the catalog page.
5. The page shows the heading and the status line above.
6. You close the console window when you are finished, and the catalog stops.

### When things go wrong

| If this happens | The person sees | Do they lose their work? |
|---|---|---|
| The data file is missing, including the very first time it is ever run | Nothing unusual — a new empty catalog is created and it starts normally | No. There was nothing there to lose |
| The data file is there but damaged | The catalog refuses to start. The window explains the file appears damaged, gives its location, and says not to delete it. The file is never changed or replaced | No. The damaged file is left completely untouched so it can be recovered |
| Another program is already using the address the catalog needs, including starting it twice by accident | The window stays open and explains that another program is using the address, and to close it and try again | No |
| Any other failure at startup | The window stays open with the plain explanation and the technical detail underneath | No |

#### The exact failure wording, approved at question 13

Used word for word. The tests check for these words, so they cannot be changed later without a test failing.

**Address already in use:**

```
Data Catalog could not start.

Another program on this computer is already using the address
the catalog needs. Close that program, then close this window
and try again.

Technical detail for whoever helps you:
<the original error text>
```

**Damaged catalog file:**

```
Data Catalog could not start.

The catalog file appears to be damaged. It has NOT been changed
or replaced.

File location: <full path to the file>

Do not delete this file - someone may be able to recover your
entries from it.

Technical detail for whoever helps you:
<the original error text>
```

### What gets tested

- A catalog entry, with all seven fields, can be written to the data file and read back exactly as written
- A missing data file causes a new empty one to be created, and the catalog starts
- A damaged data file causes a refusal to start, the correct message, and **no change to the file**
- The seven fields survive awkward content: blank values, only spaces, very long text (10,000 characters), apostrophes and quotation marks, accented letters and emoji, and text shaped like a database command, which must be stored and shown as plain text and never run
- Two entries with the same name can both be stored — rejecting duplicates is a rule for a later request, and this test records that today they are allowed
- The page shows the exact heading and status line agreed
- A real browser opens the running catalog and finds that wording on the page

### New packages needed

| Package | Version | What it does, in plain English | Why we cannot do this without it |
|---|---|---|---|
| Microsoft.EntityFrameworkCore.Sqlite | current 10.x, pinned and recorded at install | Reads and writes the catalog file | Writing database instructions by hand is where real data gets lost |
| Microsoft.EntityFrameworkCore.Design | current 10.x, pinned and recorded at install | Handles reshaping the data file safely when fields are added later | You have said more fields are coming; this is what makes that safe once entries exist |

Both are free, made by Microsoft, and carry the MIT licence. `Microsoft.EntityFrameworkCore` comes with them as a dependency and will be listed in the ledger too. The already-approved founding packages (MudBlazor, xUnit, bUnit, Playwright and their runners) are also installed here, and their exact versions written into `approvals/PACKAGE_APPROVALS.md` before anything else is done.

**If any version I install differs from what is recorded here, I stop and ask.**

### Structure and documents this affects

This creates the structure described in `docs/ARCHITECTURE.md`, so that document changes from a proposal to a description of what exists, including the answers about storage. Four decisions get recorded: the storage choice, how missing and damaged files behave, sensitivity as free text for now, and local-only version control.

An area guide (`README.md`) is written for each part created: the core rules project, the storage project, and the web application.

### Branch

`feature/001-project-setup`, created after this approval and before any code. It will be left unmerged when the work is finished — merging is a separate decision for you (`D-014`).

The catalog data file is deliberately kept out of version control. It is your data, not part of the software, so **backing it up means copying that file** — version control will not do it for you.

### What is NOT included

- Adding, editing, viewing, searching or deleting data sources
- Any screen other than the single page described
- The edit history required by `D-005` — the storage is shaped to hold it, but nothing records it yet
- The district's list of sensitivity levels — stored as free text for now
- Duplicate checking, required fields, or any validation
- District colors, logo, or fonts
- Any backup, online or otherwise

### How this gets undone

The branch is abandoned and nothing changes. If it has already been merged, deleting the `src` and `tests` folders and the start file returns the project to a rulebook-only state.

### How long, roughly

A short task. The uncertainty is in Playwright, which downloads browser software the first time it runs and can be slow. If it fails, I will say so rather than quietly skipping the browser test.

---

## Approval

**Approved by:** Product Owner
**Date:** 2026-09-21
**Their exact words:** "approved"
**Plan version approved:** Version 1 — 2026-09-21

Approving this plan also approves the two new packages listed in it (`Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`), per `docs/DEPENDENCY_POLICY.md`.

---

## What was built

_Not started._

### Test results

```
not run
```

### Anything that did not go to plan

### Noticed while building

---

## Definition of Done checklist

_To be copied from `docs/DEFINITION_OF_DONE.md` when the work is complete._
