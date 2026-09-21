# Glossary

Plain-English definitions of every technical term the Product Owner may encounter. **When an agent uses a term that is not here, it defines it on the spot and adds it to this file.**

If the PO has to ask what a word means, the agent broke principle P18.

---

**Approval block** — the few lines written into a request file recording who approved a plan, on what date, and in what words. Work without one is not allowed to start.

**Blazor Server** — the way this app is built. The pages run on the computer hosting the app rather than inside the browser, and the browser shows the results. It means the app works without installing anything.

**bUnit** — a testing tool that checks one piece of a screen on its own, quickly, without opening a browser.

**Catalog entry** — one record in the catalog, describing one data source.

**csproj** — the file that lists what a piece of this project is made of, including every add-on it uses. Agents check it against the package ledger.

**Data source** — a system that holds district information: the student information system, the gradebook, the attendance system, and so on.

**Dependency** — see *package*.

**Edge case** — an unusual or awkward situation that still has to work correctly: a blank field, a duplicate name, an enormous amount of text, two people saving at the same time.

**FERPA** — the US federal law protecting student education records. It is why no real student data is ever allowed in this project.

**Happy path** — the normal case, where everything goes right. Only ever part of the testing, never all of it.

**localhost** — an address that only works on the computer the app is running on. Nobody else can reach it.

**MudBlazor** — a free collection of ready-made screen pieces — buttons, tables, forms, menus — so the project does not design every control from scratch.

**.NET 10** — the version of Microsoft's software platform this project is built on.

**NuGet package** — see *package*.

**Package** — a piece of software written by someone else that the project uses rather than building the same thing itself. Useful, but permanent: each one must be maintained and kept up to date. This is why every package needs the PO's approval.

**Playwright** — a testing tool that opens the real app in a real browser and clicks through it the way a person would, automatically.

**Plan** — the plain-English description of what will be built, written before any code and approved by the PO.

**Product Owner (PO)** — the person who decides what gets built. Not a software developer, and never required to be.

**Regression** — something that used to work and has stopped working. Running the whole test suite every time is how these get caught.

**Request** — one thing the PO asked for. Each gets a numbered file in `requests/`.

**Test / test suite** — an automatic check that part of the app behaves correctly. The suite is all of them together, run with one command.

**Unit test** — a test of one small piece of logic on its own.

**UI test** — a test that drives the actual screens the way a person would.

**Validation** — the checks made on what someone types in: is it filled in, is it the right kind of thing, is it too long.

**xUnit** — the tool that runs the project's automated checks.

---

## Terms an agent may use about how code is written

You will not usually need these, but they may appear in a plan.

**DRY** — short for "do not repeat yourself". The idea that each rule should be written down in exactly one place in the software, so that changing it changes it everywhere. When the same rule is copied into several places, someone eventually updates only some of them, and the software starts contradicting itself.

**SOLID** — a set of five long-standing guidelines for organizing software so that it can be changed later without breaking. In practice it means each piece of the software does one job, and adding something new means adding a new piece rather than editing many old ones. The reason you may hear it mentioned: it is what makes the difference between "we can add that field next week" and "that touches everything".

**KISS** — "keep it simple". Of two ways to build something, prefer the one that is easier to explain.

**YAGNI** — "you are not going to need it". Build what you asked for and nothing else. Anything extra is a guess about the future, and guesses cost money to maintain whether or not they turn out to be right.

**Refactoring** — rearranging existing software without changing what it does, usually to make it tidier. In this project an agent may not do this to code you did not ask it to touch; it must tell you and let you decide.

**Technical debt** — shortcuts taken in the software that make future changes slower. Agents should tell you when they are taking one and why.

---

## Terms about how changes are kept track of

**Branch** — a private copy of the project where an agent builds one feature. Work on a branch cannot affect the version you have been using until it is deliberately merged. If you do not like a feature, the branch is simply abandoned and nothing needs undoing.

**Main (or main line)** — the real version of the project. Everything you have approved lives here. Agents are not allowed to build directly on it.

**Merge** — taking a finished branch and folding it into the main line, making it part of the real version. In this project **only you decide when this happens** — an agent stops when the work is done and waits for you to ask (D-014).

**Commit** — a saved checkpoint of work, with a short note about what changed. Many commits make up a branch.

**Git** — the software that keeps track of all of the above, including the complete history of every change ever made.

**Architecture** — how the pieces of the software are arranged and which pieces are allowed to talk to which. Kept written down so that it stays deliberate rather than accidental.

**Area guide** — a short written explanation kept inside each part of the software, saying how that part works and how to add to it safely.
