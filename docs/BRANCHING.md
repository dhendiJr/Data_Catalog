# Branching and Version Control

Every feature is built on its own branch. Nothing is built directly on the main line, ever.

The reason is simple: an approved plan can still turn out wrong once it is built. A branch means an unwanted feature is abandoned rather than unpicked, and the main line is always in a state the Product Owner has accepted.

> **Note:** this folder is not yet a git repository. Running `git init` is part of request 001, project setup. Everything below takes effect the moment the repository exists.

---

## The rule

> **Create a branch before you write a single line of code for a feature. Never build on `main`.**

The order is: approval first, then branch, then code. A branch created before approval is a branch that tempted someone to start early.

## Branch names

Branch names match the request they come from, so anyone can trace a branch back to the plan and the approval that authorized it.

```
feature/NNN-short-name
```

- `NNN` is the request number — the same number as the file in `requests/`
- `short-name` matches the request file name
- lower case, words separated by hyphens, no spaces, no capitals, no underscores

| Request file | Branch |
|---|---|
| `requests/001-project-setup.md` | `feature/001-project-setup` |
| `requests/004-add-search.md` | `feature/004-add-search` |
| `requests/011-edit-history.md` | `feature/011-edit-history` |

Prefixes for work that is not a feature:

| Prefix | Use it for |
|---|---|
| `feature/` | Anything the PO asked for |
| `fix/` | Correcting something that is broken, still under an approved plan |
| `docs/` | Changes only to the markdown rulebook, with no code |

There is no `refactor/` prefix. Refactoring is not something an agent initiates (`P25`).

## Commits

- Commit in small, coherent pieces. One commit does one thing.
- Commit messages are written in plain English, present tense, saying what changed and why: `Add sensitivity filter to catalog list`, not `updates` or `wip` or `fix stuff`.
- Reference the request number in the first commit of a branch.
- Never commit anything containing real personal data. If you realize you have, **stop and tell the PO immediately** — rewriting history is their decision, not yours.
- Never commit secrets, keys, or passwords. This project should not have any; if you think you need one, that is a stop.

## What happens when the feature is done

Per decision `D-014`:

1. The full test suite passes on the branch.
2. You report to the PO: what now works, the actual test output, anything that did not go to plan.
3. **You stop.** The branch stays unmerged.
4. The PO decides whether it merges. Merging is a separate request from them, not the natural end of the work.

You do not merge because the tests pass. You do not merge because the plan said so. You merge when the PO tells you to, in this conversation, and then you record it in `approvals/APPROVAL_LOG.md`.

If the PO does not want the feature, the branch is simply left alone. Nothing needs unpicking. That is the whole point.

## Hard stops — ask the PO first, every time

- **Merging** anything into `main`
- **Pushing** to any remote, or adding a remote
- **Deleting** any branch
- **Rebasing**, squashing, amending, or otherwise rewriting history
- **Force-pushing** anything, under any circumstances
- **Discarding** uncommitted work — `git reset --hard`, `git checkout --`, `git clean`
- **Reverting** a commit
- Changing anything in `.git/` directly
- Adding git hooks or changing git configuration

Creating a branch, switching to it, and committing to it are the only version control actions you take on your own initiative — and only under an approved plan.

## If you find yourself on `main`

Stop before making any change. Check with `git status` and `git branch --show-current` at the start of every build. If you are on `main` and about to write code, create the branch first. If you have **already** made changes on `main`, do not try to fix it quietly — tell the PO what happened and ask how to proceed.

## The rulebook itself

Changes to the markdown files in `docs/`, `approvals/`, and `requests/` follow the same rule, on a `docs/` branch — with one exception: recording an approval, a decision, or a request answer may be committed directly, because those are the record of what the PO said and must not sit unmerged on a branch where the next session cannot see them.

## What is never committed

A `.gitignore` is part of request 001 and must cover at minimum:

- `bin/` and `obj/`
- local settings files containing anything machine-specific
- test output, screenshots, and Playwright artifacts
- anything that ever contained real data
