# NuGet Package Policy

Every package added to this project is a permanent piece of the district's software: someone must maintain it, keep it patched, and understand it years from now. The Product Owner owns that decision, not the agent.

> **No NuGet package is installed, referenced, restored, or upgraded until it has been presented in plain English, approved by name and version, and written into `approvals/PACKAGE_APPROVALS.md` with who approved it and when.**

This applies without exception to:

- packages you add directly
- **test** packages
- packages that arrive as a dependency of another package
- **upgrading** a package already approved at a different version
- packages a tutorial, template, or scaffolding command adds on your behalf
- packages you believe are obviously standard

If `dotnet new`, a template, or any tool adds a package you did not get approved, that is a stop — tell the PO what was added and get it approved or removed before continuing.

---

## The procedure

### 1. Try to avoid it first
Before asking, check whether .NET 10, ASP.NET Core, or MudBlazor already does this. State in your request what you checked. "We could do this ourselves in about 40 lines" is a real option and the PO may prefer it.

### 2. Present it during refinement, before the plan
A package is a Stage 2 question, not a Stage 5 surprise. Ask in plain English:

> "To let people export the catalog to a spreadsheet, I'd like to use a free add-on component called **ClosedXML**, version 0.104.2. It creates Excel files — writing that from scratch would be weeks of work. It's open source, widely used, and free for us. Adding it means it becomes part of the project permanently and needs occasional updating. May I use it?"

Never ask like this:

> "I'll add ClosedXML for the export."

### 3. Put it in the plan
Every package goes in the plan's "New packages needed" table with:

| Field | Meaning |
|---|---|
| Package | Exact package id |
| Version | Exact version — never "latest" |
| What it does | Plain English, no jargon |
| Why we can't do without it | What it would cost to build ourselves |
| License | And whether it is free for a public school district to use |
| Alternatives considered | Including "write it ourselves" |
| What it drags in | Other packages it brings with it, by name |

### 4. Get approval and record it
Approving the plan approves its packages. On approval, before installing anything, add a row to `approvals/PACKAGE_APPROVALS.md` with the approver's name and the date, and a line in `approvals/APPROVAL_LOG.md`.

### 5. Install exactly what was approved
Pin the exact approved version in the `.csproj`. If the version you install differs from the version approved — for any reason — stop and ask.

### 6. Verify
Before reporting done, compare every `PackageReference` in every `.csproj` against the ledger. Any package in the project without a row is a stop. Report the mismatch immediately.

---

## Things that make a package a bad idea

Raise these to the PO if you notice them:

- Not updated in over two years, or few users
- A license that is not clearly free for a public school district (anything other than MIT, Apache 2.0, BSD, or similar — a "community edition" with commercial terms is a stop)
- Brings a long chain of other packages with it
- Does far more than what is needed
- Requires sending anything outside the local machine
- A paid tier, a trial, or a license key

## Already approved

The founding set lives in `approvals/PACKAGE_APPROVALS.md`. Read it before asking — a package already on it at the approved version needs no new approval.
