using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebStore.Domain.DTO.Order;
using WebStore.Domain.ViewModels.Order;
using WebStore.Interfaces.Services;

namespace WebStore.ServiceHosting.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class OrdersController : ControllerBase, IOrderService
	{
		private readonly IOrderService _orderService;

		public OrdersController(IOrderService orderService) => _orderService = orderService;

		[HttpGet("user/{UserName}")]
		public IEnumerable<OrderDTO> GetUserOrders(string userName)
		{
			return _orderService.GetUserOrders(userName);
		}

		[HttpGet("{id}"), ActionName("Get")]
		public OrderDTO GetOrderById(int id)
		{
			return _orderService.GetOrderById(id);
		}

		[HttpPost("{UserName?}")]
		public OrderDTO CreateOrder(CreateOrderModel orderModel, string userName)
		{
			return _orderService.CreateOrder(orderModel, userName);
		}
	}
}
