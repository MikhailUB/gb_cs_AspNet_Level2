using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Identity;
using WebStore.Domain.Entities;

namespace WebStore.ServiceHosting.Controllers
{
	/// <summary>Контроллер управления пользователями</summary>
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class UsersController : ControllerBase
	{
		private readonly ILogger<UsersController> _logger;
		private readonly UserStore<User> _userStore;

		public UsersController(WebStoreContext db, ILogger<UsersController> logger)
		{
			_logger = logger;
			_userStore = new UserStore<User>(db)
			{
				AutoSaveChanges = true
			};
		}

		#region Users

		/// <summary>Получить всех пользователей в системе</summary>
		/// <returns>Возвращает список всех пользователей из базы данных</returns>
		[HttpGet("AllUsers")]
		public async Task<IEnumerable<User>> GetAllUsers() => await _userStore.Users.ToArrayAsync();

		[HttpPost("UserId")]
		public async Task<string> GetUserIdAsync([FromBody] User user) => await _userStore.GetUserIdAsync(user);

		[HttpPost("UserName")]
		public async Task<string> GetUserNameAsync([FromBody] User user) => await _userStore.GetUserNameAsync(user);

		[HttpPost("UserName/{name}")]
		public async Task SetUserNameAsync([FromBody] User user, string name) => await _userStore.SetUserNameAsync(user, name);

		[HttpPost("NormalUserName")]
		public async Task<string> GetNormalizedUserNameAsync([FromBody] User user) => await _userStore.GetNormalizedUserNameAsync(user);

		[HttpPost("NormalUserName/{name}")]
		public Task SetNormalizedUserNameAsync([FromBody] User user, string name) => _userStore.SetNormalizedUserNameAsync(user, name);

		[HttpPost("User")]
		public async Task<bool> CreateAsync([FromBody] User user)
		{
			var result = await _userStore.CreateAsync(user);
			if (result.Succeeded)
				_logger.LogInformation("Пользователь {0} успешно создан", user.UserName);
			else
				_logger.LogWarning("При создании нового пользователя {0} возникли ошибки: {1}",
					user.UserName, string.Join(",", result.Errors.Select(error => error.Description)));
			return result.Succeeded;
		}

		[HttpPut("User")]
		public async Task<bool> UpdateAsync([FromBody] User user) => (await _userStore.UpdateAsync(user)).Succeeded;

		[HttpPost("User/Delete")]
		public async Task<bool> DeleteAsync([FromBody] User user) => (await _userStore.DeleteAsync(user)).Succeeded;

		[HttpGet("User/Find/{id}")]
		public async Task<User> FindByIdAsync(string id) => await _userStore.FindByIdAsync(id);

		[HttpGet("User/Normal/{name}")]
		public async Task<User> FindByNameAsync(string name) => await _userStore.FindByNameAsync(name);

		#endregion

		#region Roles

		[HttpPost("Role/{role}")]
		public async Task AddToRoleAsync([FromBody] User user, string role) => await _userStore.AddToRoleAsync(user, role);

		[HttpPost("Role/Delete/{role}")]
		public async Task RemoveFromRoleAsync([FromBody] User user, string role) => await _userStore.RemoveFromRoleAsync(user, role);

		[HttpPost("Roles")]
		public async Task<IList<string>> GetRolesAsync([FromBody] User user) => await _userStore.GetRolesAsync(user);

		[HttpPost("InRole/{role}")]
		public async Task<bool> IsInRoleAsync([FromBody] User user, string role) => await _userStore.IsInRoleAsync(user, role);

		[HttpGet("UsersInRole/{role}")]
		public async Task<IList<User>> GetUsersInRoleAsync(string role) => await _userStore.GetUsersInRoleAsync(role);

		#endregion

		#region Password management

		[HttpPost("GetPasswordHash")]
		public async Task<string> GetPasswordHashAsync([FromBody] User user) => await _userStore.GetPasswordHashAsync(user);

		[HttpPost("SetPasswordHash")]
		public async Task<string> SetPasswordHashAsync([FromBody] PasswordHashDTO hash)
		{
			await _userStore.SetPasswordHashAsync(hash.User, hash.Hash);
			return hash.User.PasswordHash;
		}

		[HttpPost("HasPassword")]
		public async Task<bool> HasPasswordAsync([FromBody] User user) => await _userStore.HasPasswordAsync(user);

		#endregion

		#region Claims management

		[HttpPost("GetClaims")]
		public async Task<IList<Claim>> GetClaimsAsync([FromBody] User user) => await _userStore.GetClaimsAsync(user);

		[HttpPost("AddClaims")]
		public async Task AddClaimsAsync([FromBody] AddClaimDTO claimInfo) => await _userStore.AddClaimsAsync(claimInfo.User, claimInfo.Claims);

		[HttpPost("ReplaceClaim")]
		public async Task ReplaceClaimAsync([FromBody] ReplaceClaimDTO claimInfo) =>
			await _userStore.ReplaceClaimAsync(claimInfo.User, claimInfo.OldClaim, claimInfo.NewClaim);

		[HttpPost("RemoveClaim")]
		public async Task RemoveClaimsAsync([FromBody] RemoveClaimDTO claimInfo) =>
			await _userStore.RemoveClaimsAsync(claimInfo.User, claimInfo.Claims);

		[HttpPost("GetUsersForClaim")]
		public async Task<IList<User>> GetUsersForClaimAsync([FromBody] Claim claim) => await _userStore.GetUsersForClaimAsync(claim);

		#endregion

		#region Двухфакторная авторизация

		[HttpPost("GetTwoFactorEnabled")]
		public async Task<bool> GetTwoFactorEnabledAsync([FromBody] User user) => await _userStore.GetTwoFactorEnabledAsync(user);

		[HttpPost("SetTwoFactor/{enable}")]
		public async Task SetTwoFactorEnabledAsync([FromBody] User user, bool enable) => await _userStore.SetTwoFactorEnabledAsync(user, enable);

		#endregion

		#region Emails management

		[HttpPost("GetEmail")]
		public async Task<string> GetEmailAsync([FromBody] User user) => await _userStore.GetEmailAsync(user);

		[HttpPost("SetEmail/{email}")]
		public async Task SetEmailAsync([FromBody] User user, string email) => await _userStore.SetEmailAsync(user, email);

		[HttpPost("GetEmailConfirmed")]
		public async Task<bool> GetEmailConfirmedAsync([FromBody] User user) => await _userStore.GetEmailConfirmedAsync(user);

		[HttpPost("SetEmailConfirmed/{enable}")]
		public async Task SetEmailConfirmedAsync([FromBody] User user, bool enable) => await _userStore.SetEmailConfirmedAsync(user, enable);

		[HttpGet("UserFindByEmail/{email}")]
		public async Task<User> FindByEmailAsync(string email) => await _userStore.FindByEmailAsync(email);

		[HttpPost("GetNormalizedEmail")]
		public async Task<string> GetNormalizedEmailAsync([FromBody] User user) => await _userStore.GetNormalizedEmailAsync(user);

		[HttpPost("SetNormalizedEmail/{email?}")] // если не добавить "?", то при создании пользователя без e-mail невозможно будет выполнить запрос к этому WebAPI
		public async Task SetNormalizedEmailAsync([FromBody] User user, string email) => await _userStore.SetNormalizedEmailAsync(user, email);

		#endregion

		#region Phone numbers management

		[HttpPost("GetPhoneNumber")]
		public async Task<string> GetPhoneNumberAsync([FromBody] User user) => await _userStore.GetPhoneNumberAsync(user);

		[HttpPost("SetPhoneNumber/{phone}")]
		public async Task SetPhoneNumberAsync([FromBody] User user, string phone) => await _userStore.SetPhoneNumberAsync(user, phone);

		[HttpPost("GetPhoneNumberConfirmed")]
		public async Task<bool> GetPhoneNumberConfirmedAsync([FromBody] User user) => await _userStore.GetPhoneNumberConfirmedAsync(user);

		[HttpPost("SetPhoneNumberConfirmed/{confirmed}")]
		public async Task SetPhoneNumberConfirmedAsync([FromBody] User user, bool confirmed) =>
			await _userStore.SetPhoneNumberConfirmedAsync(user, confirmed);

		#endregion

		#region Login info management

		[HttpPost("AddLogin")]
		public async Task AddLoginAsync([FromBody] AddLoginDTO login)
		{
			_logger.LogInformation("Пользователь {0} вошёл в систему", login.User);
			await _userStore.AddLoginAsync(login.User, login.UserLoginInfo);
		}

		[HttpPost("RemoveLogin/{LoginProvider}/{ProviderKey}")]
		public async Task RemoveLoginAsync([FromBody] User user, string loginProvider, string providerKey) =>
			await _userStore.RemoveLoginAsync(user, loginProvider, providerKey);

		[HttpPost("GetLogins")]
		public async Task<IList<UserLoginInfo>> GetLoginsAsync([FromBody] User user) => await _userStore.GetLoginsAsync(user);

		[HttpGet("User/FindByLogin/{LoginProvider}/{ProviderKey}")]
		public async Task<User> FindByLoginAsync(string loginProvider, string providerKey) =>
			await _userStore.FindByLoginAsync(loginProvider, providerKey);

		#endregion

		#region Lockout management

		[HttpPost("GetLockoutEndDate")]
		public async Task<DateTimeOffset?> GetLockoutEndDateAsync([FromBody] User user) => await _userStore.GetLockoutEndDateAsync(user);

		[HttpPost("SetLockoutEndDate")]
		public async Task SetLockoutEndDateAsync([FromBody] SetLockoutDTO locoutInfo) =>
			await _userStore.SetLockoutEndDateAsync(locoutInfo.User, locoutInfo.LockoutEnd);

		[HttpPost("IncrementAccessFailedCount")]
		public async Task<int> IncrementAccessFailedCountAsync([FromBody] User user) => await _userStore.IncrementAccessFailedCountAsync(user);

		[HttpPost("ResetAccessFailedCount")]
		public async Task ResetAccessFailedCountAsync([FromBody] User user) => await _userStore.ResetAccessFailedCountAsync(user);

		[HttpPost("GetAccessFailedCount")]
		public async Task<int> GetAccessFailedCountAsync([FromBody] User user) => await _userStore.GetAccessFailedCountAsync(user);

		[HttpPost("GetLockoutEnabled")]
		public async Task<bool> GetLockoutEnabledAsync([FromBody] User user) => await _userStore.GetLockoutEnabledAsync(user);

		[HttpPost("SetLockoutEnabled/{enable}")]
		public async Task SetLockoutEnabledAsync([FromBody] User user, bool enable) => await _userStore.SetLockoutEnabledAsync(user, enable);

		#endregion
	}
}
