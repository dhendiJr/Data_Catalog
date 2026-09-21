# Request 002 — Add, Remove and Display Data Sources

**Status:** Awaiting approval
**Opened:** 2026-09-21
**Product Owner:** Product Owner
**Branch:** `feature/002-add-remove-display-data-sources` — created after approval, left unmerged
**Status of branch:** not yet created

---

## What was asked for

> start request 002 Data Catalog
> The catalog is running. No data sources have been added yet. lets get some starter content for adding data sources removing, and displaying

## What I understand this to be

Turn the empty catalog into one people can actually use: add a data source, see the ones that are there, and remove one. This is the first request that produces something a staff member could use.

Two readings of "starter content" need separating before anything is planned — see question 1.

---

## What is already settled and will not be re-asked

From `docs/DECISIONS.md`:

| Decision | What it means here |
|---|---|
| D-005 | No logins. Anyone who can reach the catalog can add and remove. **Edit history is therefore required, not optional** |
| D-010 | Seven fields: name, description, system, owner, contact, sensitivity, update frequency |
| D-018 | Sensitivity is free text until the district's own levels are settled |
| D-020 | Missing catalog file is created; damaged one refuses to start |
| D-022 | Nothing is validated today, and duplicate names are allowed. Neither is a decision — nobody has chosen them |
| Guardrails | No real student, family or staff data. Any example content is invented |

---

## Questions and answers

| # | Question asked | PO's answer | Date |
|---|---|---|---|
| 1 | Does "starter content" mean example entries already in the catalog, or just the features to put content in? | Just the ability to add things. **No invented entries** — the catalog starts empty and the first entry is a real one, so nobody ever wonders whether the list is half made-up. | 2026-09-21 |
| 2 | Should entries be editable, or only removed and re-added? | **Editing is included.** Scope is now add, display, edit, remove — the full set. | 2026-09-21 |
| 3 | Four features is large. Split into two requests? | **Keep it as one request.** The catalog arrives complete rather than useful-but-limited. Accepted cost: a longer questioning stretch and a longer build before anything is visible. | 2026-09-21 |
| 4 | How should the list of data sources be laid out? | A short list — name, system, owner, sensitivity — with a click opening that data source on its own page showing all seven fields plus edit and remove. Scannable, and fits a laptop screen without sideways scrolling. | 2026-09-21 |
| 5 | Which of the seven fields must be filled in before an entry can be saved? | **Name only.** Everything else can be filled in later; blank fields display as "not recorded yet", which is honest and shows what still needs work. Supersedes the no-validation state of D-022 for the name field. | 2026-09-21 |
| 6 | What does someone see when they try to save without a name? | The name box outlines in red with **"Please give this data source a name"** underneath. Nothing is saved and everything else they typed stays put. A name of only spaces counts as blank. | 2026-09-21 |
| 7 | What happens when someone adds a data source with a name that already exists? | **Warn, but allow.** A message says one with that name already exists, with a link to view it and a button to save anyway. A district genuinely can have two things called "Attendance"; blocking would force fake names. | 2026-09-21 |
| 8 | When someone removes a data source, is it gone for good or recoverable? | **Recoverable.** Removing hides it from the list; a "Removed data sources" page lets anyone put it back. With no logins, one bad day could otherwise empty the catalog with no way to tell what was lost. | 2026-09-21 |
| 9 | Should this request record a history of changes, as D-005 requires? | **Yes.** Every add, edit and removal records the date, time, and which fields changed from what to what. No "who" — there are no logins. Shown as a History section on each data source's page. Recorded now because history not captured is lost permanently. | 2026-09-21 |
| 10 | What happens if the same data source is edited in two places at once? | **Last save wins**, no warning. Safe because the history now preserves the overwritten values. **Must be revisited before the catalog goes on a server**, where it would mean two people silently overwriting each other. | 2026-09-21 |
| 11 | Is searching or filtering the list part of this request? | **Not yet.** The list shows everything, sorted by name. Search becomes its own request once there is enough to search through. | 2026-09-21 |
| 12 | Is the proposed set of on-screen wording right? | **Yes, as written.** Full table recorded in the plan below. Kept in one place in code, guarded by tests. Two deliberate choices: "Not recorded yet" rather than blank, and "Keep it" rather than "Cancel" next to "Remove". | 2026-09-21 |
| 13 | What shape should each field's input box be, and are there any length limits? | Description is a multi-line box that grows as you type; the other six are single-line. **No length limits, no dropdowns** — offering a list of sensitivity levels or update frequencies would quietly become the district's list by accident. | 2026-09-21 |
| 14 | How do people move between the pages? | **A menu that slides out** — the standard MudBlazor drawer with a menu button, holding links to the list of data sources and to removed data sources. Chosen over a plain link layout; scales if the catalog grows more sections. | 2026-09-21 |
| 15 | What happens if someone leaves a half-filled form? | **Ask before discarding.** If anything has been typed, a box offers to keep editing or discard. If nothing has been typed, it leaves without fuss. Applies to Cancel and to navigating away. | 2026-09-21 |
| 16 | What happens when someone opens a link to a data source that is removed, or never existed? | **Removed** — the page still opens, showing everything it held, with a notice at the top and a "Put back" button. No editing until it is put back. **Never existed** — "That data source could not be found" with a link to the list. | 2026-09-21 |
| 17 | What exactly does the history record, and is it ever cleared? | **Everything, kept forever, newest first.** Records added, each field changed with before and after values, removed, and put back. Shown on the data source's page. Never trimmed — a history that quietly deletes itself is worse than none. | 2026-09-21 |
| 18 | This changes the shape of the catalog file. Set up migrations now, or recreate while empty? | **Install the reshaping tool (`dotnet-ef`) into the project folder now** and do it properly, while the catalog is provably empty and the change is free. Explicitly approved as a hard-stop exception; recorded in the approval log. | 2026-09-21 |
| 19 | What happens if saving itself fails? | | 2026-09-21 |

