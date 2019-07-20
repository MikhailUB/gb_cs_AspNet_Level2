using System.Linq;
using WebStore.Domain.Entities;
using WebStore.Domain.Models;
using WebStore.Domain.ViewModels.Cart;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;

namespace WebStore.Services
{
	public class CartService : ICartService
	{
		private readonly IProductData _productData;
		private ICartStore _cartStore;

		public CartService(IProductData productData, ICartStore cartStore)
		{
			_productData = productData;
			_cartStore = cartStore;
		}

		public void DecrementFromCart(int id)
		{
			var cart = _cartStore.Cart;

			var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
			if (item != null)
			{
				if (item.Quantity > 0)
					item.Quantity--;
				if (item.Quantity == 0)
					cart.Items.Remove(item);

				_cartStore.Cart = cart;
			}
		}

		public void RemoveFromCart(int id)
		{
			var cart = _cartStore.Cart;

			var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
			if (item != null)
			{
				cart.Items.Remove(item);
				_cartStore.Cart = cart;
			}
		}

		public void RemoveAll()
		{
			var cart = _cartStore.Cart;
			cart.Items.Clear();
			_cartStore.Cart = cart;
		}

		public void AddToCart(int id)
		{
			var cart = _cartStore.Cart;
			var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

			if (item != null)
				item.Quantity++;
			else
				cart.Items.Add(new CartItem { ProductId = id, Quantity = 1 });

			_cartStore.Cart = cart;
		}

		public CartViewModel TransformCart()
		{
			var products = _productData.GetProducts(new ProductFilter
			{
				Ids = _cartStore.Cart.Items.Select(item => item.ProductId).ToList()
			});

			var productsViewModels = products.Products.Select(p => new ProductViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Order = p.Order,
				Price = p.Price,
				ImageUrl = p.ImageUrl,
				Brand = p.Brand?.Name
			});

			return new CartViewModel
			{
				Items = _cartStore.Cart.Items.ToDictionary(item => productsViewModels.First(p => p.Id == item.ProductId), item => item.Quantity)
			};
		}
	}
}
