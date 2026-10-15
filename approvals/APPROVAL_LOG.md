# Approval Log

Every approval in this project, in order. Plans, packages, and anything from the hard-stop list in `docs/GUARDRAILS.md`.

An approval that is not written here did not happen. If you are about to rely on an approval and cannot find its line, stop and ask the PO to confirm it again.

Add a row the moment approval is given — before starting the work, not after finishing it.

| # | Date | What was approved | Type | Approved by | Their exact words | Where it is recorded |
|---|---|---|---|---|---|---|
| 1 | 2026-09-21 | Project rules, workflow, and charter as refined in the setup conversation | Setup | Product Owner | See `docs/DECISIONS.md` D-001 to D-012 | `docs/DECISIONS.md` |
| 2 | 2026-09-21 | Founding NuGet packages: MudBlazor, xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, bunit, Microsoft.Playwright | Packages | Product Owner | Stack and test tooling chosen during setup | `approvals/PACKAGE_APPROVALS.md` |
| 3 | 2026-09-21 | Request 001 project setup plan, version 1 | Plan | Product Owner | "approved" | `requests/001-project-setup.md` |
| 4 | 2026-09-21 | Microsoft.EntityFrameworkCore.Sqlite and Microsoft.EntityFrameworkCore.Design, plus Microsoft.EntityFrameworkCore as their dependency | Packages | Product Owner | Approved as part of the request 001 plan | `approvals/PACKAGE_APPROVALS.md` |
| 5 | 2026-09-21 | The startup wording drafted during request 001 without prior approval | Plan | Product Owner | "the wording is fine" | `requests/001-project-setup.md` Q15, `docs/DECISIONS.md` D-023 |
| 6 | 2026-09-21 | Merging `feature/001-project-setup` into `main` | Guardrail | Product Owner | "merge it" | `requests/001-project-setup.md` |
| 7 | 2026-09-21 | Request 002 plan, version 1 — add, display, edit and remove data sources | Plan | Product Owner | "approved" | `requests/002-add-remove-display-data-sources.md` |
| 8 | 2026-09-21 | Installing the `dotnet-ef` tool into the project folder — an explicit exception to the hard stop on installing software | Guardrail | Product Owner | Approved at request 002, question 18 | `approvals/PACKAGE_APPROVALS.md` |
| 9 | 2026-09-21 | Merging `feature/002-add-remove-display-data-sources` into `main` | Guardrail | Product Owner | "merge it" | `requests/002-add-remove-display-data-sources.md` |
| 10 | 2026-10-15 | Request 003 plan, version 1 — publish the project to a public GitHub repository under the MIT licence, copyright California Schools Information Services (CSIS) | Plan | Product Owner | "appoved" (typing slip for approved) | `requests/003-publish-to-github.md` |
| 11 | 2026-10-15 | Adding a git remote, pushing to it, and publishing this project outside this computer — three hard stops, crossed knowingly | Guardrail | Product Owner | Approved as part of the request 003 plan | `requests/003-publish-to-github.md` |
| 12 | 2026-10-15 | Keeping the repository on the `dhendiJr` account rather than the work account, after two accounts were found | Guardrail | Product Owner | Chose "dhendiJr - where I already made it" | `docs/DECISIONS.md` D-036 |
| 13 | 2026-10-15 | Merging `feature/003-publish-to-github` into `main` and pushing again, so the licence and corrected README are public | Guardrail | Product Owner | Chose "Yes, merge and push" | `requests/003-publish-to-github.md` |

**Types:** `Plan` · `Packages` · `Guardrail` · `Setup`