---

## The Plan

_Version 1 — 2026-09-21_

### What you will be able to do when this is finished

You will be able to add a data source, see all of them in a list, open one to read everything about it, correct it, and remove it. Removing hides it rather than destroying it, and you can put it back. Every change is recorded with the date and time and what it changed from, so you can always see how an entry got the way it is.

The catalog starts empty. The first entry will be one you type.

### What changes that you can see

Five pages, reached through a menu that slides out from a button at the top:

| Page | What is on it |
|---|---|
| **Data Catalog** (the front page) | The list: name, system, owner, sensitivity. A button **Add a data source**. When empty, the wording you already have |
| **Add a data source** | The seven boxes, **Save** and **Cancel** |
| **A data source** | All seven fields, its **History**, and buttons **Edit**, **Remove**, and a link back |
| **Edit** | The same seven boxes filled in, **Save changes** and **Cancel** |
| **Removed data sources** | Everything removed, each with **Put back** |

Blank fields read **Not recorded yet** rather than sitting empty, so a gap looks like a gap.

### What happens step by step

1. You open the catalog and see the list, sorted by name.
2. You press **Add a data source** and fill in as much as you know. Only the name is needed.
3. You press **Save**. You go back to the list, which now includes it, and a brief **Saved** appears.
4. Clicking any row opens that data source, showing all seven fields and its history.
5. **Edit** opens the same boxes filled in. **Save changes** records what changed.
6. **Remove** asks to confirm, then takes it off the list.
7. **Removed data sources** in the menu shows what has been removed, and **Put back** returns it.

### When things go wrong

