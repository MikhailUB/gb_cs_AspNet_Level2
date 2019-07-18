using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebStore.TagHelpers
{
	[HtmlTargetElement(Attributes = AttributeName)]
	public class ActiveRouteTagHelper : TagHelper
	{
		public const string AttributeName = "is-active-route";
		public const string IgnoreActionName = "ignore-action";

		[HtmlAttributeName("asp-action")]
		public string Action { get; set; }

		[HtmlAttributeName("asp-controller")]
		public string Controller { get; set; }

		private IDictionary<string, string> _routeValues;

		[HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
		public IDictionary<string, string> RouteValues
		{
			get => _routeValues ?? (_routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
			set => _routeValues = value;
		}

		[HtmlAttributeNotBound, ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			base.Process(context, output);

			var ignoreAction = context.AllAttributes.TryGetAttribute(IgnoreActionName, out _);

			if (IsActive(ignoreAction))
				MakeActive(output);

			output.Attributes.RemoveAll(AttributeName);
		}

		private bool IsActive(bool ignoreAction)
		{
			var routeValues = ViewContext.RouteData.Values;
			var currentController = routeValues["Controller"].ToString();
			var currentAction = routeValues["Action"].ToString();

			const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

			if (!string.IsNullOrWhiteSpace(Controller) && !string.Equals(Controller, currentController, ignoreCase))
				return false;

			if (!ignoreAction && !string.IsNullOrWhiteSpace(Action) && !string.Equals(Action, currentAction, ignoreCase))
				return false;

			foreach (var (key, value) in RouteValues)
			{
				if (!routeValues.ContainsKey(key) || routeValues[key].ToString() != value)
					return false;
			}
			return true;
		}

		private static void MakeActive(TagHelperOutput output)
		{
			var classAttribute = output.Attributes.FirstOrDefault(a => a.Name == "class");

			if (classAttribute is null)
			{
				classAttribute = new TagHelperAttribute("class", "active");
				output.Attributes.Add(classAttribute);
			}
			else if (classAttribute.Value?.ToString().Contains("active", StringComparison.Ordinal) != null)
			{
				output.Attributes.SetAttribute("class", classAttribute.Value is null ? "class" : classAttribute.Value + " class");
			}
		}
	}
}
