using CarTrade.Services.Companies;
using CarTrade.Web.Models.Companies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompaniesService companiesServices;

        public CompaniesController(ICompaniesService companiesServices)
        {
            this.companiesServices = companiesServices;
        }
        public async Task<IActionResult> Index()
        {
            var allCompanies = await this.companiesServices.AllAsync();

            return this.View(new CompanyListingViewModel
            {
                Companies = allCompanies
            });
        }

        public IActionResult Add()
           => this.View();

        [HttpPost]
        public async Task<IActionResult> Add(AddCompanyViewModel company)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await companiesServices.AddCompanyAsync(company.Name);

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int companyId)
        {
            var company = await this.companiesServices.GetByIdAsync(companyId);

            if (company == null) return this.BadRequest();

            var editCompany = new CompanyDetailViewModel
            {
                Name = company.Name,
            };

            return this.View(editCompany);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CompanyDetailViewModel companyModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await this.companiesServices.EditAsync(companyModel.Id, companyModel.Name);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
