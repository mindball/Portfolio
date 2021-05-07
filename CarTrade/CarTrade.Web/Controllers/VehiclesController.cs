using AutoMapper;
using CarTrade.Data.Enums;
using CarTrade.Services.Branches;
using CarTrade.Services.Brand;
using CarTrade.Services.Companies;
using CarTrade.Services.Vehicle;
using CarTrade.Services.Vehicle.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Vehicles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var brandsEnumerable = await brandService.AllAsync();
            var brands = brandsEnumerable.ToSelectListItems(b => b.Name, b => b.Id.ToString());

            var employersEnumerable = await employeerService.AllAsync();
            var employers = employersEnumerable.ToSelectListItems(b => b.Name, b => b.Id.ToString());

            var branchesEnumerable = await branchService.AllAsync();
            var branches = branchesEnumerable.ToSelectListItems(b => b.FullAddress, b => b.Id.ToString());

            var newVehicle = new AddVehicleViewModel()
            {
                Branches = branches,
                Employers = employers,
                Brands = brands
            };            

            return this.View(newVehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddVehicleViewModel vehicleModel)
        {
            if(!ModelState.IsValid)
            {
                return this.View(vehicleModel);
            }

            //TODO: make automatically collect from profile
            var newCar = this.mapper.Map<AddVehicleServiceModel>(vehicleModel);

            await this.vehicleService.AddVehicleAsync(newCar);

            return this.RedirectToAction(nameof(Index));
        }     
    }
}
