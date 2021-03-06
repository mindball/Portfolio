using AutoMapper;
using CarTrade.Services.InsuranceCompanies;
using CarTrade.Services.InsuranceCompanies.Models;
using CarTrade.Services.InsurancePolicies;
using CarTrade.Services.InsurancePolicies.Models;
using CarTrade.Services.Vehicles;
using CarTrade.Services.Vehicles.Models;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.InsurancePolicies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Mvc.TypedRouting;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class InsurancesPolicesController : ManagerController
    {
        private IInsurancesPoliciesService policyService;
        private IVehicleService vehicleService;
        private IInsuranceCompaniesService insuranceCompanyService;
        private IMapper mapper;

        public InsurancesPolicesController(IInsurancesPoliciesService policyService,
            IVehicleService vehicleService,
            IInsuranceCompaniesService insuranceCompany,
            IMapper mapper)
        {
            this.policyService = policyService;
            this.vehicleService = vehicleService;
            this.insuranceCompanyService = insuranceCompany;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Add([FromRoute(Name = "id")] int vehicleId)
             => this.View(new InsurancePolicyFormViewModel
             {
                 InsuranceCompanies = await GetInsuranceCompanies(),
                 StartDate = DateTime.UtcNow,
                 EndDate = DateTime.UtcNow
             });

        [HttpPost]
        public async Task<IActionResult> Add(
            [FromRoute(Name = "id")] int vehicleId,
            InsurancePolicyFormViewModel policyModel)
        {
            if (!ModelState.IsValid)
            {
                policyModel.InsuranceCompanies = await GetInsuranceCompanies();
                return this.View(policyModel);
            }

            var newPolicy = this.mapper.Map<InsurancePolicyFormServiceModel>(policyModel);

            await this.policyService.AddPolicyAsync(vehicleId, newPolicy);

            var currentVehiclePolicyAsigned =
                (await this.vehicleService.AllAsync()).Where(v => v.Id == vehicleId).FirstOrDefault();
            
            string policyInfo =
                string.Join(' ',
                policyModel.TypeInsurance.ToString(),                
                "to",
                currentVehiclePolicyAsigned.PlateNumber
                );

            TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, policyInfo));

            //magic strings
            //return this.RedirectToRoute("default",
            //    new { controller = "Vehicles", action = "Index" });
            //var result = this.RedirectToAction<VehiclesController>(c => c.Index(1));
            //var result2 = this.RedirectToRoute("default",
            //        new { controller = "Vehicles", action = "Index" });

            return this.RedirectToAction<VehiclesController>(c => c.Index(1));
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
                ListDetailsInsurancePolicyViewModel vehicleInsurances = FillCustomVehicle(vehicle);

                vehicleInsurances.PolicyDetails = this.mapper
                            .Map<IEnumerable<DetailInsurancePolicyViewModel>>(insurancePolicies);

                return View(vehicleInsurances);
            }



            return View();
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int policyId)
        {
            var policy = (await this.policyService
                .GetByIdAsync<InsurancePolicyFormViewModel>(policyId));                

            if (policy == null)
            {
                return this.BadRequest();
            }

            policy.InsuranceCompanies = await GetInsuranceCompanies();

            return this.View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int policyId, 
            InsurancePolicyFormViewModel policyModel)
        {
            if(!ModelState.IsValid)
            {
                policyModel.InsuranceCompanies = await GetInsuranceCompanies();
                return this.View(policyModel);
            }

            var editPolicy = this.mapper.Map<InsurancePolicyFormServiceModel>(policyModel);
            await this.policyService.EditPolicyAsync(policyId, editPolicy);

            TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, "policy"));

            //magic strings
            //return this.RedirectToRoute("default",
            //    new { controller = "Vehicles", action = "Index" });

            return this.RedirectToAction<VehiclesController>(c => c.Index(1));

        }

        private ListDetailsInsurancePolicyViewModel FillCustomVehicle(VehicleListingServiceModel vehicle)
            => this.mapper
                .Map<VehicleListingServiceModel, ListDetailsInsurancePolicyViewModel>(vehicle, opt =>
                            opt.ConfigureMap()
                            .ForMember(l => l.VehicleId, cfg => cfg.MapFrom(x => x.Id))
                            .ForMember(l => l.VehicleModel, cfg => cfg.MapFrom(x => x.Model))
                            .ForMember(l => l.PolicyDetails, opt
                                => opt.Ignore())
                    );

        private async Task<IEnumerable<SelectListItem>> GetInsuranceCompanies()
        {
            var companies = await this.insuranceCompanyService.AllAsync();

            return companies.Select(t => new SelectListItem
                        {
                            Text = t.Name,
                            Value = t.Id.ToString()
                        })
                    .ToList();
        }
    }
}
