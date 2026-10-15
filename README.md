# School District Data Catalog

A searchable directory of the district's data sources — which systems exist, what is in each, who owns them, and who to ask.

You can add a data source, see them all in a list, open one to read everything about it, correct it, and remove it. Removing hides rather than destroys, so anything taken off the list can be put back. Every change is recorded with the date and what it changed from.

It runs on one computer, reached in a browser, and has no logins — so anyone who can reach it can edit it. That is deliberate (see the decisions below), and it is why the change history exists.

Built with .NET 10, Blazor Server and MudBlazor, and covered by 106 automated tests.

**What it is not:** a field-level data dictionary, an access-request system, or a place where the data itself lives. It records *where* data is and who to ask about it.

Alongside the software, this repository holds something less usual: the complete rulebook an AI agent must follow to work on it, and the full record of every request — each question asked, each answer given, and the written approval before any code was written. `requests/` and `docs/` are as much the point of this project as `src/`.

## If you are an AI agent

Read [AGENTS.md](AGENTS.md) first, completely, before doing anything else.

## If you are the Product Owner

You do not need to read the technical files. These two are written for you:

- [PROJECT_CHARTER.md](PROJECT_CHARTER.md) — what we are building and what we are deliberately not building
- [docs/GLOSSARY.md](docs/GLOSSARY.md) — plain-English meanings of any word an agent uses that you do not recognize

How the work goes: you describe what you want, the agent asks you one question at a time until nothing is left guessed at, then writes you a plan in plain English. Nothing gets built until you reply **APPROVED**. Your approval gets written down with your name and the date. Then the agent builds it, tests it, and shows you the real results — including anything that failed.

Each feature is built on a **branch** — a private copy of the project. Nothing reaches the real version until you say so, so a feature you do not like is simply abandoned rather than unpicked.

If an agent ever starts building without your written approval, merges something you did not ask it to merge, or uses a word you do not understand, it is breaking the rules of this project.

## What is in here

```
AGENTS.md              the front door for any AI agent
PROJECT_CHARTER.md     what this project is and is not
docs/                  the rules: principles, workflow, questions, architecture, naming,
                       code standards, testing, branching, guardrails, decisions
approvals/             signed approvals — packages and everything else, with who and when
requests/              one file per request: the ask, the questions, the plan, the approval, the results
```
