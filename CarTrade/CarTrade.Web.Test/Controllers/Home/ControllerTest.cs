using CarTrade.Web.Controllers;
using CarTrade.Web.Models;
using CarTrade.Web.Test.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Test.Controllers.Home
{
    public class ControllerTest
    {
        [Fact]
        public void HomeControllerShouldAllowAnonymous()
        {
            //Arrange
            var controller = typeof(HomeController);

            //Act
            var attributes = controller.GetCustomAttributes(true);

            //Assert
            attributes.Should()
                .Match(attr => attr
                    .Any(a => a.GetType() == typeof(AllowAnonymousAttribute)));
        }

        [Fact]
        public async Task Controller_ShouldBeAccessibleByAnonymous()
        {
            //Arrange
            var controller = new HomeController(null, null, null, null).WithAnonymousIdentity();

            //Act
            var result = controller.Error() as ViewResult;
            var actualViewModel = result.ViewData.Model.GetType();
            var expectedViewmodel = new ErrorViewModel().GetType();

            //Assert
            Assert.Equal(expectedViewmodel, actualViewModel);
        }
    }
}
