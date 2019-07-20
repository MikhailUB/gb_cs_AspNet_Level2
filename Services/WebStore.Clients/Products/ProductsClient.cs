using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using WebStore.Clients.Base;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Clients.Products
{
	public class ProductsClient : BaseClient, IProductData
	{
		public ProductsClient(IConfiguration configuration) : base(configuration, "api/products") { }

		public IEnumerable<Section> GetSections()
		{
			return Get<List<Section>>($"{_serviceAddress}/sections");
		}

		public IEnumerable<Brand> GetBrands()
		{
			return Get<List<Brand>>($"{_serviceAddress}/brands");
		}

		public Section GetSectionById(int id)
		{
			return Get<Section>($"{_serviceAddress}/sections/{id}");
		}

		public Brand GetBrandById(int id)
		{
			return Get<Brand>($"{_serviceAddress}/brands/{id}");
		}

		public PagedProductsDTO GetProducts(ProductFilter filter)
		{
			var response = Post(_serviceAddress, filter);
			return response.Content.ReadAsAsync<PagedProductsDTO>().Result;
		}

		public ProductDTO GetProductById(int id)
		{
			return Get<ProductDTO>($"{_serviceAddress}/{id}");
		}
	}
}
