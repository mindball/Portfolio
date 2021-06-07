using Xunit;

namespace CarTrade.Services.Tests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {  
        
    }
}
