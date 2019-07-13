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

		public IActionResult Blog() => View();

		public IActionResult BlogSingle() => View();

		public IActionResult NotFound() => View(); // TODO переименовать в Error404

		public IActionResult ErrorStatusCode(string id)
		{
			switch (id)
			{
				case "404":
					return RedirectToAction(nameof(NotFound));
				default:
					return Content($"Произошла ошибка с кодом {id}");
			}
		}
	}
}
