using Microsoft.AspNetCore.Mvc;
using System;

namespace WebStore.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			//throw new ApplicationException("Тестовое исключение");
			return View();
		}

		public IActionResult ContactUs() => View();

		public IActionResult Checkout() => View();

		public IActionResult Blog() => View();

		public IActionResult BlogSingle() => View();

		public IActionResult NotFound() => View();
	}
}
