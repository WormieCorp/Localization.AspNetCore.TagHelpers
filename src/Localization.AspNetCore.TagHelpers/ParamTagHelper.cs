using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into your project
	[HtmlTargetElement("param", ParentTag = "localize")]
	public class ParamTagHelper : TagHelper
	{
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);

			if (context.Items.ContainsKey("LocalizeParameters"))
			{
				var existingItems = (IList<string>)context.Items["LocalizeParameters"];
				existingItems.Add(content.GetContent());
			}
			else
			{
				context.Items.Add("LocalizeParameters", new List<string> { content.GetContent() });
			}

			output.Content.Clear();

			output.TagName = null;
		}
	}
}
