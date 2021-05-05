using AutoMapper;
using CarTrade.Data.Enums;
using CarTrade.Services.Branches;
using CarTrade.Services.Brand;
using CarTrade.Services.Companies;
using CarTrade.Services.Vehicle;
using CarTrade.Services.Vehicle.Models;
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
            var newVehicle = new AddVehicleViewModel()
            {
                Branches = await GetBranches(),
                Employers = await GetEmployers(),
                Brands = await GetBrands()
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

            var newCar = this.mapper.Map<AddVehicleServiceModel>(vehicleModel);

            await this.vehicleService.AddVehicleAsync(newCar);

            return this.RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //public async Task<IActionResult> Add(AddVehicleServiceModel vehicleModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return this.View(vehicleModel); 
        //    }            

        //    await this.vehicleService.AddVehicleAsync(vehicleModel);

        //    return this.RedirectToAction(nameof(Index));
        //}

        private async Task<IEnumerable<SelectListItem>> GetBranches()
        {
            var branches = await branchService.AllAsync();

            return branches.Select(t => new SelectListItem
            {
                Text = t.FullAddress,
                Value = t.Id.ToString()
            }).ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetEmployers()
        {
            var employers = await employeerService.AllAsync();

            return employers.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var brands = await brandService.AllAsync();

            return brands.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Id.ToString()
            }).ToList();
        }


    }
}
