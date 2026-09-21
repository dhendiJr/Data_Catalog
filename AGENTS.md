# AGENTS.md — Read This First

**If you are an AI agent working in this project, this file is your starting point. Read it completely before you take any action — including reading other files, searching the web, or writing code.**

---

## 1. The rule that overrides everything

> **You may not create, edit, or delete a single line of project code until a written plan for the current request has been approved in writing by the Product Owner.**

There is no exception. Not for "small" changes. Not for typos. Not because the change seems obviously correct. Not because you are confident. If there is no approved plan on disk for what you are about to do, you stop and go get one.

## 2. Who you are working with

The Product Owner (PO) is **not a software developer**. They understand the school district, its staff, and what the catalog needs to do. They do not know what a migration, a dependency, a null reference, or a test harness is, and they should never need to.

This means:

- Everything you write for the PO is in plain English. No jargon. If a technical word is unavoidable, define it inline and add it to `docs/GLOSSARY.md`.
- You never ask the PO to make a decision by naming two technologies. You ask them about **outcomes and behavior**, and you translate their answer into technology yourself.
- You always give a recommendation. "What do you want?" is a bad question. "I suggest X because Y — is that right?" is a good one.
- The PO's silence is never approval. Only explicit written approval is approval.

## 3. Required reading

Read all of these before your first response in a session. They are short.

| File | What it is |
|---|---|
| `PROJECT_CHARTER.md` | What this project is and is not. |
| `docs/DESIGN_PRINCIPLES.md` | The standing rules. Non-negotiable. |
| `docs/WORKFLOW.md` | The exact loop you follow for every request. |
| `docs/QUESTION_PLAYBOOK.md` | What you must ask about before planning. |
| `docs/PLAN_TEMPLATE.md` | The format the PO reads and approves. |
| `docs/TESTING_STANDARDS.md` | What tests every feature must have. |
| `docs/CODE_STANDARDS.md` | How code is written here: DRY, SOLID, KISS, YAGNI, layering, naming. |
| `docs/NAMING_CONVENTIONS.md` | How everything in this project is named. |
| `docs/ARCHITECTURE.md` | How the project is put together. Living document — you keep it true. |
| `docs/BRANCHING.md` | Branch before you build. Never merge on your own. |
| `docs/AREA_GUIDE_TEMPLATE.md` | The guide every new area of the app must carry. |
| `docs/DEPENDENCY_POLICY.md` | How NuGet packages get approved. Read before touching a `.csproj`. |
| `docs/DEFINITION_OF_DONE.md` | What "finished" means here. |
| `docs/GUARDRAILS.md` | Hard stops and forbidden actions. |
| `docs/DECISIONS.md` | Everything already decided. Do not re-ask these. |
| `approvals/PACKAGE_APPROVALS.md` | The only packages allowed to exist in this project. |
| `approvals/APPROVAL_LOG.md` | Every approval ever given, with who gave it and when. |
| `docs/GLOSSARY.md` | Plain-English definitions for the PO. |

## 4. The loop, in one paragraph

The PO gives you a request. You ask **one question at a time** until nothing about the request is unknown — no assumptions, ever. You write a plan in plain English to `requests/NNN-short-name.md` and you stop. The PO approves it in writing. You record the approval in that file and in `approvals/APPROVAL_LOG.md`. You create a branch named for the request. You build exactly what was approved, with unit tests covering edge cases and a UI test for anything the PO can see. You run the whole test suite and show the results, including anything that failed. You update the architecture document and any area guide you affected, and record new decisions in `docs/DECISIONS.md`. Then you stop — the branch stays unmerged until the PO asks for it.

The long version is in `docs/WORKFLOW.md`. Follow it.

## 5. Hard stops

Stop and ask the PO before doing any of these. See `docs/GUARDRAILS.md` for the full list and the reasoning.

- Writing or changing any file outside this project folder
- Deleting anything the PO did not explicitly ask you to delete
- Adding, upgrading, or restoring **any** NuGet package — including test packages, and packages pulled in as a dependency of another package
- Changing how data is stored or structured
- Changing, weakening, or deleting an existing test
- Refactoring or tidying code the approved plan did not name — however obvious the improvement
- Merging, pushing, rebasing, force-pushing, deleting a branch, or discarding uncommitted work
- Building anything while on the `main` branch — create the feature branch first
- Putting real student, staff, or family data anywhere in this project
- Doing anything that was not in the approved plan

## 6. The technology, for your reference only

This project is **.NET 10, Blazor Server, and MudBlazor**, tested with **xUnit**, **bUnit**, and **Playwright**. It runs on one computer at localhost only, and it has no logins. Do not change or add to any of that without approval — see `docs/DECISIONS.md` D-005, D-006, D-008, D-009.

Never explain any of this to the Product Owner in these words. Translate to plain English, always.

## 7. When you are unsure

Unsure is a stop condition, not a judgment call. Say plainly what you do not know, ask one question about it, and wait. An agent that guesses is worse than an agent that pauses.

## 8. When you are wrong

If you discover that something you built or said is wrong, say so immediately and plainly, before continuing. Do not quietly fix it. Do not soften it. The PO needs an accurate picture more than they need you to look competent.
