using CarTrade.Microservices.EmailNotifications;
using CarTrade.Microservices.EmailNotifications.Expire;
using CarTrade.Services.Branches;
using CarTrade.Services.Tests;
using CarTrade.Services.Users;
using CarTrade.Services.Vehicles;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Microservices.Tests.EmailNotifications
{
    [Collection("Database collection")]
    public class ExpireEmailServiceTest : Services.Tests.Common.Data
    {
        private DatabaseFixture fixture;        
        private IEmailService emailService;
        private Mock<IVehicleService>  mockVehiclesService;
        private Mock<IUsersService> mockUsersService;
        private Mock<IBranchesService> mockBranchesService;

        public ExpireEmailServiceTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.mockVehiclesService = GetMockVehiclesService();
            this.mockUsersService = GetMockUsersService();
            this.mockBranchesService = GetMockBranchesService();

            this.emailService = new ExpireEmailService(this.fixture.Context, 
                this.mockUsersService.Object, 
                null,
                this.mockBranchesService.Object, 
                this.mockVehiclesService.Object);

        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnArgumentException_WhenNotExistBranches()
        {
            //Arrange
            foreach (var item in this.fixture.Context.Branches)
            {
                this.fixture.Context.Branches.Remove(item);
                await this.fixture.Context.SaveChangesAsync();
            }

            //Act
            var exceptionMessage = await Assert.ThrowsAsync<ArgumentException>(() => this.emailService.ProcessingMessageAsync());

            //Assert
            Assert.Equal(NotExistItemExceptionMessage, exceptionMessage.Message);
        }

        private static void SetupMockBranchesService(Mock<IBranchesService> mockBranchesService)
        {
            if (mockBranchesService != null)
            {
                //mockBranchesService.Setup(srv => srv.AllAsync());
            }
        }

        private static Mock<IBranchesService> GetMockBranchesService()
        {
            return new Mock<IBranchesService>();
        }

        private static void SetupMockUsersService(Mock<IUsersService> mockUsersService)
        {
            if (mockUsersService != null)
            {
                //mockUsersService.Setup(srv => srv.AllAsync());
            }
        }

        private static Mock<IUsersService> GetMockUsersService()
        {
            return new Mock<IUsersService>();
        }

        private static void SetupMockUVehiclesService(Mock<IVehicleService> mockVehiclesService)
        {
            if (mockVehiclesService != null)
            {
                //mockUsersService.Setup(srv => srv.AllAsync());
            }
        }

        private static Mock<IVehicleService> GetMockVehiclesService()
        {
            return new Mock<IVehicleService>();
        }
    }
}
