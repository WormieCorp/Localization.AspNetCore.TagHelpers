using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into
	// your project
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_NAME)]
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_TYPE)]
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_HTML)]
	public class AspLocalizeTagHelper : TagHelper
	{
		private const string ASP_LOCALIZE_HTML = "localize-html";
		private const string ASP_LOCALIZE_NAME = "localize";
		private const string ASP_LOCALIZE_TRIM = "localize-trim";
		private const string ASP_LOCALIZE_TYPE = "localize-type";

		private readonly string _applicationName;
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private IHtmlLocalizer _localizer;

		public AspLocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
		{
			if (localizerFactory == null)
				throw new ArgumentNullException(nameof(localizerFactory));
			if (hostingEnvironment == null)
				throw new ArgumentNullException(nameof(hostingEnvironment));

			_localizerFactory = localizerFactory;
			this._applicationName = hostingEnvironment.ApplicationName;
		}

		[HtmlAttributeName(ASP_LOCALIZE_HTML)]
		public virtual bool IsHtml { get; set; }

		[HtmlAttributeName(ASP_LOCALIZE_NAME)]
		public virtual string Name { get; set; } = "";

		[HtmlAttributeName(ASP_LOCALIZE_TRIM)]
		public virtual bool TrimWhitespace { get; set; } = true;

		[HtmlAttributeName(ASP_LOCALIZE_TYPE)]
		public virtual Type Type { get; set; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		protected virtual bool SupportsParameters => true;

		public override void Init(TagHelperContext context)
		{
			if (Type != null)
			{
				_localizer = _localizerFactory.Create(Type);
			}
			else
			{
				string name = Name;
				if (string.IsNullOrEmpty(name))
				{
					var path = ViewContext.ExecutingFilePath;
					if (string.IsNullOrEmpty(path))
						path = ViewContext.View.Path;

					Debug.Assert(!string.IsNullOrEmpty(path), "Couldn't determine a path for the view");

					name = BuildBaseName(path, _applicationName);
				}

				_localizer = _localizerFactory.Create(name, _applicationName);
			}

			if (!SupportsParameters)
				return;

			Stack<List<object>> currentStack;

			if (!context.Items.ContainsKey(typeof(AspLocalizeTagHelper)))
			{
				currentStack = new Stack<List<object>>();
				context.Items.Add(typeof(AspLocalizeTagHelper), currentStack);
			}
			else
			{
				currentStack = (Stack<List<object>>)context.Items[typeof(AspLocalizeTagHelper)];
			}

			currentStack.Push(new List<object>());
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await GetContentAsync(context, output);
			if (TrimWhitespace)
				content = content.Trim();
			var parameters = GetParameters(context);
			if (IsHtml)
			{
				LocalizedHtmlString locString;
				if (parameters.Any())
					locString = _localizer[content, parameters.ToArray()];
				else
					locString = _localizer[content];
				SetHtmlContent(context, output.Content, locString);
			}
			else
			{
				LocalizedString locString;
				if (parameters.Any())
					locString = _localizer.GetString(content, parameters.ToArray());
				else
					locString = _localizer.GetString(content);
				SetContent(context, output.Content, locString);
			}
		}

		protected virtual async Task<string> GetContentAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await output.GetChildContentAsync(true);
			if (output.IsContentModified)
				return output.Content.GetContent(NullHtmlEncoder.Default);
			return content.GetContent(NullHtmlEncoder.Default);
		}

		protected virtual void SetContent(TagHelperContext context, TagHelperContent outputContent, string content)
		{
			outputContent.SetContent(content);
		}

		protected virtual void SetHtmlContent(TagHelperContext context, TagHelperContent outputContent, IHtmlContent htmlContent)
		{
			outputContent.SetHtmlContent(htmlContent);
		}

		protected virtual IEnumerable<object> GetParameters(TagHelperContext context)
		{
			if (!context.Items.ContainsKey(typeof(AspLocalizeTagHelper)))
			{
				return new object[0];
			}

			var stack = (Stack<List<object>>)context.Items[typeof(AspLocalizeTagHelper)];

			return stack.Pop();
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
