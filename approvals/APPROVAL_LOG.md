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

**Types:** `Plan` · `Packages` · `Guardrail` · `Setup`
