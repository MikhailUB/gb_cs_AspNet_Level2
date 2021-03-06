﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebStore.Domain.ViewModels.Order;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly IOrderService _orderService;

		public ProfileController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		public IActionResult Index() => View();

		public IActionResult Orders()
		{
			var orders = _orderService.GetUserOrders(User.Identity.Name);
			var models = orders.Select(order => new UserOrderViewModel
			{
				Id = order.Id,
				Name = order.Name,
				Address = order.Address,
				Phone = order.Phone,
				TotalSum = order.OrderItem.Sum(o => o.Quantity * o.Price)
			});
			return View(models);
		}
	}
}
