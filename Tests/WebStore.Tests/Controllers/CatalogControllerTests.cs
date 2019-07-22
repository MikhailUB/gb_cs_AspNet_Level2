using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using WebStore.Controllers;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
	[TestClass]
	public class CatalogControllerTests
	{
		[TestMethod]
		public void ProductDetails_Returns_View_With_Correct_Item()
		{
			// Arrange-Act-Assert (A-A-A)

			#region Arrange
			// Подготовка данных для тестирования + данных для проверки

			var expected = new
			{
				Id = 1,
				Name = $"Item id 1",
				Price = 10m,
				BrandName = $"Brand of item id 1"
			};

			var productDataMock = new Mock<IProductData>();
			productDataMock
				.Setup(p => p.GetProductById(It.IsAny<int>()))
				.Returns<int>(id => new ProductDTO
				{
					Id = id,
					Name = $"Item id {id}",
					ImageUrl = $"Image_id_{id}.png",
					Order = 0,
					Price = expected.Price,
					Brand = new BrandDTO { Id = 1, Name = $"Brand of item id {id}" }
				});

			var configurationMock = new Mock<IConfiguration>();
			var controller = new CatalogController(productDataMock.Object, configurationMock.Object);

			#endregion

			#region Act
			// Процеесс вызова тестируемого кода

			var result = controller.ProductDetails(expected.Id);

			#endregion

			#region Assert
			// Проверка полученых результатов

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ProductViewModel>(viewResult.ViewData.Model);

			Assert.Equal(expected.Id, model.Id);
			Assert.Equal(expected.Name, model.Name);
			Assert.Equal(expected.Price, model.Price);
			Assert.Equal(expected.BrandName, model.Brand);

			#endregion
		}

		[TestMethod, Description("Описание модульного теста"), Timeout(700), Priority(1)]
		public void ProductDetails_Returns_NotFound()
		{
			var productDataMock = new Mock<IProductData>();
			productDataMock
				.Setup(p => p.GetProductById(It.IsAny<int>()))
				.Returns(default(ProductDTO));

			var configurationMock = new Mock<IConfiguration>();
			var controller = new CatalogController(productDataMock.Object, configurationMock.Object);

			var result = controller.ProductDetails(1);

			Assert.IsType<NotFoundResult>(result);
		}

		[TestMethod]
		public void Shop_Returns_Correct_View()
		{
			var resultProducts = new PagedProductsDTO
			{
				TotalCount = 3,
				Products = new[]
				{
					new ProductDTO
					{
						Id = 1,
						Name = "Product 1",
						Order = 0,
						ImageUrl = "Product1.png",
						Price= 10m,
						Brand = new BrandDTO { Id = 1, Name = "Brand of Product 1" }
					},
					new ProductDTO
					{
						Id = 2,
						Name = "Product 2",
						Order = 0,
						ImageUrl = "Product2.png",
						Price= 10m,
						Brand = new BrandDTO { Id = 1, Name = "Brand of Product 2" }
					},
					new ProductDTO
					{
						Id = 3,
						Name = "Product 3",
						Order = 0,
						ImageUrl = "Product3.png",
						Price= 10m,
						Brand = new BrandDTO { Id = 1, Name = "Brand of Product 3" }
					}
				}
			};

			var productDataMock = new Mock<IProductData>();
			productDataMock
				.Setup(p => p.GetProducts(It.IsAny<ProductFilter>()))
				.Returns<ProductFilter>(filter => resultProducts);

			var configurationMock = new Mock<IConfiguration>();
			var controller = new CatalogController(productDataMock.Object, configurationMock.Object);

			var expected = new { SectionId = 1, BrandId = 5 };

			var result = controller.Shop(expected.SectionId, expected.BrandId);

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<CatalogViewModel>(viewResult.ViewData.Model);

			Assert.Equal(resultProducts.Products.Count(), model.Products.Count());
			Assert.Equal(expected.BrandId, model.BrandId);
			Assert.Equal(expected.SectionId, model.SectionId);

			Assert.Equal(resultProducts.Products.First().Brand.Name, model.Products.First().Brand);
		}
	}
}
