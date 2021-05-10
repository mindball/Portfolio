using CarTrade.Services.InsuranceCompany;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.InsuranceCompanies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class InsuranceCompaniesController : BaseController
    {
        private readonly IInsuranceCompaniesService insuranceCompaniesService;

        public InsuranceCompaniesController(IInsuranceCompaniesService insuranceCompaniesService)
        {
            this.insuranceCompaniesService = insuranceCompaniesService;
        }
        public async Task<IActionResult> Index()
        {
            var allCompanies = await this.insuranceCompaniesService.AllAsync();

            return this.View(new InsuranceCompanyListingViewModel
            {
                InsuranceCompanies = allCompanies
            }); 
        }

        public IActionResult Add()
          => this.View();

        [HttpPost]
        public async Task<IActionResult> Add(AddInsuranceCompanyViewModel insuranceCompany)
        {
            //TODO: make insurance company add friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, insuranceCompany.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await insuranceCompaniesService.AddInsuranceCompanyAsync(insuranceCompany.Name);

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, insuranceCompany.Name));
            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int insuranceCompanyId)
        {
            var insuranceCompany = await this.insuranceCompaniesService.GetByIdAsync(insuranceCompanyId);
            if (insuranceCompany == null) return this.BadRequest();

            var editinsuranceCompany = new InsuranceCompanyDetailViewModel
            {
                Name = insuranceCompany.Name,
            };

            return this.View(editinsuranceCompany);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InsuranceCompanyDetailViewModel companyModel)
        {
            //TODO: make insurance company add friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureEditItemMessage, companyModel.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await this.insuranceCompaniesService.EditAsync(companyModel.Id, companyModel.Name);

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, companyModel.Name));
            return this.RedirectToAction(nameof(Index));
        }
    }
}
