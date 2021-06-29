using CarTrade.Services.Brands;
using CarTrade.Web.Infrastructure.Extensions;
using CarTrade.Web.Models.Brands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    public class BrandsController : ManagerController
    {
        private readonly IBrandService brandService;

        public BrandsController(IBrandService brandService)
        {
            this.brandService = brandService;
        }
        public async Task<IActionResult> Index()
        {
            var allBrands = await this.brandService.AllAsync();
            return this.View(new BrandListingViewModel
            {
                Brands = allBrands
            });
        }

        public IActionResult Add()
            => this.View();

        [HttpPost]
        public async Task<IActionResult> Add(AddBrandViewModel brandModel)
        {
            //TODO: make brand add friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureAddItemMessage, brandModel.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await brandService.AddBrandAsync(brandModel.Name);

            this.TempData.AddSuccessMessage(string.Format(SuccessAddItemMessage, brandModel.Name));
            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int brandId)
        {
            var brand = await this.brandService.GetByIdAsync(brandId);

            if (brand == null) return this.BadRequest();

            var editBranch = new BrandDetailViewModel
            {
                Name = brand.Name,                
            };

            return this.View(editBranch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandDetailViewModel brandModel)
        {
            //TODO: make brand edit friendly error page
            if (!ModelState.IsValid)
            {
                this.TempData.AddFailureMessage(string.Format(FailureEditItemMessage, brandModel.Name));
                return this.RedirectToAction(nameof(Index));
                //return this.BadRequest();
            }

            await this.brandService.EditAsync(brandModel.Id, brandModel.Name);

            this.TempData.AddSuccessMessage(string.Format(SuccessEditItemMessage, brandModel.Name));
            return this.RedirectToAction(nameof(Index));
        }
    }
}
