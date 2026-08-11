namespace EmployeeManagerApi.IntegrationTests.Urls;

/// <summary>
/// Every URL the API exposes, in one place. Use these constants in tests rather than
/// typing route strings inline - a typo then becomes a compile error instead of a 404.
/// </summary>
public static class ApiRoutes
{
    public static class Employees
    {
        public const string Base = "/api/employees";

        public static string ById(int id) => $"{Base}/{id}";
    }

    /// <summary>
    /// Routes for the controller you must implement in Assignment 02.
    /// </summary>
    /// <remarks>
    /// These are fixed by the assignment brief and are what the automated grading suite
    /// calls. Note the singular "assignment" - it does not match the plural used by the
    /// employees controller, but it is the required route. Do not change these values.
    /// </remarks>
    public static class Assignments
    {
        public const string Base = "/api/assignment";

        public static string ById(int id) => $"{Base}/{id}";
    }
}
