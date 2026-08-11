using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAndDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Human Resources" },
                    { 2, "Finance" },
                    { 3, "Engineering" },
                    { 4, "Marketing" },
                    { 5, "Sales" },
                    { 6, "Customer Support" },
                    { 7, "IT" },
                    { 8, "Research and Development" },
                    { 9, "Operations" },
                    { 10, "Legal" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "Name" },
                values: new object[,]
                {
                    { 1, 1, "john.matthews@acme-corp.com", "John Matthews" },
                    { 2, 2, "sarah.collins@acme-corp.com", "Sarah Collins" },
                    { 3, 3, "michael.turner@acme-corp.com", "Michael Turner" },
                    { 4, 4, "emily.rodriguez@acme-corp.com", "Emily Rodriguez" },
                    { 5, 5, "david.chen@acme-corp.com", "David Chen" },
                    { 6, 1, "jessica.brown@acme-corp.com", "Jessica Brown" },
                    { 7, 2, "daniel.wilson@acme-corp.com", "Daniel Wilson" },
                    { 8, 3, "olivia.martin@acme-corp.com", "Olivia Martin" },
                    { 9, 4, "james.anderson@acme-corp.com", "James Anderson" },
                    { 10, 5, "sophia.patel@acme-corp.com", "Sophia Patel" },
                    { 11, 1, "ryan.thompson@acme-corp.com", "Ryan Thompson" },
                    { 12, 2, "amanda.lewis@acme-corp.com", "Amanda Lewis" },
                    { 13, 3, "kevin.harris@acme-corp.com", "Kevin Harris" },
                    { 14, 4, "rachel.white@acme-corp.com", "Rachel White" },
                    { 15, 5, "brian.kim@acme-corp.com", "Brian Kim" },
                    { 16, 1, "laura.scott@acme-corp.com", "Laura Scott" },
                    { 17, 2, "steven.moore@acme-corp.com", "Steven Moore" },
                    { 18, 3, "hannah.walker@acme-corp.com", "Hannah Walker" },
                    { 19, 4, "andrew.young@acme-corp.com", "Andrew Young" },
                    { 20, 5, "priya.nair@acme-corp.com", "Priya Nair" },
                    { 21, 1, "mark.evans@acme-corp.com", "Mark Evans" },
                    { 22, 2, "natalie.brooks@acme-corp.com", "Natalie Brooks" },
                    { 23, 3, "justin.perez@acme-corp.com", "Justin Perez" },
                    { 24, 4, "lauren.stewart@acme-corp.com", "Lauren Stewart" },
                    { 25, 5, "arjun.mehta@acme-corp.com", "Arjun Mehta" },
                    { 26, 1, "chris.adams@acme-corp.com", "Chris Adams" },
                    { 27, 2, "rebecca.green@acme-corp.com", "Rebecca Green" },
                    { 28, 3, "joshua.baker@acme-corp.com", "Joshua Baker" },
                    { 29, 4, "megan.nelson@acme-corp.com", "Megan Nelson" },
                    { 30, 5, "wei.zhang@acme-corp.com", "Wei Zhang" },
                    { 31, 1, "eric.carter@acme-corp.com", "Eric Carter" },
                    { 32, 2, "nicole.ramirez@acme-corp.com", "Nicole Ramirez" },
                    { 33, 3, "brandon.phillips@acme-corp.com", "Brandon Phillips" },
                    { 34, 4, "vanessa.torres@acme-corp.com", "Vanessa Torres" },
                    { 35, 5, "ahmed.hassan@acme-corp.com", "Ahmed Hassan" },
                    { 36, 1, "patrick.murphy@acme-corp.com", "Patrick Murphy" },
                    { 37, 2, "stephanie.reed@acme-corp.com", "Stephanie Reed" },
                    { 38, 3, "anthony.cox@acme-corp.com", "Anthony Cox" },
                    { 39, 4, "melissa.price@acme-corp.com", "Melissa Price" },
                    { 40, 5, "sanjay.kulkarni@acme-corp.com", "Sanjay Kulkarni" },
                    { 41, 1, "adam.foster@acme-corp.com", "Adam Foster" },
                    { 42, 2, "julia.simmons@acme-corp.com", "Julia Simmons" },
                    { 43, 3, "matthew.howard@acme-corp.com", "Matthew Howard" },
                    { 44, 4, "danielle.long@acme-corp.com", "Danielle Long" },
                    { 45, 5, "omar.farooq@acme-corp.com", "Omar Farooq" },
                    { 46, 1, "jason.ward@acme-corp.com", "Jason Ward" },
                    { 47, 2, "kimberly.watson@acme-corp.com", "Kimberly Watson" },
                    { 48, 3, "sean.hughes@acme-corp.com", "Sean Hughes" },
                    { 49, 4, "tiffany.morgan@acme-corp.com", "Tiffany Morgan" },
                    { 50, 5, "lucas.pereira@acme-corp.com", "Lucas Pereira" },
                    { 51, 1, "benjamin.ortiz@acme-corp.com", "Benjamin Ortiz" },
                    { 52, 2, "alyssa.cooper@acme-corp.com", "Alyssa Cooper" },
                    { 53, 3, "noah.reed@acme-corp.com", "Noah Reed" },
                    { 54, 4, "katherine.bell@acme-corp.com", "Katherine Bell" },
                    { 55, 5, "diego.alvarez@acme-corp.com", "Diego Alvarez" },
                    { 56, 1, "jonathan.price@acme-corp.com", "Jonathan Price" },
                    { 57, 2, "monica.flores@acme-corp.com", "Monica Flores" },
                    { 58, 3, "caleb.russell@acme-corp.com", "Caleb Russell" },
                    { 59, 4, "erica.sanders@acme-corp.com", "Erica Sanders" },
                    { 60, 5, "nikhil.verma@acme-corp.com", "Nikhil Verma" },
                    { 61, 1, "peter.lawson@acme-corp.com", "Peter Lawson" },
                    { 62, 2, "grace.mitchell@acme-corp.com", "Grace Mitchell" },
                    { 63, 3, "aaron.bennett@acme-corp.com", "Aaron Bennett" },
                    { 64, 4, "lindsey.parker@acme-corp.com", "Lindsey Parker" },
                    { 65, 5, "yusuf.ali@acme-corp.com", "Yusuf Ali" },
                    { 66, 1, "george.coleman@acme-corp.com", "George Coleman" },
                    { 67, 2, "paige.rivera@acme-corp.com", "Paige Rivera" },
                    { 68, 3, "dylan.peterson@acme-corp.com", "Dylan Peterson" },
                    { 69, 4, "courtney.gray@acme-corp.com", "Courtney Gray" },
                    { 70, 5, "minh.nguyen@acme-corp.com", "Minh Nguyen" },
                    { 71, 1, "henry.powell@acme-corp.com", "Henry Powell" },
                    { 72, 2, "samantha.wood@acme-corp.com", "Samantha Wood" },
                    { 73, 3, "tyler.brooks@acme-corp.com", "Tyler Brooks" },
                    { 74, 4, "alexandra.kelly@acme-corp.com", "Alexandra Kelly" },
                    { 75, 5, "ravi.subramanian@acme-corp.com", "Ravi Subramanian" },
                    { 76, 1, "ethan.rogers@acme-corp.com", "Ethan Rogers" },
                    { 77, 2, "brittany.edwards@acme-corp.com", "Brittany Edwards" },
                    { 78, 3, "logan.turner@acme-corp.com", "Logan Turner" },
                    { 79, 4, "madison.cruz@acme-corp.com", "Madison Cruz" },
                    { 80, 5, "hiroshi.tanaka@acme-corp.com", "Hiroshi Tanaka" },
                    { 81, 1, "charles.fisher@acme-corp.com", "Charles Fisher" },
                    { 82, 2, "victoria.bennett@acme-corp.com", "Victoria Bennett" },
                    { 83, 3, "isaac.coleman@acme-corp.com", "Isaac Coleman" },
                    { 84, 4, "hailey.jenkins@acme-corp.com", "Hailey Jenkins" },
                    { 85, 5, "mohammed.rahman@acme-corp.com", "Mohammed Rahman" },
                    { 86, 1, "nathan.scott@acme-corp.com", "Nathan Scott" },
                    { 87, 2, "isabella.morris@acme-corp.com", "Isabella Morris" },
                    { 88, 3, "jordan.lee@acme-corp.com", "Jordan Lee" },
                    { 89, 4, "kelsey.turner@acme-corp.com", "Kelsey Turner" },
                    { 90, 5, "carlos.mendoza@acme-corp.com", "Carlos Mendoza" },
                    { 91, 1, "victor.nguyen@acme-corp.com", "Victor Nguyen" },
                    { 92, 2, "erin.wallace@acme-corp.com", "Erin Wallace" },
                    { 93, 3, "miles.johnson@acme-corp.com", "Miles Johnson" },
                    { 94, 4, "brooke.lawson@acme-corp.com", "Brooke Lawson" },
                    { 95, 5, "abdul.karim@acme-corp.com", "Abdul Karim" },
                    { 96, 1, "samuel.wright@acme-corp.com", "Samuel Wright" },
                    { 97, 2, "chloe.martinez@acme-corp.com", "Chloe Martinez" },
                    { 98, 3, "connor.hill@acme-corp.com", "Connor Hill" },
                    { 99, 4, "jenna.roberts@acme-corp.com", "Jenna Roberts" },
                    { 100, 5, "ibrahim.saleh@acme-corp.com", "Ibrahim Saleh" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
