# The Workflow

Every request follows these seven stages in order. You may not skip a stage, merge two stages, or run them out of order.

---

## Stage 1 — Intake

The PO describes something they want. Your first move is **not** to plan and **not** to code.

1. Read `docs/DECISIONS.md` in full. Anything already decided there is settled — do not re-ask it, and do not contradict it.
2. Assign the request the next number and create `requests/NNN-short-name.md` using `requests/000-TEMPLATE.md`.
3. Write down the request in the PO's own words, verbatim, in the "What was asked for" section.
4. Reply confirming in one or two plain sentences what you understand the request to be, then go straight to Stage 2.

## Stage 2 — Refinement (one question at a time)

Work through `docs/QUESTION_PLAYBOOK.md`. Ask **one** question, wait for the answer, record it in the request file, then ask the next.

Each question must:

- be a single question about a single thing
- be answerable by someone who does not write software
- say in one line why the answer matters — what changes depending on the answer
- offer a recommended answer, so the PO can agree rather than invent
- avoid naming technologies; ask about behavior and outcomes instead

Keep going until nothing about the request is unresolved. **There is no point at which you start assuming.** If the questions are piling up, your next question should be: *"This is turning into a large request. May I split it into these smaller pieces and start with the first one?"*

Before you stop asking, re-read the request and check: is there any behavior in it that the PO has not explicitly described? If yes, you are not finished asking.

## Stage 3 — Plan

Write the plan into the request file using `docs/PLAN_TEMPLATE.md`. Then **stop and say you are stopping.**

The plan must be readable start to finish by a non-developer. It must include what will change from the PO's point of view, what happens when things go wrong, what will be tested, any NuGet packages needed (see `docs/DEPENDENCY_POLICY.md`), what is explicitly *not* included, and how to undo it.

Your message to the PO ends with a direct request for approval and nothing else. Do not begin work "while waiting."

## Stage 4 — Approval

The PO replies. Only the word **APPROVED** (or an unmistakable equivalent, such as "approved" or "yes, approved") counts. "Looks good", "sure", "sounds fine", and silence do **not** count — if you get one of those, ask directly: *"To be clear, is that approval to start building? I need the word approved."*

When approval arrives:

1. Write the approval block into `requests/NNN-short-name.md`: approver's name, the date, and the exact words they used.
2. Add a line to `approvals/APPROVAL_LOG.md`.
3. If the plan included NuGet packages, add each one to `approvals/PACKAGE_APPROVALS.md`.

If the PO asks for changes instead, revise the plan and return to Stage 3. A revised plan needs a fresh approval; the old one does not carry over.

## Stage 5 — Build

**First, create the branch.** Check where you are with `git branch --show-current`. Create `feature/NNN-short-name`, matching the request number and name, and switch to it. Never write code on `main` — see `docs/BRANCHING.md`.

Build exactly what the approved plan describes.

- Write the tests alongside the feature, not afterward (`docs/TESTING_STANDARDS.md`).
- Keep changes small and reversible.
- If you hit anything the plan did not anticipate — a step that does not work, a question you missed, a package you now need — **stop**. Explain it plainly, ask about it, and get the plan amended and re-approved. Do not solve it on your own initiative.
- Touch nothing outside what the plan describes.

## Stage 6 — Verify

Run the full test suite. Not the tests you just wrote — all of them.

Report to the PO:

- what now works, described as something they can see or do
- the actual test results, in full, including anything that failed
- anything that did not go to plan

If anything failed, the work is not done. Say so plainly and fix it, or ask if the fix is outside the approved plan.

## Stage 7 — Record and close

1. Add any new decisions from Stage 2 to `docs/DECISIONS.md`, with the date.
2. Mark the request file complete.
3. Add a short "Noticed while building" list of anything worth doing that was outside scope. Do not act on it. It is the PO's to turn into a new request.
4. Update `docs/ARCHITECTURE.md` if the structure changed, and the `README.md` of every area you touched (`docs/AREA_GUIDE_TEMPLATE.md`). If you found a guide that was already wrong, say so.
5. Leave the branch unmerged and tell the PO it is waiting. Merging is theirs to ask for (`D-014`).
6. Stop. Wait for the next request.

---

## The stop conditions, collected

Stop and ask whenever any of these is true. Stopping is always the correct choice.

- There is no approved plan for what you are about to do
- The plan does not cover the situation in front of you
- You need a NuGet package that is not in the approval ledger
- Something in the approved plan turns out to be wrong or impossible
- A test fails and the fix is not clearly inside the approved plan
- You are about to do anything on the hard-stop list in `docs/GUARDRAILS.md`
- You are about to write code and `git branch --show-current` says `main`
- You are about to merge, push, rebase, delete a branch, or discard uncommitted work
- A folder you added has no area guide, or an area guide you relied on turned out to be wrong
- You are unsure
