using System.Text.Json;
using System.Text.Json.Serialization;
using EmployeeManager.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagerApi.IntegrationTests;

/// <summary>
/// Owns the in-memory API host, the HttpClient, and the integration test database.
/// Created once for every test class in the "Integration" collection.
/// </summary>
/// <remarks>
/// Why a shared fixture rather than "new EmployeeManagerWebApplicationFactory()" inside
/// each test:
/// <list type="number">
/// <item>Dropping and recreating the database per test is slow - roughly a second each.</item>
/// <item>SQL Server refuses to drop a database that still has open connections, so a
/// second test starting while the first is finishing fails intermittently.</item>
/// <item>xUnit runs test classes in parallel by default. Two classes racing to drop and
/// migrate the same database produce failures that have nothing to do with your code.</item>
/// </list>
/// </remarks>
public sealed class ApiTestFixture : IDisposable
{
    /// <summary>
    /// JSON options that match how the API is configured. Pass these to PostAsJsonAsync,
    /// PutAsJsonAsync and ReadFromJsonAsync whenever the payload contains an enum.
    /// </summary>
    /// <remarks>
    /// The API writes enums as names ("Scheduled"), because JsonStringEnumConverter is
    /// registered in Program.cs. The default client-side options understand numbers only,
    /// so ReadFromJsonAsync would throw a JsonException on the response. This trips people
    /// up constantly - if a test fails with "The JSON value could not be converted to
    /// AssignmentStatus", this is what you forgot.
    /// </remarks>
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public EmployeeManagerWebApplicationFactory Factory { get; }

    /// <summary>Pre-configured client pointing at the in-memory API.</summary>
    public HttpClient Client { get; }

    public ApiTestFixture()
    {
        Factory = new EmployeeManagerWebApplicationFactory();

        // Build the database before the first request reaches the API.
        ResetDatabase();

        Client = Factory.CreateClient();
    }

    /// <summary>
    /// Drops the integration test database and rebuilds it from the migrations,
    /// which also re-applies the HasData seed rows.
    /// </summary>
    /// <remarks>
    /// Call this from a test that needs a known-clean starting point. It is expensive,
    /// so prefer writing tests that tolerate rows left behind by earlier tests - for
    /// example, assert that a collection "contains" what you created rather than
    /// asserting on an exact count.
    /// </remarks>
    public void ResetDatabase()
    {
        // Pooled connections stay open after a request completes and will block
        // the DROP DATABASE that EnsureDeleted issues.
        SqlConnection.ClearAllPools();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureDeleted();

        // Migrate rather than EnsureCreated: it applies your migration files, so a
        // missing or out-of-date migration fails here instead of silently passing.
        dbContext.Database.Migrate();
    }

    /// <summary>Runs a unit of work against the test database directly, bypassing the API.</summary>
    /// <remarks>Handy for arranging state or asserting on what was actually persisted.</remarks>
    public void WithDbContext(Action<AppDbContext> action)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        action(dbContext);
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}
