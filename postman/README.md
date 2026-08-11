# Self-check Postman suite

Run this against your own API before you submit. Every assertion corresponds to a
requirement in the assignment brief.

Passing it does not guarantee full marks — the grading suite includes additional cases —
but **failing it means something is wrong**, and it is far cheaper to find that out now.

## Running it

Start your API first:

```bash
dotnet run --project EmployeeManager.API
```

Then either:

**In Postman** — import both files, select the *SDBP 022 - Assignment 02 (local)*
environment, and use the Collection Runner.

**From the terminal** —

```bash
npm install -g newman
newman run SDBP022-A2-selfcheck.postman_collection.json
```

## What it checks

| Folder | Covers |
|---|---|
| Row 3 - CRUD endpoints | All four routes, status codes, `Location` header, response shape |
| Row 3 - Referential integrity | Unknown `EmployeeId` / `DepartmentId` produce 400, not 500 |
| Row 4 - BR-01 | One Active assignment per employee |
| Row 4 - BR-02 | The 31-day boundary, including the exact-31 and past-date cases |
| Row 4 - BR-03 | New assignments persist as `Scheduled` |
| Row 4 - BR-04 | The transition state machine, terminal states, no-op updates |
| Row 4 - BR-05 | Not the employee's own permanent department |

Assertions named `arrange:` or `cleanup` are scaffolding — they set up and remove test
data and carry no marks. If one fails, fix that first: everything after it in the same
folder will be unreliable.

## Notes

You can run it as often as you like. Each folder creates the rows it needs and deletes
them afterwards, so it does not require a fresh database.

Dates are computed when the run starts, so the BR-02 boundary cases are always relative
to today.

If **everything** fails, the API is probably not running, or not on
`http://localhost:5092`. The brief requires that port — do not change it, because the
grading suite uses it.
