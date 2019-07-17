using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using WebStore.Services.Map;

namespace WebStore.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly IProductData _productData;

		public BrandsViewComponent(IProductData productData)
		{
			_productData = productData;
		}

		public IViewComponentResult Invoke(string brandId)
		{
			var brndId = int.TryParse(brandId, out int id) ? id : (int?)null;

			return View(new BrandCompleteViewModel { Brands = GetBrands(), CurrentBrandId = brndId });
		}

		private IEnumerable<BrandViewModel> GetBrands()
		{
			return _productData.GetBrands().Select(b => b.CreateModel());
		}
	}
}
