using System.Collections.Generic;
using WebStore.Domain.DTO.Order;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Order;

namespace WebStore.Interfaces.Services
{
	public interface IOrderService
	{
		IEnumerable<OrderDTO> GetUserOrders(string userName);

		OrderDTO GetOrderById(int id);

		OrderDTO CreateOrder(CreateOrderModel orderModel, string userName);
	}
}
