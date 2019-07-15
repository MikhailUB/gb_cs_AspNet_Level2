using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using WebStore.Domain.Models;
using WebStore.Interfaces.Services;

namespace WebStore.Services
{
	public class CookiesCartStore : ICartStore
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly string _cartName;

		public Cart Cart
		{
			get
			{
				var http_context = _httpContextAccessor.HttpContext;
				var cookie = http_context.Request.Cookies[_cartName];

				Cart cart = null;
				if (cookie is null)
				{
					cart = new Cart();
					http_context.Response.Cookies.Append(
						_cartName,
						JsonConvert.SerializeObject(cart));
				}
				else
				{
					cart = JsonConvert.DeserializeObject<Cart>(cookie);
					http_context.Response.Cookies.Delete(_cartName);
					http_context.Response.Cookies.Append(_cartName, cookie, new CookieOptions
					{
						Expires = DateTime.Now.AddDays(1)
					});
				}

				return cart;
			}
			set
			{
				var http_context = _httpContextAccessor.HttpContext;

				var json = JsonConvert.SerializeObject(value);
				http_context.Response.Cookies.Delete(_cartName);
				http_context.Response.Cookies.Append(_cartName, json, new CookieOptions
				{
					Expires = DateTime.Now.AddDays(1)
				});
			}
		}

		public CookiesCartStore(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;

			var user = httpContextAccessor.HttpContext.User;
			var userName = user.Identity.IsAuthenticated ? user.Identity.Name : null;
			_cartName = $"cart{userName}";
		}
	}
}
