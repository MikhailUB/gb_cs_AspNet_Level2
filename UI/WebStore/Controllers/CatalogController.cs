using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using WebStore.Services.Map;

namespace WebStore.Controllers
{
	public class CatalogController : Controller
	{
		private readonly IProductData _productData;

		public CatalogController(IProductData ProductData)
		{
			_productData = ProductData;
		}

		public IActionResult Shop(int? sectionId, int? brandId)
		{
			var products = _productData.GetProducts(new ProductFilter
			{
				SectionId = sectionId,
				BrandId = brandId
			});

			var model = new CatalogViewModel
			{
				BrandId = brandId,
				SectionId = sectionId,
				Products = products
					.Select(p => new ProductViewModel
					{
						Id = p.Id,
						Name = p.Name,
						Brand = p.Brand?.Name,
						Order = p.Order,
						Price = p.Price,
						ImageUrl = p.ImageUrl
					})
				//.Select(p => p.CreateViewModel())
			};

			return View(model);
		}

		public IActionResult ProductDetails(int id)
		{
			var prod = _productData.GetProductById(id);
			if (prod == null)
				return NotFound();

			return View(new ProductViewModel
			{
				Id = prod.Id,
				Name = prod.Name,
				Price = prod.Price,
				Order = prod.Order,
				ImageUrl = prod.ImageUrl,
				Brand = prod.Brand?.Name
			});
		}

	}
}
