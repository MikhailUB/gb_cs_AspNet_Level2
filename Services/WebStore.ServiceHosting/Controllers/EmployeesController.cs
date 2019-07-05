using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebStore.Domain.Models;
using WebStore.Interfaces.Services;

namespace WebStore.ServiceHosting.Controllers
{
	[Route("api/[controller]")]
	//[Route("api/employees")]
	[ApiController]
	[Produces("application/json")]
	public class EmployeesController : ControllerBase, IEmployeesData
	{
		private readonly IEmployeesData _employeesData;

		public EmployeesController(IEmployeesData employeesData) => _employeesData = employeesData;

		[HttpGet, ActionName("Get")]
		public IEnumerable<Employee> GetAll()
		{
			return _employeesData.GetAll();
		}

		[HttpGet("{id}"), ActionName("Get")]
		public Employee GetById(int id)
		{
			return _employeesData.GetById(id);
		}

		[HttpPost, ActionName("Post")]
		public void AddNew([FromBody] Employee employee)
		{
			_employeesData.AddNew(employee);
		}

		[HttpPut("{id}"), ActionName("Put")]
		public Employee Update(int id, [FromBody] Employee employee)
		{
			return _employeesData.Update(id, employee);
		}

		[HttpDelete("{id}")]
		public void Delete(int id/*, [FromServices] ILogger<EmployeesController> logger*/)
		{
			//using(logger.BeginScope($"Удаление сотрудника с id {id}"))
			{
				_employeesData.Delete(id);
				//logger.LogInformation("Сотрудник id {0} удалён усепшно", id);
			}
		}

		[NonAction]
		public void SaveChanges()
		{
			_employeesData.SaveChanges();
		}
	}
}
