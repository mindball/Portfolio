using AutoMapper;
using CarTrade.Services.Branches;
using CarTrade.Services.Branches.Models;
using CarTrade.Services.Brands;
using CarTrade.Services.Companies;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Home;
using CarTrade.Web.Models.Vehicles;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class VehiclesController : ManagerController
    {
        private readonly IVehicleService vehicleService;
        private readonly IMapper mapper;
        private readonly IBranchesService branchService;
        private readonly ICompaniesService employeerService;
        private readonly IBrandService brandService;
        private const string InsuranceType = "expire insurances";
        private const string VignetteType = "expire vignettes";
        private const string InspectionCheck = "expire inspection check";
        private const string OilCheck = "expire oil distance";

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

            var newVehicle = new VehicleFormViewModel()
            {
                InspectionSafetyCheck = DateTime.UtcNow,
                YearОfМanufacture = DateTime.UtcNow
            };
            newVehicle.CollectCompanyDetails = collectCompanyDetails;            

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
            var newCar = this.mapper.Map<VehicleFormServiceModel>(vehicleModel);

            await this.vehicleService.AddVehicleAsync(newCar);

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, vehicleModel.PlateNumber));

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")]int vehicleId)
        {
            var vehicle = 
                await this.vehicleService.GetByIdAsync<VehicleFormServiceModel>(vehicleId);

            if(vehicle == null)
            {
                return this.NotFound("Vehicle not found");
            }   

            var editVehicle = this.mapper
                .Map<VehicleFormServiceModel, VehicleFormViewModel>(vehicle, opts => 
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
                this.mapper.Map<VehicleFormServiceModel>(vehicleModel);

            await this.vehicleService.EditVehicleAsync(vehicleId, vehicleAddServiceModel);          

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, vehicleModel.PlateNumber));
            return this.RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ExpireInsurances(string branchFullAddress, IList<VehicleListExpireInsurancePolicyServiceModel> vehicles)
        {
            if(branchFullAddress == null || vehicles == null)
            {
                return this.BadRequest();
            }

            var vehiclesWithExprireData = new ListExpireDataForAllBranchesViewModel
            {
                FullAddress = branchFullAddress,
                VehiclesWithExpirePolicy = vehicles,      
                TypeOfExpire = InsuranceType
            };           

            return this.View(vehiclesWithExprireData);
        }

        [HttpPost]
        public IActionResult ExpireVignettes(string branchFullAddress, IList<VehicleListExpireVignetteServiceModel> vehicles)
        {

            if (branchFullAddress == null || vehicles == null)
            {
                return this.BadRequest();
            }

            var vehiclesWithExprireData = new ListExpireDataForAllBranchesViewModel
            {
                FullAddress = branchFullAddress,
                VehiclesWithExpireVignettes = vehicles,
                TypeOfExpire = VignetteType
            };           

            return this.View(vehiclesWithExprireData);
        }

        [HttpPost]
        public IActionResult ExpireOilChangeDistance(string branchFullAddress, IList<VehicleListingChangeOilServiceModel> vehicles)
        {
            if (branchFullAddress == null || vehicles == null)
            {
                return this.BadRequest();
            }

            var vehiclesWithExprireData = new ListExpireDataForAllBranchesViewModel
            {
                FullAddress = branchFullAddress,
                VehiclesWithOilChangeDistance = vehicles,
                TypeOfExpire = OilCheck
            };

            return this.View(vehiclesWithExprireData);
        }

        [HttpPost]
        public IActionResult ExpireInspectionSafetyCheck(string branchFullAddress, IList<VehicleListingInspectionSafetyCheckServiceModel> vehicles)
        {

            if (branchFullAddress == null || vehicles == null)
            {
                return this.BadRequest();
            }

            var vehiclesWithExprireData = new ListExpireDataForAllBranchesViewModel
            {
                FullAddress = branchFullAddress,
                VehiclesWithInspectionExpire = vehicles,
                TypeOfExpire = InspectionCheck
            };

            return this.View(vehiclesWithExprireData);
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
