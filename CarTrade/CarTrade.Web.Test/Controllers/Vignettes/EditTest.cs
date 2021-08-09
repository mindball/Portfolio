using CarTrade.Services.Vignettes;
using CarTrade.Web.Controllers;
using CarTrade.Web.Models.Vignettes;
using CarTrade.Web.Test.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Web.WebConstants;
using CarTrade.Common;

namespace CarTrade.Web.Test.Controllers.Vignettes
{
    public class EditTest
    {
        [Fact]
        public async Task Edit_ShouldReturnNotFoundWhenVignetteNotExist()
        {
            //Arrange
            var mockVignettesService = new Mock<IVignettesService>();
            mockVignettesService.Setup(v => v.GetByIdAsync<VignetteDetailViewModel>(1));

            var controller = new VignettesController(mockVignettesService.Object, null, null).WithIdentity(DataConstants.AdministratorRole);

            //Act
            var result = controller.Edit(1);
            var actionResultActual = result.Result as NotFoundResult;
            var expectedStatusCode = 404;

            //Assert
            Assert.Equal(expectedStatusCode, actionResultActual.StatusCode);
        }

        [Fact]
        public async Task Edit_ShouldReturnViewWithModel()
        {
            //Arrange
            var mockVignettesService = new Mock<IVignettesService>();
            mockVignettesService.Setup(v => v.GetByIdAsync<VignetteDetailViewModel>(1))
                .Returns(Task.FromResult(new VignetteDetailViewModel()));

            var controller = new VignettesController(mockVignettesService.Object, null, null).WithIdentity(DataConstants.AdministratorRole);

            //Act
            var result = controller.Edit(1);
            var actionResultActual = result.Result as ViewResult;
            var actionModelResultActual = actionResultActual.Model as VignetteDetailViewModel;

            //Assert
            Assert.NotNull(actionResultActual);
            Assert.NotNull(actionModelResultActual);
            Assert.IsType<VignetteDetailViewModel>(actionModelResultActual);
            
        }
    }
}