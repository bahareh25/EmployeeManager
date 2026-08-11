using EmployeeManager.API;
using EmployeeManager.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManagerApi.IntegrationTests;

/// <summary>
/// Boots the real API in memory and points it at a dedicated integration test database.
/// </summary>
/// <remarks>
/// Do not construct this directly in a test. Use <see cref="ApiTestFixture"/> through
/// the "Integration" collection so the database is created once for the whole run
/// instead of once per test.
/// </remarks>
public class EmployeeManagerWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// LocalDB is installed with Visual Studio, so this works on any machine with
    /// no setup. Override it without editing code by setting the
    /// EMPLOYEEMANAGER_TEST_DB environment variable.
    /// </summary>
    private const string DefaultConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=db_employee_integration;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("EMPLOYEEMANAGER_TEST_DB") ?? DefaultConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Drop the registrations that Program.cs made against the real database
            // before adding our own. Registering AppDbContext twice without removing
            // the originals leaves stale DbContextOptions in the container.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));

            // Note what is NOT here: no EnsureDeleted, no Migrate, no BuildServiceProvider.
            // ConfigureTestServices runs every time a host is created, so doing database
            // work here means dropping and recreating the database on every construction.
            // Schema setup belongs in ApiTestFixture, which runs exactly once.
        });
    }
}
