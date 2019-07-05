using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

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

		public IEnumerable<ProductDTO> GetProducts(ProductFilter filter)
		{
			IQueryable<Product> products = _db.Products;
			if (!(filter is null))
			{
				if (filter.SectionId != null)
					products = products.Where(product => product.SectionId == filter.SectionId);

				if (filter.BrandId != null)
					products = products.Where(product => product.BrandId == filter.BrandId);
			}
			return products.AsEnumerable().Select(p => new ProductDTO
			{
				Id = p.Id,
				Name = p.Name,
				Order = p.Order,
				Price = p.Price,
				ImageUrl = p.ImageUrl,
				Brand = p.Brand is null ? null : new BrandDTO { Id = p.Brand.Id, Name = p.Brand.Name }
			});
		}

		public ProductDTO GetProductById(int id)
		{
			var p = _db.Products
				.Include(prod => prod.Brand)
				.Include(prod => prod.Section)
				.FirstOrDefault(prod => prod.Id == id);
			return p is null ? null : new ProductDTO
			{
				Id = p.Id,
				Name = p.Name,
				Order = p.Order,
				Price = p.Price,
				ImageUrl = p.ImageUrl,
				Brand = p.Brand is null ? null : new BrandDTO { Id = p.Brand.Id, Name = p.Brand.Name }
			};
		}
	}
}
