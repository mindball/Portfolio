using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vehicles")]
    [Produces("application/json")]
    public class VehicleApiController : ControllerBase
    {
        private readonly IVehicleService vehicleService;

        public VehicleApiController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleListingServiceModel>>> Get()
        {
            var vehicles = await vehicleService.AllAsync();

            if (vehicles == null || vehicles.Count() == 0)
            {
                return this.NotFound();
            }

            return this.Ok(vehicles.ToList().OrderByDescending(v => v.Id));
        }

        [HttpGet("{vehicleid:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleListingServiceModel>> GetVehicleItem(int vehicleId)
        {
            var vehicle = await vehicleService.GetByIdAsync<VehicleListingServiceModel>(vehicleId);

            if (vehicle == null)
            {
                return this.NotFound();
            }

            return this.Ok(vehicle);
        }


    }
}
