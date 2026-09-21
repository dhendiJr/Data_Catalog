# The Question Playbook

This is the checklist you work through during Stage 2 refinement. Every category below must be resolved before you write a plan. If a category genuinely does not apply to the request, say so out loud in the request file and why — do not silently skip it.

**One question at a time. No assumptions, ever.**

---

## How to ask a good question

A good question to this Product Owner:

- asks about **one** thing
- describes **behavior**, never technology
- says **why it matters** in one line
- offers a **recommended answer** so agreement is possible
- is short enough to answer without a meeting

> **Bad:** "Should we use optimistic concurrency or row versioning for conflicting edits?"
>
> **Good:** "If two people open the same catalog entry and both save changes, whose version should win? I suggest the second person gets a message saying someone else just changed it, so nobody's work disappears silently. Does that sound right?"

> **Bad:** "What validation rules do you want on the fields?"
>
> **Good:** "If someone leaves the contact person blank when adding a data source, should the entry save anyway? I suggest yes, so people can get a rough entry in quickly and fill it in later — the entry would just show 'no contact listed'. Is that what you'd want?"

---

## Category 1 — What is actually being asked for

- In one sentence, what will a person be able to do that they cannot do today?
- Who is that person? A teacher, a data steward, a principal, an administrator?
- Is this one thing or several things? (If several, ask about splitting it.)
- How will the PO know it worked? What would they click to see it?

## Category 2 — Every error and failure path

This is the category agents most often skip. Do not skip it.

For each step a person takes, ask what should happen when it goes wrong:

- What if they leave something blank?
- What if they type something that does not make sense — text where a date belongs, a date in the year 1823?
- What if they enter something that already exists — a duplicate name?
- What if they type far more text than expected — a 10,000 character description?
- What if the thing they are looking for does not exist, or was deleted a moment ago?
- What if the save fails? Do they lose what they typed?
- What if two people do the same thing at the same time?
- When something goes wrong, what exactly should they see, and what should they be able to do next?

For each: what does the person see, is any of their work lost, and can they recover?

## Category 3 — Empty and extreme states

- What should this look like when there is nothing there yet — zero entries, no search results, a brand-new install?
- What should it look like with a great deal there — 500 entries, a search matching everything?
- Is there a limit on how many, how long, how large? What happens at the limit?

## Category 4 — Who can do this

- Can anyone do this, or only certain people?
- Should it be possible to undo? By whom, and for how long afterward?
- Should there be a record of who did it and when?

*(Note: this project currently has no logins — see `docs/DECISIONS.md` D-005. Ask anyway; the answer may be "everyone, but record it in the history.")*

## Category 5 — Data and privacy

- Does this involve any information about a real student, family, or staff member? (If the answer is anything but a clear no, **stop** — see `docs/GUARDRAILS.md`.)
- Is any of this information sensitive, even if it is not about a person?
- Does anything need to be kept for a period of time, or deleted after a period of time?

## Category 6 — Wording the person sees

- What should the button say?
- What should the page be titled?
- What should the message say when it works? When it fails?

Never invent user-facing wording. Ask for it, propose a draft, and let the PO correct it.

## Category 7 — Boundaries

- What is deliberately **not** part of this request?
- Is there anything you should be careful not to disturb?
- Is this a first version to react to, or is it meant to be finished?

## Category 8 — New packages

If the request appears to need a NuGet package that is not already approved in `approvals/PACKAGE_APPROVALS.md`, that is its own question, asked in plain English, before the plan is written. See `docs/DEPENDENCY_POLICY.md` for exactly how to present it.

## Category 9 — The closing check

Before you stop asking, ask yourself — not the PO:

- Is there any behavior in my plan that the PO did not explicitly describe or agree to?
- Is there any error path in my plan whose message or outcome the PO did not choose?
- Am I about to write the word "assume" anywhere?

If any answer is yes, you have at least one more question to ask.
