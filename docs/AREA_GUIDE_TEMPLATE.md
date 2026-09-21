# Area Guide Template

Every meaningful part of the application carries its own `README.md`, sitting in the folder it describes. It explains how that part works and how to extend it without breaking it.

The point is that a future agent — or a future person — opening a folder for the first time can work in it correctly without reading the whole codebase and without guessing.

---

## When an area guide is required

Create one when you add:

- a new project
- a new layer or a new folder directly under a layer
- a new grouped area of components, such as `Components/Catalog/`
- anything with rules a newcomer could plausibly get wrong

Update the existing one when you change how the area works, what it depends on, or how to add to it. **A change to an area without a change to its guide, where the guide is now wrong, is not done.**

Do not create one for a folder holding two obvious files. A guide nobody needs is a guide nobody maintains.

## Rules for writing one

- Written for whoever arrives next, who knows nothing about this project.
- Short. One page. If it is growing past that, the area is probably doing too much.
- Concrete. Name the actual files and the actual rules, not general good advice.
- Say what must **not** be done here, and why. That is the part that saves people.
- Dated, so a reader can judge how stale it might be.

---

## The template

Copy everything below into `README.md` inside the folder.

```markdown
# <Area name>

_Last updated: YYYY-MM-DD — request NNN_

## What this area is for

One or two plain sentences. What job does this folder do, and what job does it deliberately not do?

## What lives here

| File | What it does |
|---|---|
| | |

## How it fits the rest of the project

Which layer this belongs to (see `docs/ARCHITECTURE.md`), what it depends on, and what depends on it. Name the interfaces it talks through.

## How to add something here

Numbered steps. The actual steps, for this area specifically — where the file goes, what it must implement, where it gets registered, what test it needs.

1. 
2. 
3. 

## The rules of this area

Things that are easy to get wrong here and must not be done:

- 
- 

For example, in a components area: no business rules in a `.razor` file. In a services area: no knowledge of MudBlazor. In a repositories area: no validation.

## How this area is tested

Which test project covers it, what kind of tests, and anything unusual about testing it — fakes that exist, setup that is needed, edge cases that have caught people out.

## Decisions that shaped this area

| Decision | What it means here |
|---|---|
| D-0NN | |

## Known rough edges

Anything a newcomer would otherwise waste an hour discovering. Shortcuts taken and why. Write these down honestly — an undocumented rough edge becomes a trap.
```

---

## Keeping guides honest

An area guide that has drifted from the code is actively harmful, because the next agent will believe it.

Two safeguards:

1. **The Definition of Done** requires checking the guide for every area you touched, and updating it if it has become wrong.
2. **When you find a guide that is already wrong**, say so plainly in your report. Correcting it is a small documentation change, and if it falls outside your approved plan, ask — but never leave a known-wrong guide in place silently.
