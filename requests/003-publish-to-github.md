# Request 003 — Publish to GitHub

**Status:** Complete and merged
**Opened:** 2026-10-15
**Product Owner:** Product Owner
**Branch:** `feature/003-publish-to-github` — created after approval, left unmerged
**Status of branch:** merged into main and pushed on 2026-10-15, branch kept

---

## What was asked for

> I want to push this my github and create a new repo

## What I understand this to be

Create a new repository on the Product Owner's GitHub account and push this project to it, so the work exists somewhere other than this one computer.

---

## Why this needs refining rather than just doing

This **reverses a recorded decision.** `D-006` and request 001 question 6 both say the project stays on this computer and nothing is ever sent anywhere. That was the right call then; the PO may well want to change it now. But it is changed deliberately, in writing, not quietly.

It is also a hard stop twice over in `docs/GUARDRAILS.md`: pushing to a remote, adding a remote, and sending anything outside this computer.

## What was checked before asking

| Thing | Found |
|---|---|
| GitHub CLI (`gh`) | **Not installed.** No way to create a repository from this session |
| GitHub credentials in this session | None |
| Git remote on this repo | None |
| Tracked files | 90 |
| The catalog data file (`datacatalog.db`) | **Correctly excluded** — not tracked, will not be published |
| Commit attribution | Every commit reads `dhenderson <dhenderson@fcmat.org>` |

---

## Questions and answers

| # | Question asked | PO's answer | Date |
|---|---|---|---|
| 1 | Should the repository be private or public? | **Public.** Noted at the time: the repository contains no actual district systems — real entries live only in the excluded `datacatalog.db`, so what is published is the rulebook, the code and the request records. Accepted consequence: `dhenderson@fcmat.org` is permanently and publicly attached to all commits. | 2026-10-15 |
| 2 | How should the repository get created, given the GitHub CLI is not installed? | **The PO creates an empty repository on github.com** (no README, no licence, no .gitignore, so it cannot conflict) and gives the agent the address. The agent adds the remote and pushes. No software installed; account actions stay with the PO. | 2026-10-15 |
| 3 | What should the repository be called? | **Data_Catalog** — matches the folder name on this computer exactly, so the two are never ambiguous. | 2026-10-15 |
| 4 | Push just main, or the feature branches too? | **Just main.** Both feature branches were merged, so every commit is already in main history. The two local branches stay on this computer. | 2026-10-15 |
| 5 | Should the public repository carry a licence? | **MIT.** Without one, a public repository legally cannot be reused by anyone. MIT is also what nearly every package in this project uses. | 2026-10-15 |
| 6 | Should the stale README be corrected as part of this? | **Yes.** It still opens "Nothing has been built yet", which stopped being true in September. The paragraph is replaced; the rest, including the section written for the PO, stays. | 2026-10-15 |
| 7 | Who is the copyright holder? | **The organisation**, not the individual. Flagged at the time: naming an employer as holder and publishing under MIT releases work the employer owns, which is normally the organisation decision rather than one person. The PO accepted that. | 2026-10-15 |
| 8 | What exactly is the organisation called? | **California Schools Information Services (CSIS)**, as given by the PO ("CSIS California Schools information services"). Written in the licence as `Copyright (c) 2026 California Schools Information Services (CSIS)`. | 2026-10-15 |
| 9 | Should `.claude/launch.json` be published too? | **Yes.** Four lines naming the project and port 5199, nothing private, and it saves the next person rediscovering a non-obvious port. | 2026-10-15 |

---

## The Plan

_Version 1 — 2026-10-15_

### What you will be able to do when this is finished

The project will exist on GitHub as well as on this computer, at a public address you can send to anyone. If this machine is lost, the software and its whole history survive. Other people — another district, a colleague, you on a different computer — can read it, and under the MIT licence use it.

### What changes that you can see

- A new **public** repository on your GitHub account called **Data_Catalog**, holding the project and all nine commits
- A new file in the project, `LICENSE`, reading `Copyright (c) 2026 California Schools Information Services (CSIS)`
- The README's opening corrected: it currently says nothing has been built yet, which has not been true since September
- `.claude/launch.json` committed, so anyone who clones it can start the catalog

### What happens step by step

1. **You** create an empty repository on github.com called `Data_Catalog`, public, with **no** README, licence or .gitignore — anything pre-added would collide with what is already here.
2. You paste me its address.
3. **Before anything leaves this computer**, I check what would be published: that `datacatalog.db` is not among the files, that nothing looks like a password or key, and that no real person's details are in there apart from your commit email, which you have accepted.
4. I add the licence, correct the README, commit `.claude/launch.json`, and run the full test suite to confirm nothing broke.
5. I add your repository as the remote and push `main`.
6. I confirm what arrived matches what is here, and give you the address.

### When things go wrong

| If this happens | What I do | Is anything lost or exposed? |
|---|---|---|
| The pre-publish check finds the catalog data file, a key, or anything unexpected | **I stop and tell you before pushing.** Nothing leaves this computer | No. That is the point of checking first |
| The repository you made is not empty (it has a README or licence) | I stop and ask. Forcing past it would mean overwriting what GitHub created | No |
| Git asks you to sign in to GitHub | A sign-in window may appear on your screen and you complete it. I never see or handle your password | No |
| The address is wrong or the repository does not exist | The push fails harmlessly, I remove the remote and ask you to check | No |
| The push is rejected for any other reason | I report the exact message rather than trying to force it | No. A rejected push changes nothing |
| You change your mind after it is public | Deleting the repository is yours to do on GitHub. **But anything already copied by anyone cannot be unpublished** | Possibly. This is the one step that is not fully reversible |

