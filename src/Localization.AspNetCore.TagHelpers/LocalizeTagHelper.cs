using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into
	// your project
	[HtmlTargetElement(LOCALIZE_TAG_NAME)]
	[HtmlTargetElement(Attributes = LOCALIZE_ATTRIBUTE_NAME)]
	public class LocalizeTagHelper : TagHelper
	{
		private const string LOCALIZE_ATTRIBUTE_NAME = "asp-localize";
		private const string LOCALIZE_TAG_NAME = "localize";
		private readonly string _applicationName;
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private IHtmlLocalizer _localizer;

		public LocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
		{
			if (localizerFactory == null)
				throw new ArgumentNullException(nameof(localizerFactory));

			if (hostingEnvironment == null)
				throw new ArgumentNullException(nameof(hostingEnvironment));

			_localizerFactory = localizerFactory;

			this._applicationName = hostingEnvironment.ApplicationName;
		}

		[HtmlAttributeName("html")]
		public bool IsHtml { get; set; } = false;

		public override int Order
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		///   Gets or sets a flag indicating whether to trim whitespace before
		///   and after the text to be localized.
		/// </summary>
		/// <value>
		///   <c>true</c> to trim whitespace; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		///   Defaults to <see langword="true"/>
		/// </remarks>
		[HtmlAttributeName("trim")]
		public bool Trim { get; set; } = true;

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Init(TagHelperContext context)
		{
			var path = ViewContext.ExecutingFilePath;

			if (string.IsNullOrEmpty(path))
			{
				path = ViewContext.View.Path;
			}

			Debug.Assert(!string.IsNullOrEmpty(path), "Couldn't determine a path for the view");

			var baseName = BuildBaseName(path, _applicationName);

			_localizer = _localizerFactory.Create(baseName, _applicationName);

			var parameters = new List<string>();
			Stack<List<string>> stack;
			if (!context.Items.ContainsKey(this.GetType()))
			{
				stack = new Stack<List<string>>();

				context.Items.Add(this.GetType(), stack);
			}
			else
			{
				stack = (Stack<List<string>>)context.Items[this.GetType()];
			}

			stack.Push(parameters);

			base.Init(context);
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var stack = (Stack<List<string>>)context.Items[this.GetType()];

			var content = await output.GetChildContentAsync();
			if (output.IsContentModified)
				content = output.Content;

			var parameters = stack.Pop();

			var stringContent = content.GetContent(NullHtmlEncoder.Default);
			if (Trim)
				stringContent = stringContent.Trim();
			if (!string.IsNullOrEmpty(stringContent))
			{
				if (!IsHtml)
				{
					LocalizedString newContent;
					if (parameters.Any())
					{
						//var parameters = (IList<string>)context.Items[typeof(ParamTagHelper)];

						newContent = _localizer.GetString(stringContent, parameters.ToArray());
					}
					else
					{
						newContent = _localizer.GetString(stringContent);
					}
					content.SetContent(newContent);
				}
				else
				{
					LocalizedHtmlString newContent;
					if (parameters.Any())
					{
						//var parameters = (IList<string>)context.Items[typeof(ParamTagHelper)];

						newContent = _localizer[stringContent, parameters.ToArray()];
					}
					else
					{
						newContent = _localizer[stringContent];
					}

					content.SetHtmlContent(newContent);
				}
			}
			output.Content = content;

			if (!context.AllAttributes.ContainsName(LOCALIZE_ATTRIBUTE_NAME))
			{
				output.TagName = null;
			}
			if (output.Attributes.ContainsName(LOCALIZE_ATTRIBUTE_NAME))
			{
				var index = output.Attributes.IndexOfName(LOCALIZE_ATTRIBUTE_NAME);
				output.Attributes.RemoveAt(index);
			}
		}

		private static string BuildBaseName(string path, string applicationName)
		{
			var extension = Path.GetExtension(path);
			var startIndex = path[0] == '/' || path[0] == '\\' ? 1 : 0;
			var length = path.Length - startIndex - extension.Length;
			var capacity = length + applicationName.Length + 1;
			var builder = new StringBuilder(path, startIndex, length, capacity);

			builder.Replace('/', '.').Replace('\\', '.');

			builder.Insert(0, '.');
			builder.Insert(0, applicationName);

			return builder.ToString();
		}
	}
}
