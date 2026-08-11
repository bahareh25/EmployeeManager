using EmployeeManager.Core.Models;
using System.Text.Json;

namespace EmployeeManager.Infrastructure.DataMocks;

/// <summary>
/// Seed data applied by EF Core migrations through ModelBuilder.HasData.
/// </summary>
/// <remarks>
/// Synchronous on purpose - see the note on <see cref="DepartmentDataMock"/>.
/// Employees are spread across DepartmentIds 1-5; departments 6-10 exist but have
/// no permanent staff, which is useful when you need a department an employee is
/// not already assigned to.
/// </remarks>
public static class EmployeeDataMock
{
    private const string EmployeeData = @"[
  { ""Id"": 1, ""Name"": ""John Matthews"", ""Email"": ""john.matthews@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 2, ""Name"": ""Sarah Collins"", ""Email"": ""sarah.collins@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 3, ""Name"": ""Michael Turner"", ""Email"": ""michael.turner@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 4, ""Name"": ""Emily Rodriguez"", ""Email"": ""emily.rodriguez@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 5, ""Name"": ""David Chen"", ""Email"": ""david.chen@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 6, ""Name"": ""Jessica Brown"", ""Email"": ""jessica.brown@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 7, ""Name"": ""Daniel Wilson"", ""Email"": ""daniel.wilson@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 8, ""Name"": ""Olivia Martin"", ""Email"": ""olivia.martin@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 9, ""Name"": ""James Anderson"", ""Email"": ""james.anderson@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 10, ""Name"": ""Sophia Patel"", ""Email"": ""sophia.patel@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 11, ""Name"": ""Ryan Thompson"", ""Email"": ""ryan.thompson@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 12, ""Name"": ""Amanda Lewis"", ""Email"": ""amanda.lewis@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 13, ""Name"": ""Kevin Harris"", ""Email"": ""kevin.harris@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 14, ""Name"": ""Rachel White"", ""Email"": ""rachel.white@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 15, ""Name"": ""Brian Kim"", ""Email"": ""brian.kim@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 16, ""Name"": ""Laura Scott"", ""Email"": ""laura.scott@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 17, ""Name"": ""Steven Moore"", ""Email"": ""steven.moore@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 18, ""Name"": ""Hannah Walker"", ""Email"": ""hannah.walker@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 19, ""Name"": ""Andrew Young"", ""Email"": ""andrew.young@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 20, ""Name"": ""Priya Nair"", ""Email"": ""priya.nair@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 21, ""Name"": ""Mark Evans"", ""Email"": ""mark.evans@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 22, ""Name"": ""Natalie Brooks"", ""Email"": ""natalie.brooks@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 23, ""Name"": ""Justin Perez"", ""Email"": ""justin.perez@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 24, ""Name"": ""Lauren Stewart"", ""Email"": ""lauren.stewart@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 25, ""Name"": ""Arjun Mehta"", ""Email"": ""arjun.mehta@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 26, ""Name"": ""Chris Adams"", ""Email"": ""chris.adams@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 27, ""Name"": ""Rebecca Green"", ""Email"": ""rebecca.green@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 28, ""Name"": ""Joshua Baker"", ""Email"": ""joshua.baker@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 29, ""Name"": ""Megan Nelson"", ""Email"": ""megan.nelson@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 30, ""Name"": ""Wei Zhang"", ""Email"": ""wei.zhang@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 31, ""Name"": ""Eric Carter"", ""Email"": ""eric.carter@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 32, ""Name"": ""Nicole Ramirez"", ""Email"": ""nicole.ramirez@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 33, ""Name"": ""Brandon Phillips"", ""Email"": ""brandon.phillips@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 34, ""Name"": ""Vanessa Torres"", ""Email"": ""vanessa.torres@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 35, ""Name"": ""Ahmed Hassan"", ""Email"": ""ahmed.hassan@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 36, ""Name"": ""Patrick Murphy"", ""Email"": ""patrick.murphy@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 37, ""Name"": ""Stephanie Reed"", ""Email"": ""stephanie.reed@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 38, ""Name"": ""Anthony Cox"", ""Email"": ""anthony.cox@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 39, ""Name"": ""Melissa Price"", ""Email"": ""melissa.price@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 40, ""Name"": ""Sanjay Kulkarni"", ""Email"": ""sanjay.kulkarni@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 41, ""Name"": ""Adam Foster"", ""Email"": ""adam.foster@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 42, ""Name"": ""Julia Simmons"", ""Email"": ""julia.simmons@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 43, ""Name"": ""Matthew Howard"", ""Email"": ""matthew.howard@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 44, ""Name"": ""Danielle Long"", ""Email"": ""danielle.long@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 45, ""Name"": ""Omar Farooq"", ""Email"": ""omar.farooq@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 46, ""Name"": ""Jason Ward"", ""Email"": ""jason.ward@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 47, ""Name"": ""Kimberly Watson"", ""Email"": ""kimberly.watson@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 48, ""Name"": ""Sean Hughes"", ""Email"": ""sean.hughes@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 49, ""Name"": ""Tiffany Morgan"", ""Email"": ""tiffany.morgan@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 50, ""Name"": ""Lucas Pereira"", ""Email"": ""lucas.pereira@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 51, ""Name"": ""Benjamin Ortiz"", ""Email"": ""benjamin.ortiz@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 52, ""Name"": ""Alyssa Cooper"", ""Email"": ""alyssa.cooper@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 53, ""Name"": ""Noah Reed"", ""Email"": ""noah.reed@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 54, ""Name"": ""Katherine Bell"", ""Email"": ""katherine.bell@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 55, ""Name"": ""Diego Alvarez"", ""Email"": ""diego.alvarez@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 56, ""Name"": ""Jonathan Price"", ""Email"": ""jonathan.price@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 57, ""Name"": ""Monica Flores"", ""Email"": ""monica.flores@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 58, ""Name"": ""Caleb Russell"", ""Email"": ""caleb.russell@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 59, ""Name"": ""Erica Sanders"", ""Email"": ""erica.sanders@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 60, ""Name"": ""Nikhil Verma"", ""Email"": ""nikhil.verma@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 61, ""Name"": ""Peter Lawson"", ""Email"": ""peter.lawson@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 62, ""Name"": ""Grace Mitchell"", ""Email"": ""grace.mitchell@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 63, ""Name"": ""Aaron Bennett"", ""Email"": ""aaron.bennett@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 64, ""Name"": ""Lindsey Parker"", ""Email"": ""lindsey.parker@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 65, ""Name"": ""Yusuf Ali"", ""Email"": ""yusuf.ali@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 66, ""Name"": ""George Coleman"", ""Email"": ""george.coleman@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 67, ""Name"": ""Paige Rivera"", ""Email"": ""paige.rivera@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 68, ""Name"": ""Dylan Peterson"", ""Email"": ""dylan.peterson@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 69, ""Name"": ""Courtney Gray"", ""Email"": ""courtney.gray@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 70, ""Name"": ""Minh Nguyen"", ""Email"": ""minh.nguyen@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 71, ""Name"": ""Henry Powell"", ""Email"": ""henry.powell@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 72, ""Name"": ""Samantha Wood"", ""Email"": ""samantha.wood@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 73, ""Name"": ""Tyler Brooks"", ""Email"": ""tyler.brooks@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 74, ""Name"": ""Alexandra Kelly"", ""Email"": ""alexandra.kelly@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 75, ""Name"": ""Ravi Subramanian"", ""Email"": ""ravi.subramanian@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 76, ""Name"": ""Ethan Rogers"", ""Email"": ""ethan.rogers@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 77, ""Name"": ""Brittany Edwards"", ""Email"": ""brittany.edwards@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 78, ""Name"": ""Logan Turner"", ""Email"": ""logan.turner@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 79, ""Name"": ""Madison Cruz"", ""Email"": ""madison.cruz@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 80, ""Name"": ""Hiroshi Tanaka"", ""Email"": ""hiroshi.tanaka@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 81, ""Name"": ""Charles Fisher"", ""Email"": ""charles.fisher@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 82, ""Name"": ""Victoria Bennett"", ""Email"": ""victoria.bennett@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 83, ""Name"": ""Isaac Coleman"", ""Email"": ""isaac.coleman@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 84, ""Name"": ""Hailey Jenkins"", ""Email"": ""hailey.jenkins@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 85, ""Name"": ""Mohammed Rahman"", ""Email"": ""mohammed.rahman@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 86, ""Name"": ""Nathan Scott"", ""Email"": ""nathan.scott@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 87, ""Name"": ""Isabella Morris"", ""Email"": ""isabella.morris@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 88, ""Name"": ""Jordan Lee"", ""Email"": ""jordan.lee@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 89, ""Name"": ""Kelsey Turner"", ""Email"": ""kelsey.turner@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 90, ""Name"": ""Carlos Mendoza"", ""Email"": ""carlos.mendoza@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 91, ""Name"": ""Victor Nguyen"", ""Email"": ""victor.nguyen@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 92, ""Name"": ""Erin Wallace"", ""Email"": ""erin.wallace@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 93, ""Name"": ""Miles Johnson"", ""Email"": ""miles.johnson@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 94, ""Name"": ""Brooke Lawson"", ""Email"": ""brooke.lawson@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 95, ""Name"": ""Abdul Karim"", ""Email"": ""abdul.karim@acme-corp.com"", ""DepartmentId"": 5 },

  { ""Id"": 96, ""Name"": ""Samuel Wright"", ""Email"": ""samuel.wright@acme-corp.com"", ""DepartmentId"": 1 },
  { ""Id"": 97, ""Name"": ""Chloe Martinez"", ""Email"": ""chloe.martinez@acme-corp.com"", ""DepartmentId"": 2 },
  { ""Id"": 98, ""Name"": ""Connor Hill"", ""Email"": ""connor.hill@acme-corp.com"", ""DepartmentId"": 3 },
  { ""Id"": 99, ""Name"": ""Jenna Roberts"", ""Email"": ""jenna.roberts@acme-corp.com"", ""DepartmentId"": 4 },
  { ""Id"": 100, ""Name"": ""Ibrahim Saleh"", ""Email"": ""ibrahim.saleh@acme-corp.com"", ""DepartmentId"": 5 }
]
";

    public static List<Employee> GetAllEmployees() =>
        JsonSerializer.Deserialize<List<Employee>>(EmployeeData) ?? [];

    public static Employee? GetEmployeeById(int id) =>
        GetAllEmployees().FirstOrDefault(e => e.Id == id);
}