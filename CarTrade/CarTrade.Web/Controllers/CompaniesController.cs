using CarTrade.Services.Companies;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Companies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class CompaniesController : BaseController
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
            //TODO: make company add friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, company.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await companiesServices.AddCompanyAsync(company.Name);

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, company.Name));
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
            //TODO: make company edit friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureEditItemMessage, companyModel.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await this.companiesServices.EditAsync(companyModel.Id, companyModel.Name);
            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, companyModel.Name));
            return this.RedirectToAction(nameof(Index));
        }
    }
}
