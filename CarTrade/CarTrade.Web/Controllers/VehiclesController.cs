using AutoMapper;
using CarTrade.Services.Branches;
using CarTrade.Services.Brand;
using CarTrade.Services.Companies;
using CarTrade.Services.Vehicle;
using CarTrade.Services.Vehicle.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IVehicleService vehicleService;
        private readonly IMapper mapper;
        private readonly IBranchesService branchService;
        private readonly ICompaniesService employeerService;
        private readonly IBrandService brandService;

        public VehiclesController(IVehicleService vehicleService,
            IMapper mapper, IBranchesService branchService,
            ICompaniesService employeerService,
            IBrandService brandService)
        {
            this.vehicleService = vehicleService;
            this.mapper = mapper;
            this.branchService = branchService;
            this.employeerService = employeerService;
            this.brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await this.vehicleService.AllAsync();

            return View(vehicles);
        }

        public async Task<IActionResult> Add()
        {
            var collectCompanyDetails = await this.FillCollectCompanyDetails();

            var newVehicle = new VehicleFormViewModel();
            newVehicle.CollectCompanyDetails = collectCompanyDetails;
            //{
            //    CollectCompanyDetails. = collectCompanyDetails.Branches,
            //    CollectCompanyDetails.Brands = collectCompanyDetails.Brands,
            //    CollectCompanyDetails.Employers = collectCompanyDetails.Employers
            //};

            return this.View(newVehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VehicleFormViewModel vehicleModel)
        {
            if (vehicleModel == null || !ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, vehicleModel.PlateNumber));
                var collectCompanyDetails = await this.FillCollectCompanyDetails();
                vehicleModel.CollectCompanyDetails.Branches = collectCompanyDetails.Branches;
                vehicleModel.CollectCompanyDetails.Brands = collectCompanyDetails.Brands;
                vehicleModel.CollectCompanyDetails.Employers = collectCompanyDetails.Employers;

                return this.View(vehicleModel);
            }

            //TODO: make automatically collect from profile with reflection
            var newCar = this.mapper.Map<AddVehicleServiceModel>(vehicleModel);

            await this.vehicleService.AddVehicleAsync(newCar);

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, vehicleModel.PlateNumber));

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")]int vehicleId)
        {
            var vehicle = 
                await this.vehicleService.GetByIdAsync<AddVehicleServiceModel>(vehicleId);

            if(vehicle == null)
            {
                return this.NotFound("Vehicle not found");
            }   

            var editVehicle = this.mapper
                .Map<AddVehicleServiceModel, VehicleFormViewModel>(vehicle, opts => 
                    opts.ConfigureMap()
                    .ForMember(a => a.CollectCompanyDetails, opt => 
                        opt.Ignore()));

            var collectCompanyDetails = await this.FillCollectCompanyDetails();
            editVehicle.CollectCompanyDetails = collectCompanyDetails;           

            return this.View(editVehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int vehicleId,
            VehicleFormViewModel vehicleModel)
        {
            if (vehicleModel == null || !ModelState.IsValid)
            {                
                this.TempData.AddFailureMessage(string.Format(FailureEditItemMessage, vehicleModel.PlateNumber));
                return RedirectToAction(nameof(Index));
            }

            var vehicleAddServiceModel =
                this.mapper.Map<AddVehicleServiceModel>(vehicleModel);

            await this.vehicleService.EditVehicleAsync(vehicleId, vehicleAddServiceModel);          

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, vehicleModel.PlateNumber));
            return this.RedirectToAction(nameof(Index));
        }

        private async Task<CollectCompanyDetailsViewModel> FillCollectCompanyDetails()
        {
            var brandsEnumerable = await brandService.AllAsync();
            var employersEnumerable = await employeerService.AllAsync();
            var branchesEnumerable = await branchService.AllAsync();

            var newVehicle = new CollectCompanyDetailsViewModel()
            {
                Branches = branchesEnumerable
                     .ToSelectListItems(b => b.FullAddress, b => b.Id.ToString()),
                Employers = employersEnumerable
                     .ToSelectListItems(b => b.Name, b => b.Id.ToString()),
                Brands = brandsEnumerable
                     .ToSelectListItems(b => b.Name, b => b.Id.ToString())
            };

            return newVehicle;
        }
    }
}
