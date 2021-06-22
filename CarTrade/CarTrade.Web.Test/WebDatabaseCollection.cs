using CarTrade.Services.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CarTrade.Web.Test
{
    [CollectionDefinition("Web collection")]
    public class WebDatabaseCollection : ICollectionFixture<DatabaseFixture>
    {

    }
}
