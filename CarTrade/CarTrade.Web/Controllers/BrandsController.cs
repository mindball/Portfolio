using CarTrade.Services.Brand;
using CarTrade.Web.Models.Brands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarTrade.Web.Controllers
{
    public class BrandsController : Controller
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
        public async Task<IActionResult> Add(AddBrandViewModel brand)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await brandService.AddBrandAsync(brand.Name);

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute(Name = "id")] int brandId)
        {
            var brand = await this.brandService.GetByIdAsync(brandId);
            var editBranch = new BrandDetailViewModel
            {
                Name = brand.Name,                
            };

            return this.View(editBranch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandDetailViewModel brandModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            await this.brandService.EditAsync(brandModel.Id, brandModel.Name);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
