using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebStore.Interfaces.Api;

namespace WebStore.Controllers
{
	public class WebAPITestController : Controller
	{
		private readonly IValuesService _valuesService;

		public WebAPITestController(IValuesService valuesService)
		{
			_valuesService = valuesService;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _valuesService.GetAsync());
		}
	}
}
