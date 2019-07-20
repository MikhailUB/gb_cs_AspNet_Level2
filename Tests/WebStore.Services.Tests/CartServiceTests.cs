using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Domain.DTO.Product;
using WebStore.Domain.Entities;
using WebStore.Domain.Models;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using Assert = Xunit.Assert;

namespace WebStore.Services.Tests
{
	[TestClass]
	public class CartServiceTests
	{
		[TestMethod]
		public void Cart_Class_ItemsCount_Returns_Correct_Quantity()
		{
			const int expectedCount = 5;

			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem { ProductId = 1, Quantity = 2 },
					new CartItem { ProductId = 2, Quantity = 3 }
				}
			};
			var result = cart.ItemsCount;

			Assert.Equal(expectedCount, result);
		}

		[TestMethod]
		public void CartViewModel_Returns_Correct_ItemsCount()
		{
			const int expectedCount = 6;

			var cartViewModel = new CartViewModel
			{
				Items = new Dictionary<ProductViewModel, int>
				{
					{ new ProductViewModel { Id = 1, Name = "Item 1", Price = 0.5m }, 1 },
					{ new ProductViewModel { Id = 2, Name = "Item 2", Price = 1.5m }, 2 },
					{ new ProductViewModel { Id = 3, Name = "Item 3", Price = 2.5m }, 3 },
				}
			};
			var result = cartViewModel.ItemsCount;

			Assert.Equal(expectedCount, result);
		}

		[TestMethod]
		public void CartService_AddToCart_WorkCorrect()
		{
			var cart = new Cart
			{
				Items = new List<CartItem>()
			};

			var productDataMock = new Mock<IProductData>();
			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);

			const int expectedId = 5;
			cartService.AddToCart(expectedId);

			Assert.Equal(1, cart.ItemsCount);
			Assert.Single(cart.Items);
			Assert.Equal(expectedId, cart.Items[0].ProductId);
		}

		[TestMethod]
		public void CartService_RemoveFromCart_Remove_Correct_Item()
		{
			const int itemId = 1;
			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem{ ProductId = itemId, Quantity = 1 },
					new CartItem { ProductId = 2, Quantity = 3 }
				}
			};

			var productDataMock = new Mock<IProductData>();
			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);
			cartService.RemoveFromCart(itemId);

			Assert.Single(cart.Items);
			Assert.Equal(2, cart.Items[0].ProductId);
		}

		[TestMethod]
		public void CartService_RemoveAll_ClearCart()
		{
			const int itemId = 1;
			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem{ ProductId = itemId, Quantity = 1 },
					new CartItem { ProductId = 2, Quantity = 3 }
				}
			};

			var productDataMock = new Mock<IProductData>();
			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);
			cartService.RemoveAll();

			Assert.Empty(cart.Items);
		}

		[TestMethod]
		public void CartService_Decrement_Correct()
		{
			const int itemId = 1;
			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem{ ProductId = itemId, Quantity = 3 },
					new CartItem { ProductId = 2, Quantity = 5 }
				}
			};

			var productDataMock = new Mock<IProductData>();
			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);
			cartService.DecrementFromCart(itemId);

			Assert.Equal(7, cart.ItemsCount);
			Assert.Equal(2, cart.Items.Count);
			Assert.Equal(itemId, cart.Items[0].ProductId);
			Assert.Equal(2, cart.Items[0].Quantity);
		}

		[TestMethod]
		public void CartService_Remove_Item_When_Decrement()
		{
			const int itemId = 1;
			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem{ ProductId = itemId, Quantity = 1 },
					new CartItem { ProductId = 2, Quantity = 5 }
				}
			};

			var productDataMock = new Mock<IProductData>();
			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);
			cartService.DecrementFromCart(itemId);

			Assert.Equal(5, cart.ItemsCount);
			Assert.Single(cart.Items);
		}

		[TestMethod]
		public void CartService_TransformCart_WorksCorrect()
		{
			var cart = new Cart
			{
				Items = new List<CartItem>
				{
					new CartItem{ ProductId = 1, Quantity = 1 },
					new CartItem { ProductId = 2, Quantity = 5 }
				}
			};

			var products = new List<ProductDTO>
			{
				new ProductDTO
				{
					Id = 1,
					Name = "Product 1",
					ImageUrl = "Image1.png",
					Order = 0,
					Price = 1.1m
				},
				new ProductDTO
				{
					Id = 2,
					Name = "Product 2",
					ImageUrl = "Image2.png",
					Order = 1,
					Price = 2.1m
				}
			};

			var productDataMock = new Mock<IProductData>();
			productDataMock
				.Setup(c => c.GetProducts(It.IsAny<ProductFilter>()))
				.Returns(new PagedProductsDTO { Products = products, TotalCount = products.Count });

			var cartStoreMock = new Mock<ICartStore>();
			cartStoreMock
				.Setup(c => c.Cart)
				.Returns(cart);

			var cartService = new CartService(productDataMock.Object, cartStoreMock.Object);
			var result = cartService.TransformCart();

			Assert.Equal(6, result.ItemsCount);
			Assert.Equal(1.1m, result.Items.First().Key.Price);
		}
	}
}
