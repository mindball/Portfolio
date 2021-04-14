using CarTrade.Services.InsuranceCompany;
using CarTrade.Web.Models.InsuranceCompanies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class InsuranceCompaniesController : Controller
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
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await insuranceCompaniesService.AddInsuranceCompanyAsync(insuranceCompany.Name);

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
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await this.insuranceCompaniesService.EditAsync(companyModel.Id, companyModel.Name);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
