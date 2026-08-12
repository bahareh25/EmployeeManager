using EmployeeManager.Application.Repositories;
using EmployeeManager.Infrastructure;
using EmployeeManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace EmployeeManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    // Enums are sent and received as their NAMES ("Scheduled", "Active", ...),
                    // not as integers. The database column remains an int - this setting only
                    // affects the JSON on the wire. The automated test suite depends on this.
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                    // Navigation properties form cycles (Employee -> Department -> Employees).
                    // Without this, serializing an entity graph throws a JsonException.
                    // Returning DTOs (see Application/Dtos) is the better fix; this is a safety net.
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Produces RFC 7807 ProblemDetails bodies for error status codes.
            builder.Services.AddProblemDetails();

            //Add dependency Injection for Application and Infrastructure layers
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();

            //Add dbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDB"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Only redirect outside Development. In Development the API is reachable over
                // plain HTTP at http://localhost:5092, which is what Postman and the integration
                // tests use - a 307 redirect to HTTPS would break both.
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
