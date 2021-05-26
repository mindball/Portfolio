using AutoMapper;
using CarTrade.Services.Vehicle;
using CarTrade.Services.Vignettes;
using CarTrade.Services.Vignettes.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Vignettes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class VignettesController : BaseController
    {
        private readonly IVignettesService vignettesService;
        private readonly IVehicleService vehicleService;
        private readonly IMapper mapper;

        public string FailureAddItemMessage { get; private set; }

        public VignettesController(IVignettesService vignettesService, 
            IVehicleService vehicleService,
            IMapper mapper)
        {
            this.vignettesService = vignettesService;
            this.vehicleService = vehicleService;

            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
            => this.View(new VignetteListingViewModel
                        {
                            AllVignettes = await this.vignettesService.AllAsync()
                        }
            );

        public async Task<IActionResult> Add([FromRoute(Name = "id")]int vehicleId)
        {
            //var vehicleVignette = (await this.vignettesService
            //    .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId))
            //        .Where(vg => vg.);

            //if(vehicleVignette == null)
            //{
            //    TempData.AddFailureMessage(FailureAddItemMessage);
            //    return BadRequest();
            //}
            
            //if(vehicleVignette.EndDate >= DateTime.UtcNow)                
            //{
            //    //TODO: inform client to exist vignette
            //    TempData.AddFailureMessage($"This vehicle has vignette until {vehicleVignette.EndDate}");
            //    return RedirectToAction("Index", "Vehicles");
            //}

            if(!(await this.vehicleService.AllAsync()).Any(v => v.Id == vehicleId))
            {
                return this.BadRequest();
            }

            return this.View(new VignetteFormViewModel 
                        { 
                            VehicleId = vehicleId 
                        });
        }

        [HttpPost]
        public async Task<IActionResult> Add(VignetteFormViewModel vignetteModel)
        {
            if (!ModelState.IsValid)
            {                
                return this.View(vignetteModel);
            }

            var vehicleVignette = await this.vignettesService
                .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vignetteModel.VehicleId);

            if (vehicleVignette == null)
            {
                TempData.AddFailureMessage(FailureAddItemMessage);
                return BadRequest();
            }

            //if (vignetteModel.StartDate < DateTime.UtcNow.AddYears(-1) 
            //    || vehicleVignette.StartDate >= vehicleVignette.EndDate
            //    || vehicleVignette.EndDate <= DateTime.UtcNow
            //    )
            //        return BadRequest();

            var newVignette = this.mapper.Map<VignetteFormServiceModel>(vignetteModel);
            await this.vignettesService.AddVignetteAsync(newVignette.VehicleId, newVignette);

            this.TempData.AddSuccessMessage(SuccessAddItemMessage);

           return RedirectToAction("Index", "Vehicles");
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int vignetteId)
        {
            return this.View();
        }

        public async Task<IActionResult> ListVehicleVignetes([FromRoute(Name = "id")] int vehicleId)
        {
            var vehicleVignettes = await this.vignettesService
                    .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId);

            var vehicleVignettesModel = new VignetteListingViewModel
            {
                AllVignettes = vehicleVignettes.OrderByDescending(vg => vg.EndDate)
            };

            return this.View(vehicleVignettesModel);
        }
    }
}
