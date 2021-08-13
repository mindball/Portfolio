using CarTrade.Services.Tests;
using Xunit;

namespace CarTrade.Microservices.Tests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {

    }
}
