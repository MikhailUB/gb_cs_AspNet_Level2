using System.Collections.Generic;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Order;

namespace WebStore.Interfaces.Services
{
	public interface IOrderService
	{
		IEnumerable<Order> GetUserOrders(string userName);

		Order GetOrderById(int id);

		Order CreateOrder(OrderViewModel orderModel, CartViewModel cartModel, string userName);
	}
}
