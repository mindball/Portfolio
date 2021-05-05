using CarTrade.Services.Vehicle;
using CarTrade.Services.Vehicle.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IVehicleService vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await this.vehicleService.AllAsync();

            return View(vehicles);
        }

        public IActionResult Add()
            =>  this.View();

        [HttpPost]
        public async Task<IActionResult> Add(AddVehicleServiceModel vehicleModel)
        {
            if(!ModelState.IsValid)
            {
                return this.View(vehicleModel);
            }

            await this.vehicleService.AddVehicleAsync(vehicleModel);

            return this.RedirectToAction(nameof(Index));
        }

    }
}
