using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebStore.Domain.ViewModels.Product;
using WebStore.Interfaces.Services;
using WebStore.Services.Map;

namespace WebStore.Components
{
	public class SectionsViewComponent : ViewComponent
	{
		private readonly IProductData _productData;

		public SectionsViewComponent(IProductData productData)
		{
			_productData = productData;
		}

		public IViewComponentResult Invoke(string sectionId)
		{
			var sectId = int.TryParse(sectionId, out int id) ? id : (int?)null;
			var sections = GetSections(sectId, out int? parentSectionId);

			return View(new SectionCompleteViewModel
			{
				Sections = sections,
				CurrentSectionId = sectId,
				CurrentParentSectionId = parentSectionId
			});
		}

		//public async Task<IViewComponentResult> InvokeAsync() { }

		private IEnumerable<SectionViewModel> GetSections(int? sectionId, out int? parentSectionId)
		{
			parentSectionId = null;
			var sections = _productData.GetSections();

			var parentSections = sections
				.Where(s => s.ParentId == null)
				.Select(s => s.CreateViewModel())
				.ToList();

			foreach (var parentSection in parentSections)
			{
				var childs = sections
					.Where(s => s.ParentId == parentSection.Id)
					.Select(s => s.CreateViewModel());

				foreach (var childSection in childs)
				{
					if (childSection.Id == sectionId)
						parentSectionId = parentSection.Id;

					parentSection.ChildSections.Add(childSection);
				}
				parentSection.ChildSections.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));
			}
			parentSections.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));

			return parentSections;
		}
	}
}
