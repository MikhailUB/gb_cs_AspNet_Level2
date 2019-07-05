﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebStore.Domain.DTO.Order;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Order;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
	public class CartController : Controller
	{
		private readonly ICartService _cartService;
		private readonly IOrderService _orderService;

		public CartController(ICartService cartService, IOrderService orderService)
		{
			_cartService = cartService;
			_orderService = orderService;
		}

		public IActionResult Details()
		{
			var model = new DetailsViewModel
			{
				CartViewModel = _cartService.TransformCart(),
				OrderViewModel = new OrderViewModel()
			};
			return View(model);
		}

		public IActionResult DecrementFromCart(int id)
		{
			_cartService.DecrementFromCart(id);
			return RedirectToAction("Details");
		}

		public IActionResult RemoveFromCart(int id)
		{
			_cartService.RemoveFromCart(id);
			return RedirectToAction("Details");
		}

		public IActionResult RemoveAll()
		{
			_cartService.RemoveAll();
			return RedirectToAction("Details");
		}

		public IActionResult AddToCart(int id)
		{
			_cartService.AddToCart(id);
			return RedirectToAction("Details");
		}

		[HttpPost, ValidateAntiForgeryToken]
		public IActionResult CheckOut(OrderViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var detailsModel = new DetailsViewModel
				{
					CartViewModel = _cartService.TransformCart(),
					OrderViewModel = model
				};
				return View(nameof(Details), detailsModel);
			}

			var createModel = new CreateOrderModel
			{
				OrderViewModel = model,
				OrderItems = _cartService.TransformCart().Items.Select(i => new OrderItemDTO
				{
					Id = i.Key.Id,
					Price = i.Key.Price,
					Quantity = i.Value
				}).ToList()
			};

			var order = _orderService.CreateOrder(createModel, User.Identity.Name);
			_cartService.RemoveAll();

			return RedirectToAction("OrderConfirmed", new { id = order.Id });
		}

		public IActionResult OrderConfirmed(int id)
		{
			ViewBag.OrderId = id;
			return View();
		}

	}
}
