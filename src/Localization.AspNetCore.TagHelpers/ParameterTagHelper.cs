using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into
	// your project
	[HtmlTargetElement("parameter", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class ParameterTagHelper : TagHelper
	{
		public override int Order
		{
			get
			{
				return 2;
			}
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
			if (output.IsContentModified)
				content = output.Content;

			if (!context.Items.ContainsKey(typeof(LocalizeTagHelper)))
			{
				output.Content = content;
				return;
			}

			var stack = (Stack<List<string>>)context.Items[typeof(LocalizeTagHelper)];

			var existingItems = stack.Peek();
			existingItems.Add(content.GetContent(NullHtmlEncoder.Default));

			output.Content.Clear();
			output.TagName = null;
		}
	}
}
