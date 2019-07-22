using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.Services.Map;

namespace WebStore.Services
{
	public class SqlProductData : IProductData
	{
		private readonly WebStoreContext _db;

		public SqlProductData(WebStoreContext db)
		{
			_db = db;
		}

		public IEnumerable<Section> GetSections() => _db.Sections
			//.Include(s => s.Products)
			.AsEnumerable();

		public IEnumerable<Brand> GetBrands() => _db.Brands
			//.Include(brand => brand.Products)
			.AsEnumerable();

		public Section GetSectionById(int id) => _db.Sections.FirstOrDefault(s => s.Id == id);

		public Brand GetBrandById(int id) => _db.Brands.FirstOrDefault(b => b.Id == id);

		public PagedProductsDTO GetProducts(ProductFilter filter)
		{
			IQueryable<Product> products = _db.Products;

			if (filter?.SectionId != null)
				products = products.Where(product => product.SectionId == filter.SectionId);

			if (filter?.BrandId != null)
				products = products.Where(product => product.BrandId == filter.BrandId);

			var totalCount = products.Count();

			if (filter?.PageSize != null)
				products = products
					.Skip((filter.Page - 1) * (int)filter.PageSize)
					.Take(filter.PageSize.Value);

			return new PagedProductsDTO
			{
				Products = products.AsEnumerable().ToDTO(),
				TotalCount = totalCount
			};
		}

		public ProductDTO GetProductById(int id)
		{
			return _db.Products
				.Include(prod => prod.Brand)
				.Include(prod => prod.Section)
				.FirstOrDefault(product => product.Id == id)
				.ToDTO();
		}
	}
}
