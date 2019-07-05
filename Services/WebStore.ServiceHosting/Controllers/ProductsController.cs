using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.ServiceHosting.Controllers
{
	//[Route("api/[controller]")]
	[Route("api/products")]
	[ApiController]
	[Produces("application/json")]
	public class ProductsController : ControllerBase, IProductData
	{
		private readonly IProductData _productData;

		public ProductsController(IProductData productData) => _productData = productData;

		[HttpGet("sections")]
		public IEnumerable<Section> GetSections() => _productData.GetSections();

		[HttpGet("brands")]
		public IEnumerable<Brand> GetBrands() => _productData.GetBrands();

		[HttpPost, ActionName("Post")]
		public IEnumerable<ProductDTO> GetProducts([FromBody] ProductFilter filter) => _productData.GetProducts(filter);

		[HttpGet("{id}"), ActionName("Get")]
		public ProductDTO GetProductById(int id) => _productData.GetProductById(id);
	}
}
