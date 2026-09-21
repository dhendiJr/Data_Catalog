# Decision Log

Everything the Product Owner has already decided. **Read this file in full before asking any question.** If a question is answered here, it is settled — do not re-ask it and do not contradict it.

This file is what makes "ask about everything" survivable: nothing is ever asked twice.

Add a new entry every time the PO answers a question that will matter again. Convert relative dates ("next month") to actual dates. Never delete an entry — supersede it, and leave the original visible with a pointer.

---

| ID | Date | Question | Decision | Status |
|---|---|---|---|---|
| D-001 | 2026-09-21 | Should the project's safety controls be enforced by tooling, or written as documentation? | Documentation only, tool-agnostic. Plain markdown any AI agent can read and follow — no tool-specific automation. The written record is therefore the only real control, which is why approvals must be filed. | Active |
| D-002 | 2026-09-21 | How is a Product Owner's approval recorded? | The agent writes the plan to `requests/NNN-name.md` and stops. The PO replies with the word APPROVED. The agent then writes the approver's name, the date, and their exact words into that file, plus a line in `approvals/APPROVAL_LOG.md`. **No approval block in the file means no code.** | Active |
| D-003 | 2026-09-21 | How many questions does the agent ask before writing a plan? | Ask everything. One question at a time, until nothing is unresolved. **No assumptions, ever** — there is no question budget and no point at which the agent starts guessing. Large question counts are a signal to propose splitting the request. | Active |
| D-004 | 2026-09-21 | Should the rulebook pin a technology stack? | Stay technology-neutral; decide the stack in the first planning session. | **Superseded by D-008** |
| D-005 | 2026-09-21 | Who can view the catalog, and who can change it? | No logins. Wiki-style: anyone who can reach the app can view and edit it. Because there is no way to tell who made a change, an **edit history is a requirement, not an option**, and catalog entries must never contain real data about people. | Active |
| D-006 | 2026-09-21 | Where does the catalog run? | On one computer, reachable only from that machine (localhost). Not on a server, not on the district network, not on the internet. Moving it anywhere else is its own separate request and would require revisiting D-005. | Active |
| D-007 | 2026-09-21 | What does the catalog do? | It is a searchable directory of the district's data sources — which systems exist, what is in each, who owns them, who to contact, how sensitive they are. Not a field-level data dictionary, not an access-request system, not a report inventory. | Active |
| D-008 | 2026-09-21 | What technology is the catalog built with? | **.NET 10, Blazor Server, and MudBlazor** for the screen components. This supersedes D-004: the stack is pinned, not neutral. Changing the .NET version, the Blazor hosting model, or the MudBlazor major version is a hard stop requiring approval. | Active |
| D-009 | 2026-09-21 | What testing tools are required? | **xUnit** for logic, **bUnit** for Blazor components, **Microsoft Playwright** for real browser tests of the running app. All three are approved founding packages. | Active |
| D-010 | 2026-09-21 | What does a catalog entry record? | Seven fields to start: name, plain-language description, the system it lives in, who owns it, who to contact, how sensitive it is, how often it updates. More fields can be added later through the normal request process. | Active |
| D-011 | 2026-09-21 | How are NuGet packages controlled? | Every package — including test packages, packages pulled in as dependencies of others, and version upgrades of already-approved packages — must be presented in plain English, approved by name and version, and recorded in `approvals/PACKAGE_APPROVALS.md` with who approved it and when. A package in a `.csproj` with no ledger row is a stop. See `docs/DEPENDENCY_POLICY.md`. | Active |
| D-012 | 2026-09-21 | Whose name appears on approvals? | Approvals are recorded as "Product Owner" rather than a personal name. The role may pass between people, so the record attributes the role, not the individual. Agents write "Product Owner" in every approval block and ledger row. | Active |
| D-013 | 2026-09-21 | Should code craft standards be written down as rules? | Yes. `docs/CODE_STANDARDS.md` defines DRY, SOLID, KISS, YAGNI, layering, naming, comments, and error handling, with the .NET and Blazor specifics. Summarised as principles P21 to P25. Critically, P25 states these apply only to code being written under an approved plan, and are never a licence to refactor untouched code. | Active |
| D-014 | 2026-09-21 | Who merges a finished branch into the main line? | The agent builds on a branch, runs the tests, reports, and **stops**. The branch stays unmerged. Merging is a separate thing the PO asks for after seeing the results. An unwanted feature is abandoned rather than unpicked, and `main` only ever contains work the PO accepted. | Active |
| D-015 | 2026-09-21 | Is a branch required for every feature? | Yes. A branch is created after approval and before the first line of code, named `feature/NNN-short-name` to match its request. Building on `main` is a hard stop. See `docs/BRANCHING.md`. | Active |
| D-016 | 2026-09-21 | How are naming and architecture governed? | `docs/NAMING_CONVENTIONS.md` governs every name in the project. `docs/ARCHITECTURE.md` is a living document describing the four layers and their one-way dependencies, and must be updated in the same change that alters the structure. Principles P26 and P27. | Active |
| D-017 | 2026-09-21 | How is each part of the application documented? | Every project, layer, and grouped area carries its own `README.md` from `docs/AREA_GUIDE_TEMPLATE.md`: what it is for, what lives there, how to add to it, what must never be done in it, how it is tested. Created when the area is created, updated in the same change that alters it. Principle P28. | Active |

---

## Still open

| Question | Why it matters | Waiting on |
|---|---|---|
| What should the catalog be called, on screen? | It is the page title and the name staff will use for it | The PO |
| What are the sensitivity levels, in the district's own words? | One of the seven entry fields; the values must match how the district already talks about data | The PO |
