using System.Collections.Generic;
using System.Linq;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;
using WebStore.Services.Map;

namespace WebStore.Services
{
	public class InMemoryProductData : IProductData
	{
		public IEnumerable<Section> GetSections() => TestData.Sections;

		public IEnumerable<Brand> GetBrands() => TestData.Brands;

		public Section GetSectionById(int id) => TestData.Sections.FirstOrDefault(s => s.Id == id);

		public Brand GetBrandById(int id) => TestData.Brands.FirstOrDefault(b => b.Id == id);

		public PagedProductsDTO GetProducts(ProductFilter filter)
		{
			IEnumerable<Product> products = TestData.Products;
			if (filter?.BrandId != null)
				products = products.Where(product => product.BrandId == filter.BrandId);

			if (filter?.SectionId != null)
				products = products.Where(product => product.SectionId == filter.SectionId);

			return new PagedProductsDTO { Products = products.ToDTO() };
		}

		public ProductDTO GetProductById(int id) => TestData.Products.FirstOrDefault(product => product.Id == id)?.ToDTO();
	}
}
