# Project Charter — School District Data Catalog

Read this before your first response in any session. It says what this project is and, just as importantly, what it is not.

---

## The problem

Staff across the district need to know where information comes from. Today, finding out means asking around: who owns attendance data, whether the number in this report matches the one in that one, who to talk to about food service records. The knowledge lives in a handful of people's heads.

## What we are building

**A searchable directory of the district's data sources.** A phone book for data.

For each system the district uses — student information, gradebook, attendance, transportation, food service, and so on — the catalog records what lives in it, who owns it, who to ask, and how sensitive it is. Someone with a question about data should be able to search the catalog and find out who to talk to, without asking around.

## What a single entry records

Seven fields (decision D-010):

| Field | Plain meaning |
|---|---|
| **Name** | What people call this data source |
| **Description** | What it is, in one or two plain sentences |
| **System** | Which software or platform it lives in |
| **Owner** | Which department or role is responsible for it |
| **Contact** | Who to ask about it |
| **Sensitivity** | How careful people need to be with it |
| **Update frequency** | How often it changes — live, nightly, weekly, once a year |

## What this is **not**

- **Not a field-level data dictionary.** It does not catalog individual tables and columns.
- **Not an access request system.** It does not control or track who may see what.
- **Not a report inventory.** It catalogs systems, not the reports drawn from them.
- **Not a place where data lives.** It describes where data is. It never contains the data itself.

Each of these might be a good idea later. Each would be its own request, refined and approved from scratch.

## How it is built

- **.NET 10, Blazor Server, MudBlazor** (D-008)
- **xUnit, bUnit, Playwright** for testing (D-009)
- Runs on **one computer, localhost only** (D-006)
- **No logins** — anyone who can reach it can view and edit, wiki-style (D-005)

Because there are no logins, two things follow and are not negotiable:

1. **Edit history is required.** Every change to an entry is recorded, so a wrong edit can be seen and undone.
2. **No real personal data, ever.** The catalog describes data sources; it never holds student, family, or staff records. Not real ones, not sample ones. All examples are invented. This is a FERPA requirement, not a style preference.

## Who it is for

**Product Owner:** a district staff member who understands the district and its data, and who does not write software. They decide what gets built. Every plan is written for them to read and approve.

**Users:** district staff who need to find out where data comes from and who to ask about it.

## How work happens

Every change follows `docs/WORKFLOW.md`: one question at a time until nothing is unknown, a plan in plain English, written approval, then build with tests, then honest reporting of the results.

The first request through this process will be choosing how to set the project up.

## What success looks like

A staff member with a question about data opens the catalog, searches, and finds the right person to ask — in under a minute, without asking anyone first.
