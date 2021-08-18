using CarTrade.Data.Models;
using CarTrade.Microservices.EmailNotifications;
using CarTrade.Microservices.EmailNotifications.Expire;
using CarTrade.Services.Tests;
using CarTrade.Services.Users;
using CarTrade.Services.Vehicles;
using Moq;
using System.Linq;
using System;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Common.DataConstants;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CarTrade.Microservices.Tests.EmailNotifications
{
    [Collection("Database collection")]
    public class ExpireEmailServiceTest : Services.Tests.Common.Data
    {
        private const string searchPatternBr = "<br />";
        private DatabaseFixture fixture;
        private IEmailService emailService;
        private IVehicleService vehiclesService;
        private IUsersService usersService;
        private IEmailConfiguration emailConfiguration;

        public ExpireEmailServiceTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.vehiclesService = new VehicleService(fixture.Context, null);
            this.usersService = new UsersService(fixture.Context, null, null);
            this.emailConfiguration = new EmailConfiguration();

            SeedRoles(this.fixture);
            SeedUsers(this.fixture);
            SeedUserRoles(this.fixture);

            this.emailService = new ExpireEmailService(this.fixture.Context,
            null,
            vehiclesService,
            usersService);
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

        [Fact]
        public async Task ProcessingMessageAsync_ShouldEmailMessagesIsEmpty()
        {
            //Arrange
            foreach (var item in this.fixture.Context.Vehicles)
            {
                this.fixture.Context.Vehicles.Remove(item);
                await this.fixture.Context.SaveChangesAsync();
            }

            //Act
            var messages = await this.emailService.ProcessingMessageAsync();

            //Assert
            Assert.Null(messages);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldEmailMessages_ReturnNotEmptyMessage()
        {
            //Arrange

            //Act
            var messages = await this.emailService.ProcessingMessageAsync();

            //Assert
            Assert.NotNull(messages);
            Assert.True(messages.Count() > 0);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnEmailMessageWithAdminRecipient_WhenRecipientsNotExitsOrEmpty()
        {
            //Arrange
            //Recipients in branch with id 2 not exist
            foreach (var item in this.fixture.Context.Vehicles)
            {
                if (item.BranchId == 2) continue;
                this.fixture.Context.Vehicles.Remove(item);
                await this.fixture.Context.SaveChangesAsync();
            }

            //Act
            var actualMessages = await this.emailService.ProcessingMessageAsync();
            var actualRecipientEmail = actualMessages.Select(m => m.ToAddresses.FirstOrDefault().Address).FirstOrDefault();

            string adminId = "1";
            var expectedRecipient = await fixture.Context.Users.FirstOrDefaultAsync(u => u.Id.Equals(adminId));

            //Assert
            Assert.NotNull(actualMessages);
            Assert.NotNull(actualRecipientEmail);
            Assert.Equal(expectedRecipient.Email, actualRecipientEmail);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnCorrectCountOfExpireInsurance()
        {
            //Arrange
            int expectedResult = 7;
            string searchPattern = "TypeOfInsurance";
            //Act
                     
            var resultMessages = await this.emailService.ProcessingMessageAsync();
            var actualCount = 0;
            foreach (var message in resultMessages)
            {
                actualCount += (message.Content.Split(searchPattern).Length) - 1;                
            }

            //Assert
            Assert.Equal(expectedResult, actualCount);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnCorrectCountOfExpireVignettes()
        {
            //Arrange
            int expectedCount = 8;

            //Act
            var resultMessages = await this.emailService.ProcessingMessageAsync();             
            var actualCount = CountBr(VignetteExpire, searchPatternBr, InspectionCheckExpire, resultMessages);

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnCorrectCountOfExpireExpireInspectionCheck()
        {
            //Arrange
            int expectedCount = 7;     

            //Act
            var resultMessages = await this.emailService.ProcessingMessageAsync();
            var actualCount = CountBr(InspectionCheckExpire, searchPatternBr, OilCheckExpire, resultMessages);

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task ProcessingMessageAsync_ShouldReturnCorrectCountOfExpirOoilDistance()
        {
            //Arrange
            int expectedCount = 7;

            //Act
            var resultMessages = await this.emailService.ProcessingMessageAsync();
            var actualCount = CountBr(OilCheckExpire, searchPatternBr, OilCheckExpire, resultMessages);

            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        private static int CountBr(string searchPattern, string searchPattern2, string searchPattern3, List<EmailMessage> resultMessages)
        {
            int actualCount = 0;
            foreach (var message in resultMessages)
            {
                var splitContentWhensearchPatternExist = message.Content.Split(searchPattern);
                if (splitContentWhensearchPatternExist.Length > 1
                    || splitContentWhensearchPatternExist.Length == 2)
                {
                    var expireVigneteStr = splitContentWhensearchPatternExist[1].Split(searchPattern3);
                    var splitExpireVignetteSection =
                        expireVigneteStr[0].Split(searchPattern2, StringSplitOptions.RemoveEmptyEntries).Length;
                    actualCount += (splitExpireVignetteSection / 2);
                }
            }

            return actualCount;
        }

        private static Mock<IVehicleService> GetMockVehiclesService()
        {
            return new Mock<IVehicleService>();
        }

        private async Task SeedUsers(DatabaseFixture fixture)
        {
            fixture.Context.Users.Add(new User
            {
                Id = 1.ToString(),
                Email = "admn@admin.com",
                UserName = "admin",
                FirstName = "Autohit trade",
                SecondName = "Autohit trade",
                LastName = "Autohit trade",
                BranchId = 1
            });

            fixture.Context.Users.Add(new User
            {
                Id = 2.ToString(),
                Email = "manager@starazagora.com",
                UserName = "starazagora2",
                FirstName = "starazagora2",
                SecondName = "starazagora2",
                LastName = "starazagora2",
                BranchId = 1
            });

            fixture.Context.Users.Add(new User
            {
                Id = 3.ToString(),
                Email = "user@starazagora.com",
                UserName = "user.stz",
                FirstName = "user",
                SecondName = "starazagora2",
                LastName = "starazagora2",
                BranchId = 1
            });

            fixture.Context.Users.Add(new User
            {
                Id = 4.ToString(),
                Email = "manager@burgas.com",
                UserName = "manager.burgas",
                FirstName = "burgas",
                SecondName = "burgas",
                LastName = "burgas",
                BranchId = 3,
            });

            fixture.Context.Users.Add(new User
            {
                Id = 5.ToString(),
                Email = "user@burgas.com",
                UserName = "user.burgas",
                FirstName = "burgas",
                SecondName = "burgas",
                LastName = "burgas",
                BranchId = 3,
            });

            fixture.Context.Users.Add(new User
            {
                Id = 6.ToString(),
                Email = "manager@plovdiv.com",
                UserName = "manager.plovdiv",
                FirstName = "plovdiv",
                SecondName = "plovdiv",
                LastName = "plovdiv",
                BranchId = 4,
            });

            fixture.Context.Users.Add(new User
            {
                Id = 7.ToString(),
                Email = "user@plovdiv.com",
                UserName = "user.plovdiv",
                FirstName = "plovdiv",
                SecondName = "plovdiv",
                LastName = "plovdiv",
                BranchId = 4,
            });

            fixture.Context.Users.Add(new User
            {
                Id = 8.ToString(),
                Email = "manager@ruse.com",
                UserName = "ruse",
                FirstName = "ruse",
                SecondName = "ruse",
                LastName = "ruse",
                BranchId = 5,
            });

            await this.fixture.Context.SaveChangesAsync();
        }

        private async Task SeedRoles(DatabaseFixture fixture)
        {
            this.fixture.Context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = 1.ToString(),
                Name = AdministratorRole
            });
            this.fixture.Context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = 2.ToString(),
                Name = ManagerRole
            });
            this.fixture.Context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = 3.ToString(),
                Name = DriverRole
            });

            await this.fixture.Context.SaveChangesAsync();
        }

        private async Task SeedUserRoles(DatabaseFixture fixture)
        {
            //Administrator Stz
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 1.ToString(),
                UserId = 1.ToString()
            });

            //Manager Stz
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 2.ToString(),
                UserId = 2.ToString()
            });

            //Manager Burgas
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 2.ToString(),
                UserId = 4.ToString()
            });

            //User Burgas
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 3.ToString(),
                UserId = 5.ToString()
            });

            //Manager Plovdiv
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 2.ToString(),
                UserId = 6.ToString()
            });

            //User Plovdiv
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 3.ToString(),
                UserId = 7.ToString()
            });

            //Manager Ruse
            this.fixture.Context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                RoleId = 2.ToString(),
                UserId = 8.ToString()
            });


            await this.fixture.Context.SaveChangesAsync();
        }
    }
}
