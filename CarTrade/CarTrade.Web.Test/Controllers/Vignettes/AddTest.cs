using AutoMapper;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
using CarTrade.Web.Controllers;
using CarTrade.Web.Models.Vignettes;
using CarTrade.Web.Test.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Test.Controllers.Vignettes
{
    public class AddTest
    {
        private const int vehicleIdConst = 1;

        [Fact]
        //Callback не се инициализира защото, трябва да има съществуващ VehicleId
        public async Task AddPostMethod_WithValidVignette_ShouldCallServiceMethod()
        {
            //Arrange
            bool serviceMethodCall = false;
            VignetteFormViewModel newVignette = GetVignetteFormViewMode();
            VignetteFormServiceModel newVignette2 = GetVignetteFormServiceModel();

            var mockVignettesService = new Mock<IVignettesService>();
            var mockVehiclesService = new Mock<IVehicleService>();
            var mockImapper = new Mock<IMapper>();

            TempDataDictionary tempData = TempDataDictionary();

            var controller = new VignettesController(
                mockVignettesService.Object,
                mockVehiclesService.Object,
                mockImapper.Object)
            {
                TempData = tempData
            };

            MockGetVignetteByVehicleId(mockVignettesService);
            MockIMapper(newVignette2, mockImapper);            

            mockVignettesService.Setup(srv => srv.AddVignetteAsync(vehicleIdConst, newVignette2))
                .Callback(() => serviceMethodCall = true);

            //Act
            var result = await controller.Add(newVignette) as RedirectToActionResult;

            //Assert
            Assert.True(serviceMethodCall);
        }

        [Fact]
        public async Task AddPostMethod_WithNotValidModelState_ShouldReturnFilledVignetteFormViewModel()
        {
            //Arrange
            var newVignette = new VignetteFormViewModel()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                Expired = false
            };
            var controller = new VignettesController(null, null, null).WithIdentity(AdministratorRole);
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = await controller.Add(newVignette) as ViewResult;
            var model = result.Model as VignetteFormViewModel;
            //Assert
            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(newVignette.StartDate, model.StartDate);
            Assert.Equal(newVignette.EndDate, model.EndDate);
            Assert.Equal(newVignette.Expired, model.Expired);
        }

        [Fact]
        public async Task AddPostMethod_WithValidVignette_ShouldCAddSuccessMessageTempData()
        {
            VignetteFormViewModel newVignette = GetVignetteFormViewMode();
            VignetteFormServiceModel newVignette2 = GetVignetteFormServiceModel();

            var mockVignettesService = new Mock<IVignettesService>();
            var mockVehiclesService = new Mock<IVehicleService>();
            var mockImapper = new Mock<IMapper>();

            TempDataDictionary tempData = TempDataDictionary();

            var controller = new VignettesController(
                mockVignettesService.Object,
                mockVehiclesService.Object,
                mockImapper.Object)
            {
                TempData = tempData
            };

            MockGetVignetteByVehicleId(mockVignettesService);
            MockIMapper(newVignette2, mockImapper);
            MockAddVignette(mockVignettesService);

            //Act
            await controller.Add(newVignette);

            //Assert
            Assert.True(controller.TempData.ContainsKey(TempDataSuccessMessageKey));
        }

        [Fact]
        public async Task AddGetMethod_ShouldReturnBadRequestWhenVehicleNotExist()
        {
            //Arrange
            var mockVehiclesService = new Mock<IVehicleService>();
            var vehicles = ReturnVehicleCollection();            
            mockVehiclesService.Setup(srv => srv.AllAsync())
                            .Returns(Task.FromResult(vehicles)).Verifiable();
            var controller = new VignettesController(null, mockVehiclesService.Object, null).WithIdentity(AdministratorRole);

            //Act
            var result = await controller.Add(int.MaxValue) as BadRequestResult;
            var actual = result.StatusCode;
            var expected = 400;
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task AddGetMethod_ShouldReturnRedirectToActionWhenVehicleHasActiveVignette()
        {
            //Arrange            
            var mockVehiclesService = new Mock<IVehicleService>();
            var mockVignetteService = new Mock<IVignettesService>();
            var vehicles = ReturnVehicleCollection();
            mockVehiclesService.Setup(srv => srv.AllAsync())
                            .Returns(Task.FromResult(vehicles)).Verifiable();
            mockVignetteService.Setup(srv => srv.DoesVehicleHaveActiveVignetteAsync(vehicleIdConst))
                            .Returns(Task.FromResult(true));
            TempDataDictionary tempData = TempDataDictionary();
            var controller = new VignettesController(mockVignetteService.Object, mockVehiclesService.Object, null)
            {
                TempData = tempData
            }
            .WithIdentity(AdministratorRole);

            //Act
            var result = await controller.Add(vehicleIdConst) as RedirectToActionResult;
            var actualControllerName = result.ControllerName.ToLower();
            var actualActionName = result.ActionName.ToLower();
            var expectedConroller = "vehicles";
            var expectedActionName = "edit";

            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedConroller, actualControllerName);
            Assert.Equal(expectedActionName, actualActionName);
        }

        [Fact]
        public async Task AddGetMethod_ShouldViewWithVignetteFormViewModel()
        {
             
            //Arrange            
            var mockVehiclesService = new Mock<IVehicleService>();
            var mockVignetteService = new Mock<IVignettesService>();
            var vehicles = ReturnVehicleCollection();
            mockVehiclesService.Setup(srv => srv.AllAsync())
                            .Returns(Task.FromResult(vehicles)).Verifiable();
            mockVignetteService.Setup(srv => srv.DoesVehicleHaveActiveVignetteAsync(vehicleIdConst))
                            .Returns(Task.FromResult(false));
            TempDataDictionary tempData = TempDataDictionary();
            var controller = new VignettesController(mockVignetteService.Object, mockVehiclesService.Object, null)
            {
                TempData = tempData
            }
            .WithIdentity(AdministratorRole);

            //Act
            var result = await controller.Add(vehicleIdConst) as ViewResult;            
            var viewModel = result.Model as VignetteFormViewModel;
            var modelValue = viewModel.VehicleId;

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(viewModel);
            Assert.Equal(vehicleIdConst, modelValue);            
        }

        private IEnumerable<VehicleListingServiceModel> ReturnVehicleCollection()
            => new List<VehicleListingServiceModel>()
            {
                new VehicleListingServiceModel
                {
                    Id = vehicleIdConst
                }
            };

        private static void MockAddVignette(Mock<IVignettesService> mockVignettesService)
        {
            mockVignettesService.Setup(srv =>
                            srv.AddVignetteAsync(vehicleIdConst, It.IsAny<VignetteFormServiceModel>()));
        }

        private static TempDataDictionary TempDataDictionary()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return tempData;
        }

        private static void MockIMapper(VignetteFormServiceModel newVignette2, Mock<IMapper> mockImapper)
        {
            mockImapper.Setup(m => m.Map<VignetteFormServiceModel>(It.IsAny<VignetteFormViewModel>()))
                             .Returns(newVignette2);
        }

        private static void MockGetVignetteByVehicleId(Mock<IVignettesService> mockVignettesService)
        {
            mockVignettesService.Setup(srv =>
                            srv.GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleIdConst))
                            .Returns((Task<IEnumerable<VignetteListingServiceModel>>)It.IsAny<IEnumerable<VignetteListingServiceModel>>());
        }

        private static VignetteFormServiceModel GetVignetteFormServiceModel()
        {
            return new VignetteFormServiceModel()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                Expired = false
            };
        }

        private static VignetteFormViewModel GetVignetteFormViewMode()
        {
            //Arrange
            //bool serviceMethodCall = false;
            return new VignetteFormViewModel()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                Expired = false
            };
        }
    }
}
