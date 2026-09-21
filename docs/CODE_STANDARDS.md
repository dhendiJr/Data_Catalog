# Code Standards

How code in this project is written. These are not style preferences — they are what keeps the catalog changeable a year from now, when a different person or a different agent has to add a field to it.

The Product Owner never reads this file. It exists so that "write it well" is a rule with a definition rather than a hope.

---

## The rule that sits above all of these

> **Apply these standards to code you are writing right now. Do not refactor existing code to satisfy them.**

This is where good engineering instinct collides with `P7 — scope is a ratchet, not a slope`. Seeing duplication in code you were not asked to touch is **not** permission to fix it. Note it under "Noticed while building" and let the PO decide.

The one exception: if the approved plan says you are changing a piece of code, you write *that* piece to these standards.

---

## DRY — Do not repeat yourself

**What it means:** every piece of knowledge in the system has one home. If the rule "a data source name cannot be blank" is written in three places, then one day it will be changed in two of them, and the third becomes a bug nobody can explain.

**What it looks like here:**
- Validation rules live in one place and are used by the form, the service, and the tests.
- The list of sensitivity levels is defined once, not repeated in a dropdown, a filter, and a display label.
- Shared display logic becomes a Blazor component used in several places rather than markup copied between pages.

**When NOT to apply it — read this part:**

DRY is about *knowledge*, not about *text that looks similar*. Two pieces of code that happen to look alike today, for unrelated reasons, are not duplication — merging them couples two things that need to change independently, which is worse than the duplication was.

**Rule of three:** the first time, write it. The second time, note the similarity and write it again. The third time, and only if it is genuinely the same rule rather than a coincidence, factor it out. Premature abstraction is a heavier cost than a little repetition.

---

## SOLID

Five principles for structuring code. Each one below is stated plainly and tied to what it actually means in a .NET and Blazor project.

### S — Single responsibility

**Each class, service, and component has one job, and one reason to change.**

A page component that loads data, validates it, saves it, and formats it for display has four reasons to change and will break in four ways. Split it: the component shows things and captures clicks, a service holds the rules, a repository talks to storage.

*A good smell test:* if describing what a class does needs the word "and", it probably has two jobs.

### O — Open for extension, closed for modification

**Adding a new case should mean adding new code, not editing a stack of old code.**

If adding a new sensitivity level means editing six `switch` statements scattered across the project, the design is wrong — and worse, in this project that edit touches six files nobody approved you to touch. Structure so that new cases plug in.

*Practical form:* prefer a table, a map, or a polymorphic type over a `switch` that will keep growing.

### L — Substitutability (Liskov)

**Anything implementing an interface must actually behave the way that interface promises.**

If `ICatalogRepository.GetAll()` returns entries, no implementation of it may return null instead, throw where others return empty, or quietly reorder results. Code that depends on the interface must not need to know which implementation it got — that is the entire point of having the interface.

This matters most in testing: a fake repository used in a test must behave like the real one, or the tests prove nothing about the real thing.

### I — Interface segregation

**Small, focused interfaces beat one large one.**

An interface with twelve members forces every implementation — including the fakes in your tests — to supply twelve things, most of which are irrelevant. If your test doubles are full of "not implemented" stubs, the interface is too big.

### D — Depend on abstractions, not concrete things

**Components and services depend on interfaces, supplied through .NET dependency injection, not on concrete classes they create themselves.**

A component that constructs its own data access cannot be tested without real storage. A component handed an `ICatalogRepository` can be tested in bUnit with a fake, which is exactly why this project can test components without a database.

*Practical form:* inject services into components; use constructor injection in services; register them in `Program.cs`. Never construct a service inside a component.

---

## KISS — Keep it simple

**The simplest thing that fully satisfies the approved plan wins.**

Clever code is code the next person has to decode. Given two approaches that both work, take the one that can be explained in one sentence. If you cannot explain in plain English what a method does, that is a signal — the same signal `P18` applies to the PO.

## YAGNI — You are not going to need it

**Build what the approved plan describes. Do not build for imagined futures.**

No configuration options nobody asked for. No extension points for features that do not exist. No "we might want this later." Later is a new request, refined and approved like everything else. In this project YAGNI is not merely good practice — building unrequested things is a direct violation of `P7`.

## Separation of concerns

The project has clear layers, and code stays in its layer:

| Layer | Job | Must not |
|---|---|---|
| **Components** (`.razor`) | Show things, capture clicks | Contain business rules or talk to storage directly |
| **Services** | Business rules, validation, decisions | Know anything about screens or MudBlazor |
| **Repositories** | Reading and writing stored data | Contain business rules |
| **Models** | The shape of a catalog entry | Depend on any of the above |

If a rule needs to be tested without a browser, it belongs in a service. Business logic inside a `.razor` file is a design error, not a shortcut.

## Naming

- Names say what a thing is or does, in full words. `sensitivityLevel`, not `sl`. `CatalogEntryService`, not `Helper`, `Manager`, or `Utils`.
- A name that needs a comment to explain it is the wrong name.
- Booleans read as a statement: `isArchived`, `hasContact`.
- Test names describe the behavior in plain words: `SavingEntryWithBlankNameShowsValidationMessage`.

## Comments

Comments explain **why**, never **what**. The code already says what it does. If code is unclear enough to need a "what" comment, fix the code instead.

The exception worth writing: a comment recording a decision the PO made, with its decision ID — such as a note that the second save wins by design, referencing D-0xx — because that is knowledge no reader can recover from the code itself.

## Error handling

- Never swallow an error silently. Ever.
- Never catch a broad exception just to make a problem go away.
- Every error a person can see must match the wording in the approved plan (`P9`).
- If something fails in a way the plan did not describe, that is a stop, not an improvisation.

---

## Checking your work

Before reporting done, ask yourself:

1. Did I copy and paste anything? Is it the same *knowledge*, or just similar-looking text?
2. Does any class I wrote need the word "and" to describe?
3. Does any component I wrote contain a business rule?
4. Did I build anything the plan did not ask for?
5. Could someone unfamiliar read this and follow it without me?
6. **Did I change any existing code that the plan did not name?**

Question 6 is the one that gets agents into trouble. The honest answer to it is usually where a rule got broken.
