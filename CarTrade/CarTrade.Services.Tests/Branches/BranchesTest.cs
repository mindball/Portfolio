using CarTrade.Services.Branches;
using CarTrade.Services.Branches.Models;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CarTrade.Services.Tests.Branches
{
    [Collection("Database collection")]
    public class BranchesTest
    {
        private DatabaseFixture fixture;
        private IBranchesService branchService;

        public BranchesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.branchService = new BranchesService(fixture.Context);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 1)]
        [InlineData(3, 3)]
        [InlineData(4, 1)]
        [InlineData(5, 5)]
        public async Task GetAllVehicleByBranchAsyncShouldReturn(int branchId, int expected)
        {
            //Arrange

            //Act

            var result = await this.branchService.GetAllVehicleByBranchAsync(branchId);

            //Assert
            result.Should().NotBeNull(null);
            result.AllVehicles.Should().NotBeEmpty()
                    .And.HaveCount(expected)
                    .And.OnlyHaveUniqueItems();
        }
    }
}