| If this happens | The person sees | Do they lose their work? |
|---|---|---|
| They save with the name blank, or only spaces | The name box outlines red with **Please give this data source a name** | No. Everything else they typed stays |
| The name already exists | **A data source called "X" already exists.** with **View it** and **Save anyway** | No. They choose |
| They press Cancel or navigate away with something typed | A box asking whether to discard, with **Keep editing** and **Discard** | Only if they choose to discard |
| They press Cancel having typed nothing | It just leaves | Nothing to lose |
| The save itself fails — disk full, file locked by a backup | **This data source could not be saved. Nothing you typed has been lost — please try again.** with the technical detail underneath | No. Everything stays on screen, so Save works once the problem clears |
| They open a link to a removed data source | The page opens with a notice that it was removed, and **Put back**. No editing until it is back | No |
| They open a link to something that never existed | **That data source could not be found** and a link to the list | No |
| The same entry is saved from two places | The last save wins, silently. The earlier values remain visible in the history | Recoverable from the history |
| They remove something by mistake | It is on **Removed data sources** and **Put back** returns it | No |

The wording in rows 5 and 7 is drafted rather than chosen by you — approving this plan approves it. Everything else is from question 12.

### What gets tested

**Rules, without a browser:** a name of blanks or spaces is refused; an entry saves with only a name; the other six fields accept being empty; a duplicate name is reported but permitted; removing hides rather than deletes; putting back restores; the history records adding, each field change with its before and after value, removing, and putting back; history is newest first; nothing is ever trimmed.

**Awkward content, as in 001:** 10,000 characters, quotes and apostrophes, accented letters and emoji, text shaped like a database command, text that looks like web page code — all stored and displayed as plain text, never run.

**The pages, without a browser:** the list shows the four columns; an empty catalog shows the existing wording; blank fields show **Not recorded yet**; a removed entry shows its notice and no edit button.

**In a real browser:** add a data source and see it appear; open it; edit it and see the change and the new history line; remove it and see it leave the list; find it under removed and put it back; try to save with no name and see the message; leave a half-filled form and see the discard question.

### New packages needed

| What | Version | What it does, in plain English | Why we cannot do this without it |
|---|---|---|---|
| `dotnet-ef` (a tool, installed into the project folder) | current 10.x, pinned and recorded | Reshapes the catalog file safely when fields are added | Approved at question 18. Without it, every future field change risks the data already in the catalog |

No new NuGet packages beyond that. The history and the removal marker are built with what is already approved.

### Structure and documents this affects

This request introduces the **services layer** that `docs/ARCHITECTURE.md` describes but which has not existed until now — the rules about names, duplicates, removal and history belong there, not in the pages and not in storage.

- `DataCatalog.Core` — a second model for a recorded change, and a service holding the rules
- `DataCatalog.Infrastructure` — the removal marker, the history table, and the first real migration
- `DataCatalog.Web` — five pages and the slide-out menu
- `docs/ARCHITECTURE.md`, all four area guides, and `docs/DECISIONS.md` updated in the same work

### Branch

`feature/002-add-remove-display-data-sources`, created after this approval. Left unmerged when finished; merging is yours to ask for.

### What is NOT included

- **Searching or filtering** — its own request, once there is enough to search
- **The district's sensitivity levels** — still free text, still undecided
- **Who made a change** — the history records what and when, never who, because there are no logins
- **Permanently deleting** anything — removed means hidden, always recoverable
- **Warning two people who edit at once** — last save wins. This must be revisited before the catalog ever goes on a server
- **Any starter or example entries** — the catalog starts empty
- **District colors, logo or fonts**

### How this gets undone

The branch is abandoned and nothing changes. If already merged, the catalog reverts to the single page of request 001 — and because the catalog file's shape changes here, anything added in the meantime would be lost. That is the one thing worth knowing before approving.

### How long, roughly

The largest piece of work so far — five pages, a new layer, a schema change, and around sixty tests. Longer than 001. The riskiest part is the migration tooling, which has never been run in this project; if it misbehaves I will stop and say so rather than working around it.

---

## Approval

**Approved by:**
**Date:**
**Their exact words:**
**Plan version approved:**

_No text in this section means no code may be written._

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
