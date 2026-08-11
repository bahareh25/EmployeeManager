# EmployeeManager — SDBP 022 Assignment 02 starter

A layered ASP.NET Core Web API on .NET 10 with EF Core, SQL Server, and both unit and
integration test projects. This is the required starting point for Assignment 02.

## Prerequisites

- **.NET 10 SDK** — check with `dotnet --version`
- **SQL Server LocalDB** — installed with Visual Studio 2022/2026 as part of the
  *ASP.NET and web development* workload. Check with `sqllocaldb info`.
- Visual Studio 2022/2026, JetBrains Rider, or VS Code with the C# Dev Kit

## Project layout

| Project | Contains |
|---|---|
| `EmployeeManager.Core` | Domain entities (`Employee`, `Department`) |
| `EmployeeManager.Application` | Repository interfaces and DTOs — no EF dependency |
| `EmployeeManager.Infrastructure` | `AppDbContext`, repository implementations, migrations, seed data |
| `EmployeeManager.API` | Controllers, DI wiring, JSON configuration |
| `EmployeeManager.Tests` | Unit tests — repository mocked, no database |
| `EmployeeManagerApi.IntegrationTests` | Integration tests — real API, real database |

## Getting started

```bash
git clone <repo-url>
cd EmployeeManager
dotnet restore
dotnet build
```

Create the application database (run from the solution root):

```bash
dotnet ef database update --project EmployeeManager.Infrastructure --startup-project EmployeeManager.API
```

If `dotnet ef` is not recognised:

```bash
dotnet tool install --global dotnet-ef
```

Run the API:

```bash
dotnet run --project EmployeeManager.API
```

- API base URL: **http://localhost:5092**
- Swagger UI: **http://localhost:5092/swagger**

HTTPS redirection is disabled in Development on purpose, so Postman and the integration
tests can call plain HTTP without hitting a 307 redirect or a certificate warning.

## Databases

Two separate databases, both on LocalDB, so tests never touch your development data:

| Database | Used by | Configured in |
|---|---|---|
| `db_employee` | the running API | `EmployeeManager.API/appsettings.json` |
| `db_employee_integration` | integration tests | `EmployeeManagerWebApplicationFactory.cs` |

To point either at a different server, override the connection string rather than editing
the committed default:

```bash
# integration tests
setx EMPLOYEEMANAGER_TEST_DB "Server=.\SQLEXPRESS;Database=db_employee_integration;Trusted_Connection=True;TrustServerCertificate=True;"

# API — user secrets
dotnet user-secrets set "ConnectionStrings:EmployeeDB" "Server=.\SQLEXPRESS;Database=db_employee;Trusted_Connection=True;TrustServerCertificate=True;" --project EmployeeManager.API
```

> **Your submission must run on the grading machine.** Do not commit a connection string
> that names your own computer. LocalDB is the safe default.

## Running the tests

```bash
dotnet test
```

Goal 4 requires a screenshot of this command passing, taken from a terminal whose path
shows your name or Humber index number. Use the CLI — a Test Explorer screenshot does not
satisfy the requirement.

To run one project at a time:

```bash
dotnet test EmployeeManager.Tests
dotnet test EmployeeManagerApi.IntegrationTests
```

### How the integration tests are wired

Read these three files before writing your own tests:

- **`EmployeeManagerWebApplicationFactory.cs`** — boots the API in memory and swaps the
  connection string for the test database. It does no database setup.
- **`ApiTestFixture.cs`** — creates the database once for the whole test run and exposes a
  ready `HttpClient`, a `ResetDatabase()` helper, and `WithDbContext(...)` for direct
  database access.
- **`IntegrationTestCollection.cs`** — puts every test class in one xUnit collection so
  they share the fixture and never run concurrently.

Every integration test class you add must follow this shape:

```csharp
[Collection(IntegrationTestCollection.Name)]
public class MyTests
{
    private readonly HttpClient _client;

    public MyTests(ApiTestFixture fixture) => _client = fixture.Client;
}
```

Do **not** write `new EmployeeManagerWebApplicationFactory()` inside a test. Doing so
rebuilds the host per test and, once you have more than one test class, produces
intermittent "cannot drop database because it is in use" failures.

Tests share one database and run in an unspecified order. Write assertions that tolerate
rows left behind by earlier tests — prefer "the response contains the item I created" over
an exact row count — or call `fixture.ResetDatabase()` when you need a clean slate.

## API conventions

These apply to the controller you write as well as the existing one.

**Enums are JSON strings.** `JsonStringEnumConverter` is registered in `Program.cs`, so a
status travels as `"Scheduled"`, not `0`. The database column stays an `int`.

**Controllers return DTOs, never EF entities.** See `EmployeeManager.Application/Dtos`.
Entities carry navigation properties that form cycles when serialized, and exposing them
couples your API contract to your schema.

**Status codes:**

| Situation | Response |
|---|---|
| Read succeeded | `200 OK` |
| Collection is empty | `200 OK` with `[]` — not `404` |
| Resource created | `201 Created` + `Location` header |
| Update succeeded | `200 OK` with the updated resource |
| Delete succeeded | `204 No Content` |
| Resource not found | `404 Not Found` |
| Invalid input or a broken business rule | `400 Bad Request` + `ValidationProblemDetails` |
| Conflict with existing state | `409 Conflict` + `ProblemDetails` |

`EmployeeController` is a complete worked example of all of the above.

**Validation precedence.** A single request can break several rules at once, but only one
status code comes back. Run your checks in this order:

1. Model binding and data annotations — automatic via `[ApiController]` → `400`
2. Does the addressed resource exist? → `404`
3. Do the foreign keys in the body resolve? → `400`
4. Business rules, in the order the brief lists them → `400`, or `409` for conflicts

Step 2 before step 3 is the one people get wrong. A `PUT /api/employees/999999` carrying an
invalid `departmentId` must return `404`, not `400` — answering "your department is wrong"
for a URL that identifies nothing points the client at the wrong problem. The brief
publishes the full order for `/api/assignment`; follow it, because a correct
implementation can still return the wrong code if the checks run in the wrong sequence.

## Seed data

Applied by the migration through `HasData`, so `dotnet ef database update` and
`Database.Migrate()` both produce a populated database. There is no separate seeding step.

- 10 departments, ids 1–10
- 100 employees, ids 1–100, spread across departments 1–5

Departments 6–10 have no permanent staff, which is useful when you need a department an
employee is not already assigned to.

## Submitting

Before you zip:

```bash
dotnet build      # must succeed with no errors
dotnet test       # must pass
```

Then run the self-check Postman suite in `postman/` against your running API — see
[postman/README.md](postman/README.md). It checks every requirement in the brief, and
catches contract mistakes that your own tests will not.

Exclude `bin/`, `obj/` and `.vs/` from your ZIP — `.gitignore` already excludes them from
source control. Name the ZIP as specified in the assignment brief.
