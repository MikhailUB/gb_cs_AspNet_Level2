using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using WebStore.Controllers;
using WebStore.Domain.DTO.Order;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Order;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
	[TestClass]
	public class CartControllerTests
	{
		[TestMethod]
		public void CheckOut_ModelState_Invalid_Returns_ViewModel()
		{
			var cartServiceMock = new Mock<ICartService>();
			var orderServiceMock = new Mock<IOrderService>();

			var controller = new CartController(cartServiceMock.Object, orderServiceMock.Object);

			controller.ModelState.AddModelError("TestError", "Invalid Model on unit testing");

			const string expectedModelName = "Test order";

			var result = controller.CheckOut(new OrderViewModel { Name = expectedModelName });

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<DetailsViewModel>(viewResult.ViewData.Model);

			Assert.Equal(expectedModelName, model.OrderViewModel.Name);
		}

		[TestMethod]
		public void CheckOut_Calls_Service_and_Return_Redirect()
		{
			var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }));

			var cartServiceMock = new Mock<ICartService>();
			cartServiceMock
				.Setup(c => c.TransformCart())
				.Returns(() => new CartViewModel
				{
					Items = new Dictionary<ProductViewModel, int>
					{
						{ new ProductViewModel(), 1 }
					}
				});

			const int expectedOrderId = 1;

			var orderServiceMock = new Mock<IOrderService>();
			orderServiceMock
				.Setup(c => c.CreateOrder(It.IsAny<CreateOrderModel>(), It.IsAny<string>()))
				.Returns(new OrderDTO { Id = expectedOrderId });

			var controller = new CartController(cartServiceMock.Object, orderServiceMock.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext { User = user }
				}
			};

			var result = controller.CheckOut(new OrderViewModel
			{
				Name = "Test",
				Address = "",
				Phone = ""
			});

			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectResult.ControllerName);
			Assert.Equal(nameof(CartController.OrderConfirmed), redirectResult.ActionName);

			Assert.Equal(expectedOrderId, redirectResult.RouteValues["id"]);
		}
	}
}
