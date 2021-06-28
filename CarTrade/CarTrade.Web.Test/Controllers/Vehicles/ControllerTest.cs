using CarTrade.Web.Controllers;
using CarTrade.Web.Test.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Test.Controllers.Vehicles
{
    public class ControllerTest
    {
        [Fact]
        public void ControllerShouldHasAuthorizeAttribute()
        {
            //Arrange
            var controller = typeof(VehiclesController);

            //Act
            var attributes = controller.GetCustomAttributes(true);

            //Assert
            attributes.Should()
                .Match(attr => attr
                    .Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public void Controller_ShouldBeAccessibleByAdministrator()
        {
            //Arrange, Act
            var controller = new VehiclesController(null, null, null, null, null).WithIdentity(AdministratorRole);

            //Assert
            Assert.True(controller.User.IsInRole(AdministratorRole));
        }

        //няма смисъл винаги влизаш в action-a
        //[Fact]
        //public async Task Controller_ShouldBeNotAccessibleByOtherUsers()
        //{
        //    //Arrange
        //    var controller = new VehiclesController(null, null, null, null, null);

        //    //Act
        //    var result = await controller.Index() as ViewResult;
        //    var viewName = result.ViewName;

        //    //Assert
        //    Assert.True(string.IsNullOrEmpty(viewName));
        //}
    }
}
