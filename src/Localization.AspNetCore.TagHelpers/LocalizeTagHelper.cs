using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into
	// your project
	[HtmlTargetElement(LOCALIZE_TAG_NAME)]
	public class LocalizeTagHelper : AspLocalizeTagHelper
	{
		private const string LOCALIZE_TAG_NAME = "localize";

		public LocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
			: base(localizerFactory, hostingEnvironment)
		{
		}

		[HtmlAttributeName("html")]
		public override bool IsHtml
		{
			get
			{
				return base.IsHtml;
			}

			set
			{
				base.IsHtml = value;
			}
		}

		[HtmlAttributeName("name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}

			set
			{
				base.Name = value;
			}
		}

		[HtmlAttributeName("trim")]
		public override bool TrimWhitespace
		{
			get
			{
				return base.TrimWhitespace;
			}

			set
			{
				base.TrimWhitespace = value;
			}
		}

		[HtmlAttributeName("type")]
		public override Type Type
		{
			get
			{
				return base.Type;
			}

			set
			{
				base.Type = value;
			}
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await base.ProcessAsync(context, output);

			output.TagName = null;
		}
	}
}
