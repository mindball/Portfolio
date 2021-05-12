using AutoMapper;
using CarTrade.Services.InsuranceCompany;
using CarTrade.Services.InsurancePolicy;
using CarTrade.Services.Vehicle;
using CarTrade.Services.Vehicle.Models;
using CarTrade.Web.Models.InsurancePolice;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class InsurancesPolicesController : BaseController
    {
        private IInsurancesPoliciesService policyService;
        private IVehicleService vehicleService;
        private IInsuranceCompaniesService insuranceCompany;
        private IMapper mapper;

        public InsurancesPolicesController(IInsurancesPoliciesService policyService,
            IVehicleService vehicleService,
            IInsuranceCompaniesService insuranceCompany,
            IMapper mapper)
        {
            this.policyService = policyService;
            this.vehicleService = vehicleService;
            this.insuranceCompany = insuranceCompany;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Details([FromRoute(Name = "id")] int vehicleId)
        {
            var vehicle = await this.vehicleService.GetByIdAsync<VehicleListingServiceModel>(vehicleId);
            if (vehicle == null)
            {
                return this.BadRequest();
            }

            var insurancePolicies =
                await this.policyService.GetAllInsuranceByVehicleId(vehicleId);

            //Case new vehicle 
            if (insurancePolicies != null || insurancePolicies.Count() != 0)
            {
                ListDetailsInsurancePoliceViewModel vehicleInsurances = FillCustomVehicle(vehicle);

                vehicleInsurances.PolicyDetails = this.mapper
                            .Map<IEnumerable<DetailInsurancePolicyViewModel>>(insurancePolicies);

                return View(vehicleInsurances);
            }



            return View();
        }

        private ListDetailsInsurancePoliceViewModel FillCustomVehicle(VehicleListingServiceModel vehicle)
            => this.mapper
                .Map<VehicleListingServiceModel, ListDetailsInsurancePoliceViewModel>(vehicle, opt =>
                            opt.ConfigureMap()
                            .ForMember(l => l.VehicleId, cfg => cfg.MapFrom(x => x.Id))
                            .ForMember(l => l.VehicleModel, cfg => cfg.MapFrom(x => x.Model))
                            .ForMember(l => l.PolicyDetails, opt
                                => opt.Ignore())
                    );
        
    }
}
