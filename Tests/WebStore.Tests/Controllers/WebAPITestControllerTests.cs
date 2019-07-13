using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Controllers;
using WebStore.Interfaces.Api;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
	[TestClass]
	public class WebAPITestControllerTests
	{
		private WebAPITestController _controller;
		private string[] _resultArray = new[] { "123", "456", "789" };

		[TestInitialize]
		public void Initialize()
		{
			var valueServiceMock = new Mock<IValuesService>();

			valueServiceMock
				.Setup(svc => svc.GetAsync())
				.ReturnsAsync(_resultArray);

			_controller = new WebAPITestController(valueServiceMock.Object);
		}

		[TestMethod]
		public async Task Index_Method_Returns_View_With_Values()
		{
			var result = await _controller.Index();

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);

			Assert.Equal(_resultArray.Length, model.Count());
		}
	}
}
