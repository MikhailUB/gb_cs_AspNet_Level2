using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels.Account;

namespace WebStore.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ILogger<AccountController> _logger;

		public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
		}

		public IActionResult Register() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrerUserViewModel model)
		{
			if (!ModelState.IsValid) // проверка данных формы
				return View(model);

			using (_logger.BeginScope($"Регистрация нового пользователя {model.UserName}"))
			{
				var newUser = new User
				{
					UserName = model.UserName
				};
				// Регистрируем в системе
				var createResult = await _userManager.CreateAsync(newUser, model.Password);
				if (createResult.Succeeded) // Если получилось
				{
					await _signInManager.SignInAsync(newUser, false); // Логиним на сайте

					_logger.LogInformation($"Пользователь {model.UserName} успешно зарегистрирован");
					return RedirectToAction("Index", "Home"); // редирект на главную
				}

				foreach (var error in createResult.Errors) // Если ошибки
					ModelState.AddModelError("", error.Description); // добавляем их к состоянию модели

				_logger.LogWarning($"Ошибка регистрации пользователя {model.UserName}: " +
					string.Join(", ", createResult.Errors.Select(e => e.Description)));
			}

			return View(model); // возвращем модель браузеру
		}

		public IActionResult Login() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			if (!ModelState.IsValid)
				return View(login);

			var loginResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false);
			if (loginResult.Succeeded)
			{
				_logger.LogInformation($"Пользователь {login.UserName} вошёл");

				if (Url.IsLocalUrl(login.ReturnUrl))
					return Redirect(login.ReturnUrl);

				return RedirectToAction("Index", "Home"); // на главную
			}

			ModelState.AddModelError("", "Неверное имя пользователя или пароль!");
			_logger.LogWarning($"Ошибка входа пользователя {login.UserName}");

			return View(login);
		}

		public async Task<IActionResult> Logout()
		{
			var userName = User.Identity.Name;
			await _signInManager.SignOutAsync();
			_logger.LogInformation($"Пользователь {userName} вышел из системы");

			return RedirectToAction("Index", "Home");
		}

		public IActionResult AccessDenied() => View();
	}
}
