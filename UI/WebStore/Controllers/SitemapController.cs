using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;
using System.Collections.Generic;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
	public class SitemapController : Controller
	{
		private readonly IProductData _productData;

		public SitemapController(IProductData productData) => _productData = productData;

		public IActionResult Index()
		{
			var nodes = new List<SitemapNode>
			{
				new SitemapNode(Url.Action("Index", "Home")),
				new SitemapNode(Url.Action("Shop", "Catalog")),
				new SitemapNode(Url.Action("Blog", "Home")),
				new SitemapNode(Url.Action("BlogSingle", "Home")),
				new SitemapNode(Url.Action("ContactUs", "Home")),
			};

			var sections = _productData.GetSections();

			foreach (var section in sections)
			{
				if (section.ParentId == null)
					nodes.Add(new SitemapNode(Url.Action("Shop", "Catalog", new { SectionId = section.Id })));
			}
			var brands = _productData.GetBrands();
			foreach (var brand in brands)
				nodes.Add(new SitemapNode(Url.Action("Shop", "Catalog", new { BrandId = brand.Id })));

			var products = _productData.GetProducts(new ProductFilter());

			foreach (var product in products)
				nodes.Add(new SitemapNode(Url.Action("ProductDetails", "Catalog", new { id = product.Id })));

			return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
		}
	}
}
