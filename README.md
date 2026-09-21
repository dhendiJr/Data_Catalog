# School District Data Catalog

A searchable directory of the district's data sources — which systems exist, what is in each, who owns them, and who to ask.

**Nothing has been built yet.** This folder currently holds the rules that any AI agent must follow when building it.

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
