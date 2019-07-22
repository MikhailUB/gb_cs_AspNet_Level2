using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using WebStore.Domain.ViewModels.Product;

namespace WebStore.TagHelpers
{
	public class PagingTagHelper : TagHelper
	{
		private readonly IUrlHelperFactory _urlHelperFactory;

		[ViewContext, HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; }

		public PageViewModel PageModel { get; set; }

		public string PageAction { get; set; }

		[HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
		public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

		public PagingTagHelper(IUrlHelperFactory urlHelperFactory) => _urlHelperFactory = urlHelperFactory;

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var url_helper = _urlHelperFactory.GetUrlHelper(ViewContext);

			var ul = new TagBuilder("ul");
			ul.AddCssClass("pagination");

			for (var i = 1; i < PageModel.TotalPages; i++)
				ul.InnerHtml.AppendHtml(CreateItem(i, url_helper));

			base.Process(context, output);
			output.Content.AppendHtml(ul);
		}

		private TagBuilder CreateItem(int pageNumber, IUrlHelper urlHelper)
		{
			var li = new TagBuilder("li");
			var a = new TagBuilder("a");

			if (pageNumber == PageModel.PageNumber)
				li.AddCssClass("active");
			else
			{
				PageUrlValues["page"] = pageNumber;
				a.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
			}

			a.InnerHtml.AppendHtml(pageNumber.ToString());
			li.InnerHtml.AppendHtml(a);
			return li;
		}
	}
}
