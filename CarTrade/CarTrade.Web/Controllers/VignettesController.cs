using AutoMapper;
using CarTrade.Services.Vehicles;
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
    public class VignettesController : ManagerController
    {
        private readonly IVignettesService vignettesService;
        private readonly IVehicleService vehicleService;
        private readonly IMapper mapper;
        private readonly string NotAsignVignettes = $"It has not vignettes on this vihicle";
        

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
            });

        public async Task<IActionResult> ListVehicleVignetes([FromRoute(Name = "id")] int vehicleId)
        {
            var vehicleVignettes = await this.vignettesService
                    .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vehicleId);

            var vehicleVignettesModel = new VignetteListingViewModel();

            if (vehicleVignettes != null && vehicleVignettes.Count() > 0)
            {
                vehicleVignettesModel.VehicleId = vehicleId;
                vehicleVignettesModel.AllVignettes = vehicleVignettes.OrderByDescending(vg => vg.EndDate);

                return this.View(vehicleVignettesModel);
            }

            this.TempData.AddFailureMessage(NotAsignVignettes);

            return this.RedirectToAction(nameof(Add), new { id = vehicleId });
        }

        //TODO: not implement
        public async Task<IActionResult> ListBranchVignetes([FromRoute(Name = "id")] int branchId)
        {
            return this.View();
        }

        public async Task<IActionResult> Add([FromRoute(Name = "id")] int vehicleId)
        {
            if (!(await this.vehicleService.AllAsync()).Any(v => v.Id == vehicleId))
            {
                return this.BadRequest();
            }

            var vehicleVignette = await this.vignettesService.DoesVehicleHaveActiveVignetteAsync(vehicleId);

            if (vehicleVignette)// && vehicleVignette.)
            {
                TempData.AddFailureMessage(string.Format(ActiveItem, vehicleId, "vignette"));
                return this.RedirectToAction("edit", "vehicles", new { id = vehicleId });
            }

            return this.View(new VignetteFormViewModel
            {
                VehicleId = vehicleId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(VignetteFormViewModel vignetteModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(vignetteModel);
            }

            //TODO: Check is valid vignette
            var vehicleVignette = await this.vignettesService
                .GetVignetteByVehicleIdAsync<VignetteListingServiceModel>(vignetteModel.VehicleId);

            //TODO: Check is valid vignette
            //if (vehicleVignette == null)
            //{
            //    TempData.AddFailureMessage(FailureAddItemMessage);
            //    return BadRequest();
            //}

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
            var vignette = await this.vignettesService
                .GetByIdAsync<VignetteDetailViewModel>(vignetteId);

            if(vignette == null)
            {
                return this.NotFound();
            }

            return this.View(vignette);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VignetteFormDetailViewModel vignetteModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(vignetteModel);
            }

            var isActiveVignette = await this.vignettesService
                .DoesVehicleHaveActiveVignetteAsync(vignetteModel.VehicleId);

            if(isActiveVignette)
            {
                this.TempData.EditFailureMessage(FailureEditItemMessage);
            }

            var editVignette = this.mapper.Map<VignetteFormServiceModel>(vignetteModel);

            await this.vignettesService.EditAsync(vignetteModel.Id, editVignette);
            this.TempData.EditSuccessMessage(SuccessEditItemMessage);

            return RedirectToAction(nameof(Index));
        }
    }
}
