namespace EmployeeManagerApi.IntegrationTests;

/// <summary>
/// Groups every integration test class into one xUnit collection so they share a single
/// <see cref="ApiTestFixture"/> and never run at the same time as each other.
/// </summary>
/// <remarks>
/// Put <c>[Collection(IntegrationTestCollection.Name)]</c> on every integration test class
/// you write, and take <see cref="ApiTestFixture"/> as a constructor parameter.
/// This class has no code of its own - the attributes are the whole point.
/// </remarks>
[CollectionDefinition(IntegrationTestCollection.Name, DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<ApiTestFixture>
{
    public const string Name = "Integration";
}
