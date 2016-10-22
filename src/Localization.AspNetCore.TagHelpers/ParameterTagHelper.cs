using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Localization.AspNetCore.TagHelpers
{
	/// <summary>
	///   Adds parameters to <see cref="GenericLocalizeTagHelper" /> and
	///   <see cref="LocalizeTagHelper" />. Used through a <c>parameter</c> html tag.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
	[HtmlTargetElement("parameter", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class ParameterTagHelper : TagHelper
	{
		/// <inheritdoc />
		public override int Order
		{
			get
			{
				return 2;
			}
		}

		/// <summary>
		///   This method adds parameters to the parent tag helper if they are a
		///   <see cref="GenericLocalizeTagHelper" /> or a <see cref="LocalizeTagHelper" />.
		/// </summary>
		/// <inheritdoc />
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
			if (output.IsContentModified)
				content = output.Content;

			if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
			{
				output.Content = content;
				return;
			}

			var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];

			var existingItems = stack.Peek();
			existingItems.Add(content.GetContent(NullHtmlEncoder.Default));

			output.SuppressOutput();
			output.TagName = null;
		}
	}
}
