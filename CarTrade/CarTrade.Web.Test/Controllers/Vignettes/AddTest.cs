using AutoMapper;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
using CarTrade.Web.Controllers;
using CarTrade.Web.Models.Vignettes;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarTrade.Web.Test.Controllers.Vignettes
{
    public class AddTest
    {
        [Fact]
        public async Task Add_WithValidVignette_ShouldCallServiceMethod()
        {
            //Arrange
            bool serviceMethodCall = false;
            var newVignette = new VignetteFormViewModel();
            var newVignette2 = new VignetteFormServiceModel();

            var mockVignettesService = new Mock<IVignettesService>();
            var mockVehiclesService = new Mock<IVehicleService>();
            var mockImapper = new Mock<IMapper>();

            mockImapper.Setup(m => m.Map<VignetteFormServiceModel>(newVignette))
                .Returns(newVignette2);

            mockVignettesService.Setup(srv => srv.AddVignetteAsync(1, newVignette2))
                .Callback(() => serviceMethodCall = true);

            var controller = new VignettesController(
                mockVignettesService.Object,
                mockVehiclesService.Object,
                mockImapper.Object);

            //Act
            var result = controller.Add(newVignette);
            
            //Assert
            Assert.True(serviceMethodCall);
        }
    }
}