### What gets tested

This request adds no features, so there is nothing new to unit test. What I do verify:

- The full suite still passes after the licence, README and config changes — `dotnet test`, all 106
- `datacatalog.db` is not in the published files, checked explicitly rather than assumed
- The commits on GitHub match the commits here, by their identifiers
- The published repository contains no file that was not already tracked here

### New packages needed

None.

### Structure and documents this affects

- New `LICENSE` at the project root
- `README.md` opening corrected
- `.claude/launch.json` becomes tracked
- A decision recorded that **supersedes request 001 question 6** ("this computer only — nothing is ever sent anywhere")

**Precisely what is and is not superseded:** `D-006` says the *catalog runs* on one computer, reachable only from it. That is **unchanged** — publishing the code does not put the running app anywhere. What changes is only where the *source and its history* live.

### Branch

`feature/003-publish-to-github`, created after this approval. Left unmerged; merging is yours to ask for.

### What is NOT included

- Making the repository private later, or deleting it — both yours to do on GitHub
- Any automated building or testing on GitHub
- The two feature branches — main only (question 4)
- Asking your employer whether this may be published under their name. **I have flagged it; the decision is recorded as yours**
- Anything about where the catalog *runs*. It stays on this computer

### How this gets undone

Locally: the branch is abandoned, or the remote removed with one command. On GitHub: you delete the repository. **The part that cannot be undone is publication itself** — once public, anyone may have taken a copy, and deleting the repository afterwards does not retrieve it.

### How long, roughly

Short — minutes, once you have created the repository. The one unpredictable part is GitHub sign-in, which happens on your screen, not mine.

---

## Approval

**Approved by:** Product Owner
**Date:** 2026-10-15
**Their exact words:** "appoved" — an obvious typing slip for "approved", taken as approval
**Plan version approved:** Version 1 — 2026-10-15

Approving this plan also approves, per `docs/GUARDRAILS.md`, the hard stops it necessarily crosses: adding a remote, pushing to it, and publishing this project outside this computer.

The repository address was not supplied with the approval, so everything up to the push was done and the push waited for it.

---

## What was built

- A **public** repository at `https://github.com/dhendiJr/Data_Catalog`, holding the project and its full history
- `LICENSE` — MIT, `Copyright (c) 2026 California Schools Information Services (CSIS)`
- The README's opening corrected; it had claimed since September that nothing was built
- `.claude/launch.json` published, so anyone cloning can start the catalog
- `D-035` (publish) and `D-036` (which account) recorded

### Test results

```
dotnet test

Passed!  - Failed: 0, Passed: 71, Skipped: 0, Total: 71 - DataCatalog.Core.UnitTests.dll
Passed!  - Failed: 0, Passed: 22, Skipped: 0, Total: 22 - DataCatalog.Web.ComponentTests.dll
Passed!  - Failed: 0, Passed: 13, Skipped: 0, Total: 13 - DataCatalog.UiTests.dll
```

**Pre-publish check, before anything left this computer:**

| Check | Result |
|---|---|
| `datacatalog.db` in any commit, ever | No — district entries are not published |
| Credentials, keys, tokens in tracked files | None |
| Emails in file content | `dhenderson@fcmat.org` only, which the PO accepted |
| Files published | 92 |

### Anything that did not go to plan

1. **The plan had the order wrong, and it showed.** It said to push `main`, and also that visitors would see the licence and corrected README — but those were committed on this request's branch, and `D-014` keeps a branch unmerged until the PO asks. So the first push put the project public **with no licence and the stale README**. The PO then approved merging and pushing again. The flaw was mine, in the plan, and the browser check caught it rather than anyone noticing later.
2. **A rule break of my own.** `git add -A` swept `.claude/launch.json` into the approval-records commit on `main`, instead of onto this request's branch — `P29`. The content was what the PO approved; it landed one commit early on the wrong branch. Offered to rewrite history to fix it; the PO said leave it.
3. **Two GitHub accounts, discovered the hard way.** The repository was created under `dhendiJr` because that is who `gh` is signed in as. The first push was then refused — `Permission to dhendiJr/Data_Catalog.git denied to dhendersonfcmat` — because the saved Git credentials belong to a different account. Nothing had been published at that point. The PO chose to keep it on `dhendiJr` (`D-036`).
4. **The push protocol had to change.** `gh` configured Git for SSH; the HTTPS credential helper authenticates as the other account. The remote was set to SSH, which authenticates correctly with a key already registered on `dhendiJr`. No configuration outside this project was touched.
5. **The PO installed the GitHub CLI themselves.** Installing software is a hard stop for an agent; this was their own action on their own machine, and it is what made creating the repository possible at all.
6. **The accepted email appears in file content as well.** `dhenderson@fcmat.org` is in this request record because I wrote it there while reporting the pre-publish check, not only in commit metadata.

### Noticed while building

Not acted on.

- **`gh` is not on this session's shell PATH** and has to be called by its full path, `C:\Program Files\GitHub CLI\gh.exe`. A newly opened terminal will have it.
- **The repository has no description or topics**, so it is unlabelled to anyone browsing GitHub.
- **No automated building or testing on GitHub.** The 106 tests run only on this computer, so nothing would catch a broken commit after a push.
- **A `dhendersonfcmat` account exists.** If this work should ever sit under the work account instead, moving it is its own request.

---

## Definition of Done checklist

_To be copied from `docs/DEFINITION_OF_DONE.md` when the work is complete._
