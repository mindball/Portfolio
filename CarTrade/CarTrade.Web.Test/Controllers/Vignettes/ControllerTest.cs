using CarTrade.Web.Controllers;
using CarTrade.Web.Test.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Test.Controllers.Vignettes
{
    public class ControllerTest
    {
        [Fact]
        public void HomeControllerShouldAllowAnonymous()
        {
            //Arrange
            var controller = typeof(VignettesController);

            //Act
            var attributes = controller.GetCustomAttributes(true);

            //Assert
            attributes.Should()
                .Match(attr => attr
                    .Any(a => a.GetType() == typeof(AuthorizeAttribute)));
        }

        [Fact]
        public async Task Controller_ShouldBeAccessibleByAdministrator()
        {
            //Arrange, Act
            var controller = new VignettesController(null, null, null).WithIdentity(AdministratorRole);
                        
            //Assert
            Assert.True(controller.User.IsInRole(AdministratorRole));
        }

        [Fact]
        public async Task Controller_ShouldBeNotAccessibleByOtherUsers()
        {
            //Arrange
            var controller = new VignettesController(null, null, null).WithAnonymousIdentity();

            //Act
            var result = await controller.ListBranchVignetes(1) as ViewResult;
            var viewName = result.ViewName;

            //Assert
            Assert.True(string.IsNullOrEmpty(viewName));
        }
    }
}
