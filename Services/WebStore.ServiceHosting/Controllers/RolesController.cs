﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.DAL.Context;

namespace WebStore.ServiceHosting.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class RolesController : ControllerBase
	{
		private readonly RoleStore<IdentityRole> _roleStore;

		public RolesController(WebStoreContext db)
		{
			_roleStore = new RoleStore<IdentityRole>(db) { AutoSaveChanges = true };
		}

		[HttpGet("AllRoles")]
		public async Task<IEnumerable<IdentityRole>> GetAllRoles() => await _roleStore.Roles.ToArrayAsync();

		[HttpPost]
		public async Task<bool> CreateAsync(IdentityRole role) => (await _roleStore.CreateAsync(role)).Succeeded;

		[HttpPut]
		public async Task<bool> UpdateAsync(IdentityRole role) => (await _roleStore.UpdateAsync(role)).Succeeded;

		[HttpPost("Delete")]
		public async Task<bool> DeleteAsync(IdentityRole role) => (await _roleStore.DeleteAsync(role)).Succeeded;

		[HttpPost("GetRoleId")]
		public async Task<string> GetRoleIdAsync(IdentityRole role) => await _roleStore.GetRoleIdAsync(role);

		[HttpPost("GetRoleName")]
		public async Task<string> GetRoleNameAsync(IdentityRole role) => await _roleStore.GetRoleNameAsync(role);

		[HttpPost("SetRoleName/{name}")]
		public async Task SetRoleNameAsync(IdentityRole role, string name) => await _roleStore.SetRoleNameAsync(role, name);

		[HttpPost("GetNormalizedRoleName")]
		public async Task<string> GetNormalizedRoleNameAsync(IdentityRole role) => await _roleStore.GetNormalizedRoleNameAsync(role);

		[HttpPost("SetNormalizedRoleName/{name}")]
		public async Task SetNormalizedRoleNameAsync(IdentityRole role, string name) => await _roleStore.SetNormalizedRoleNameAsync(role, name);

		[HttpGet("FindById/{id}")]
		public async Task<IdentityRole> FindByIdAsync(string id) => await _roleStore.FindByIdAsync(id);

		[HttpGet("FindByName/{name}")]
		public async Task<IdentityRole> FindByNameAsync(string name) => await _roleStore.FindByNameAsync(name);
	}
}
