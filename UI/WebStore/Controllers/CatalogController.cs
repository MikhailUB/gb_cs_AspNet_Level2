using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
	public class CatalogController : Controller
	{
		private readonly IProductData _productData;
		private readonly IConfiguration _configuration;

		public CatalogController(IProductData productData, IConfiguration configuration)
		{
			_productData = productData;
			_configuration = configuration;
		}

		public IActionResult Shop(int? sectionId, int? brandId, int page = 1)
		{
			var pageSize = int.Parse(_configuration["PageSize"]);

			var pagedProducts = _productData.GetProducts(new ProductFilter
			{
				SectionId = sectionId,
				BrandId = brandId,
				Page = page,
				PageSize = pageSize
			});

			var model = new CatalogViewModel
			{
				BrandId = brandId,
				SectionId = sectionId,
				Products = pagedProducts.Products
					.Select(p => new ProductViewModel
					{
						Id = p.Id,
						Name = p.Name,
						Brand = p.Brand?.Name,
						Order = p.Order,
						Price = p.Price,
						ImageUrl = p.ImageUrl
					}),
				PageModel = new PageViewModel
				{
					PageSize = pageSize,
					PageNumber = page,
					TotalItems = pagedProducts.TotalCount
				}

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
