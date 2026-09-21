# Plan Template

Copy this into `requests/NNN-short-name.md` and fill it in. Write every word for someone who does not build software.

Rules for writing a plan:

- No technical terms without an inline definition.
- No section left as "to be determined" — if you do not know it, you are still in Stage 2.
- **No assumptions section.** If you were tempted to write one, go ask.
- Every error case the PO described must appear in "When things go wrong."

---

```markdown
## The Plan

### What you will be able to do when this is finished
_Two or three sentences, from the PO's point of view. No technology. Example: "You'll be able to add a new data source by filling in a short form, and it will show up in the list straight away."_

### What changes that you can see
_A short list of concrete, visible changes. A new page. A new button. A new column. Where each one lives._

### What happens step by step
_Numbered, in plain English, describing what the person does and what the system does back. Aim for ten steps or fewer._

### When things go wrong
_A table. One row per thing that can go wrong, with what the person sees and whether any of their work is lost. Every one of these was answered by the PO during questioning — none are invented here._

| If this happens | The person sees | Do they lose their work? |
|---|---|---|
| | | |

### What gets tested
_Plain-English list of what the automated tests will check, including the awkward cases: blank fields, duplicates, very long text, nothing there at all, two people at once. See docs/TESTING_STANDARDS.md._

### New packages needed
_Any NuGet package not already in approvals/PACKAGE_APPROVALS.md, presented per docs/DEPENDENCY_POLICY.md. Write "None" if none. Approving this plan approves these packages._

| Package | Version | What it does, in plain English | Why we can't do this without it |
|---|---|---|---|
| | | | |

### Structure and documents this affects
_Any new project, layer, or folder — and therefore any area guide that must be created or updated. Say whether `docs/ARCHITECTURE.md` changes. Write "no structural change" if there is none._

### Branch
_The branch this will be built on: `feature/NNN-short-name`. It will be left unmerged when the work is finished; merging is a separate decision for the PO._

### What is NOT included
_Explicitly list what this does not do, especially anything the PO might reasonably expect._

### How this gets undone
_One or two sentences on how to put things back if the PO does not like it._

### How long, roughly
_A rough sense of size: a short task, a day, several days. Say if you are unsure._

---

## Approval

**I need your approval before I write any code.** Please reply with the word **APPROVED** if this is what you want, or tell me what to change.

Approved by: _______________
Date: _______________
Their exact words: _______________
```
