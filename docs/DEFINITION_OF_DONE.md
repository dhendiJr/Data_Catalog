# Definition of Done

Work is "done" only when **every** line below is true. Not most. Every one.

If you cannot tick a line, the work is not done — say exactly which line and why.

## Approval
- [ ] A written plan for this work exists in `requests/`
- [ ] That plan carries an approval block with the approver's name, the date, and their words
- [ ] A matching line exists in `approvals/APPROVAL_LOG.md`
- [ ] What was built matches what was approved — nothing extra, nothing missing
- [ ] Anything that diverged from the plan was raised with the PO and re-approved

## Packages
- [ ] Every NuGet package used appears in `approvals/PACKAGE_APPROVALS.md` with an approver and a date
- [ ] No package was added that was not in the approved plan
- [ ] The version in the `.csproj` matches the version in the ledger

## The feature
- [ ] It does what the plan said it would do
- [ ] Every error case in the plan's "When things go wrong" table behaves as described
- [ ] Every message the person sees matches the wording the PO chose
- [ ] Empty states — nothing there yet, no results — look the way the plan described

## Tests
- [ ] xUnit tests cover the logic, including the applicable rows of the edge case matrix
- [ ] bUnit tests cover any component with behavior
- [ ] A Playwright test covers each visible change, including at least one failure path
- [ ] Each new test was observed failing before it passed
- [ ] No existing test was changed, skipped, relaxed, or deleted
- [ ] `dotnet test` was run across the whole solution, not just the new tests
- [ ] Every test passes

## Code quality
- [ ] No knowledge is duplicated: any rule, list, or piece of logic I wrote exists in exactly one place (P21)
- [ ] Every class or component I wrote has one job, describable without the word "and" (P22)
- [ ] No business rule lives inside a `.razor` file; components do not talk to storage directly (P22)
- [ ] Dependencies are injected interfaces, not concrete classes constructed inside the caller (P22)
- [ ] I built nothing the plan did not ask for — no spare options, no extension points (P24)
- [ ] I changed no existing code the plan did not name; anything I noticed is listed, not fixed (P25)
- [ ] Every name says what the thing is; no comment explains what the code already says

## Structure, names, and records
- [ ] Work was done on a branch named `feature/NNN-short-name`, never on `main` (P29)
- [ ] The branch is unmerged and the PO has been told it is waiting for their decision (D-014)
- [ ] Every name follows `docs/NAMING_CONVENTIONS.md` — full words, one way per kind of thing (P27)
- [ ] Wording the PO chose is used everywhere: screen, code, tests, glossary (P27)
- [ ] `docs/ARCHITECTURE.md` matches what is actually on disk (P26)
- [ ] Every new folder or area has a `README.md` from `docs/AREA_GUIDE_TEMPLATE.md` (P28)
- [ ] Every area guide I touched is still true; any that was already wrong has been reported (P28)

## Reporting
- [ ] The PO has been shown the actual test output
- [ ] Any failure, shortcut, or incomplete part was stated plainly and not glossed over
- [ ] The PO has been told, in plain English, what they can now do and where to click to see it

## Record
- [ ] New decisions are written into `docs/DECISIONS.md` with the date
- [ ] Any new technical term the PO saw is defined in `docs/GLOSSARY.md`
- [ ] The request file is marked complete
- [ ] Out-of-scope observations are listed as "Noticed while building" — listed, not acted on

---

**The honesty rule:** never report done when this checklist is not satisfied. A half-finished feature described accurately is useful. A half-finished feature described as complete is a problem the PO will discover later, at a worse time.
