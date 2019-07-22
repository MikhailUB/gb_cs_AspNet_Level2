using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Order;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Order;
using WebStore.Interfaces.Services;
using WebStore.Services.Map;

namespace WebStore.Services
{
	public class SqlOrdersService : IOrderService
	{
		private readonly WebStoreContext _db;
		private readonly UserManager<User> _userManager;

		public SqlOrdersService(WebStoreContext db, UserManager<User> userManager)
		{
			_db = db;
			_userManager = userManager;
		}

		public IEnumerable<OrderDTO> GetUserOrders(string userName)
		{
			return _db.Orders
				.Include(order => order.User)
				.Include(order => order.OrderItems)
				.Where(order => order.User.UserName == userName)
				.ToDTO();
		}

		public OrderDTO GetOrderById(int id)
		{
			return _db.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefault(o => o.Id == id)
				.ToDTO();
		}

		public OrderDTO CreateOrder(CreateOrderModel orderModel, string userName)
		{
			var user = _userManager.FindByNameAsync(userName).Result;

			using (var transaction = _db.Database.BeginTransaction())
			{
				var order = new Order
				{
					Name = orderModel.OrderViewModel.Name,
					Address = orderModel.OrderViewModel.Address,
					Phone = orderModel.OrderViewModel.Phone,
					User = user,
					Date = DateTime.Now
				};
				_db.Orders.Add(order);

				foreach (var item in orderModel.OrderItems)
				{
					var product = _db.Products.FirstOrDefault(p => p.Id == item.Id);
					if (product is null)
						throw new InvalidOperationException($"Товар с идентификатором {item.Id} в базе данных не найден.");

					var orderItem = new OrderItem
					{
						Order = order,
						Price = product.Price,
						Quantity = item.Quantity,
						Product = product
					};
					_db.OrderItems.Add(orderItem);
				}
				_db.SaveChanges();
				transaction.Commit();

				return order.ToDTO();
			}
		}
	}
}
