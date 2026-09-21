# Testing Standards

Tests are part of a feature, not a follow-up. A feature without tests is not finished, and is not reported as finished.

## The toolchain

| Tool | What it tests | When you use it |
|---|---|---|
| **xUnit** | Plain C# logic — rules, validation, calculations, services | Every piece of logic |
| **bUnit** | Blazor components in isolation, without a browser | Every component with behavior |
| **Playwright** | The real running app in a real browser | Every change the PO can see |

All three are NuGet packages and are pre-approved as founding entries in `approvals/PACKAGE_APPROVALS.md`. Any additional testing package needs its own approval like any other.

## Project layout

```
src/                          the application (.NET 10, Blazor Server, MudBlazor)
tests/
  UnitTests/                  xUnit — logic
  ComponentTests/             bUnit — Blazor components
  UiTests/                    Playwright — the whole app in a browser
```

## Commands

```
dotnet test
```

runs everything. Playwright browsers install once with `pwsh bin/Debug/net10.0/playwright.ps1 install`.

Stage 6 of the workflow means running `dotnet test` and pasting the real result, including failures.

---

## The edge case matrix

Every feature that accepts input must have a test for each row that applies. If a row does not apply, say why in the plan — do not silently omit it.

| Case | What to test |
|---|---|
| **Happy path** | The normal thing works |
| **Empty** | Blank field, empty list, zero results, brand-new install with nothing in it |
| **Missing** | Required value absent; the record being edited no longer exists |
| **Null** | Nulls where the type allows them, so nothing crashes with a null reference |
| **Whitespace** | A field containing only spaces or tabs is not treated as filled in |
| **Duplicate** | Adding something that already exists, by exact match and by case difference |
| **Too long** | Text far longer than expected — 10,000 characters in a description |
| **Too short** | One character where something meaningful was expected |
| **Wrong type** | Letters where a number or date belongs |
| **Boundary** | Exactly at a limit, one under, one over — 0, 1, max, max+1 |
| **Special characters** | Apostrophes, quotes, angle brackets, emoji, accented letters, right-to-left text |
| **Injection-shaped input** | Input that looks like code or a database command is stored and displayed as plain text, never executed |
| **Concurrency** | Two people changing the same record at once behaves as the PO specified |
| **Failure** | When saving or loading fails, the person is told and their typed work is not lost |
| **Large volume** | Behavior with many records — the list, the search, the page |

## Rules

1. **One behavior per test.** The test name says what it checks in plain words: `AddingEntryWithBlankNameShowsValidationMessage`.
2. **Watch it fail first.** Confirm a test can fail before you make it pass. A test that has never failed proves nothing.
3. **Never weaken a test to get to green.** Do not edit, skip, relax, or delete a failing test. If you believe the test itself is wrong, stop and ask the PO.
4. **Tests are independent.** Any test can run alone, in any order, repeatedly, with the same result.
5. **No real data.** Test data is invented and obviously fake. "Northside Elementary", "Chris Example", "chris@example.test". Never a real student, family, or staff member — see `docs/GUARDRAILS.md`.
6. **Test through the front door.** Test what the feature does, the way a person would experience it — not its private internals.

## UI tests specifically

Every change the PO can see needs a Playwright test that:

- opens the real app in a browser
- finds elements the way a person would — by visible label or text, not by internal identifiers
- performs the action and checks what appears on screen
- includes at least one failure path, not only the happy path

MudBlazor components render their own markup; target the visible text and roles, so tests survive a MudBlazor version change.

## What "tests pass" means

It means you ran `dotnet test`, every test ran, every test passed, and you are showing the PO the output. It does not mean you expect them to pass.
