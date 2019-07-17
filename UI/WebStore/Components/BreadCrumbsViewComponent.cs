using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using WebStore.Domain.ViewModels.BreadCrumbs;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
	public class BreadCrumbsViewComponent : ViewComponent
	{
		private readonly IProductData _productData;

		public BreadCrumbsViewComponent(IProductData productData) => _productData = productData;

		public IViewComponentResult Invoke(BreadCrumbsType type, int id, BreadCrumbsType fromType)
		{
			if (!Enum.IsDefined(typeof(BreadCrumbsType), type))
				throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(BreadCrumbsType));

			if (!Enum.IsDefined(typeof(BreadCrumbsType), fromType))
				throw new InvalidEnumArgumentException(nameof(fromType), (int)fromType, typeof(BreadCrumbsType));

			switch (type)
			{
				case BreadCrumbsType.None:
					break;
				case BreadCrumbsType.Section:
					var section = _productData.GetSectionById(id);
					return View(new[]
					{
						new BreadCrumbsViewModel
						{
							BreadCrumbsType = type,
							Id = id.ToString(),
							Name = section.Name
						}
					});
				case BreadCrumbsType.Brand:
					var brand = _productData.GetBrandById(id);
					return View(new[]
					{
						new BreadCrumbsViewModel
						{
							BreadCrumbsType = type,
							Id = id.ToString(),
							Name = brand.Name
						}
					});
				case BreadCrumbsType.Item:
					return View(GetItemBreadCrumbs(type, id, fromType));
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
			return View(new BreadCrumbsViewModel[0]);
		}

		public IEnumerable<BreadCrumbsViewModel> GetItemBreadCrumbs(BreadCrumbsType type, int id, BreadCrumbsType fromType)
		{
			var item = _productData.GetProductById(id);

			var crumbs = new List<BreadCrumbsViewModel>();

			if (fromType == BreadCrumbsType.Section)
			{
				crumbs.Add(new BreadCrumbsViewModel
				{
					BreadCrumbsType = fromType,
					Id = id.ToString(),
					Name = item.Section.Name
				});
			}
			else
			{
				crumbs.Add(new BreadCrumbsViewModel
				{
					BreadCrumbsType = fromType,
					Id = id.ToString(),
					Name = item.Brand.Name
				});
			}
			crumbs.Add(new BreadCrumbsViewModel
			{
				BreadCrumbsType = type,
				Id = item.Id.ToString(),
				Name = item.Name
			});
			return crumbs;
		}
	}
}
