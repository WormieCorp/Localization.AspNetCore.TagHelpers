using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization.AspNetCore.TagHelpers.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.TagHelpers
{
	/// <summary>
	///   Adds support to localize the inner text for a tag, when one of the following attributes
	///   have been added: <c>localize</c>, <c>localize-html</c> or <c>localize-type</c>.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
	/// <example>
	///   <code>
	/// <![CDATA[
	/// <span localize="">
	/// To text to localize goes here
	/// </span>
	/// ]]>
	///   </code>
	/// </example>
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_NAME)]
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_TYPE)]
	[HtmlTargetElement(Attributes = ASP_LOCALIZE_HTML)]
	public class GenericLocalizeTagHelper : TagHelper
	{
		private const string ASP_LOCALIZE_HTML = "localize-html";
		private const string ASP_LOCALIZE_NAME = "localize";
		private const string ASP_LOCALIZE_TRIM = "localize-trim";
		private const string ASP_LOCALIZE_TYPE = "localize-type";

		private readonly string _applicationName;
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private IHtmlLocalizer _localizer;

		/// <summary>
		///   Initializes a new instance of the <see cref="GenericLocalizeTagHelper" /> class.
		/// </summary>
		/// <param name="localizerFactory">
		///   The localizer factory to create a <see cref="IHtmlLocalizer" /> from.
		/// </param>
		/// <param name="hostingEnvironment">The hosting environment.</param>
		public GenericLocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
		{
			Throws.NotNull(localizerFactory, nameof(localizerFactory));
			Throws.NotNull(hostingEnvironment, nameof(hostingEnvironment));

			_localizerFactory = localizerFactory;
			this._applicationName = hostingEnvironment.ApplicationName;
		}

		/// <summary>
		///   Gets or sets a value indicating whether the inner text should be treated as HTML (no encoding).
		/// </summary>
		/// <value><c>true</c> if the inner text is HTML; otherwise, <c>false</c>.</value>
		/// <remarks>This defaults to false.</remarks>
		/// <example>
		///   <code>
		/// <![CDATA[
		/// <ul>
		///   <li localize-html="true">
		///     <a href="~/home">Home</a>
		///   </li>
		/// </ul>
		/// ]]>
		///   </code>
		/// </example>
		[HtmlAttributeName(ASP_LOCALIZE_HTML)]
		public virtual bool IsHtml { get; set; }

		/// <summary>
		///   Optionally overrides the name/path of the resource file. If the name is empty it
		///   resolves to the current path and name of the view. i.e the view located at
		///   <c>"~/Views/Home/About.cshtml"</c> passes the following name to the html localizer as <c>Views/Home/About</c>
		/// </summary>
		/// <value>The optional name/path to the resource file.</value>
		/// <example>
		///   <code>
		/// <![CDATA[
		/// <span localize="MyCustomResource">
		///   The text to localize.
		/// </span>
		/// ]]>
		///   </code>
		///   Passes the path as <c>~/MyCustomResource</c>
		/// </example>
		[HtmlAttributeName(ASP_LOCALIZE_NAME)]
		public virtual string Name { get; set; } = "";

		/// <summary>
		///   Gets or sets a value indicating whether beginning and ending whitespace.
		/// </summary>
		/// <value><c>true</c> to trim beginning and ending whitespace; otherwise, <c>false</c>.</value>
		[HtmlAttributeName(ASP_LOCALIZE_TRIM)]
		public virtual bool TrimWhitespace { get; set; } = true;

		/// <summary>
		///   Gets or sets the type to use when looking up the resource file.
		/// </summary>
		/// <value>The type to use when looking up the resource file.</value>
		/// <example>
		///   <code>
		/// <![CDATA[
		/// <span localize-type="typeof(Localization.Demo.Models.SharedType)">
		///   The text
		/// </span>
		/// ]]>
		///   </code>
		///   Creates a new html localizer passing the specified type.
		/// </example>
		[HtmlAttributeName(ASP_LOCALIZE_TYPE)]
		public virtual Type Type { get; set; }

		/// <summary>
		///   Gets or sets the view context (automatically set when using razor views).
		/// </summary>
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		/// <summary>
		///   Gets a value indicating whether this localizer tag helper supports parameters.
		/// </summary>
		/// <value><c>true</c> if this tag helper supports parameters; otherwise, <c>false</c>.</value>
		/// <remarks>
		///   Defaults to <see langword="true" />, but may be overridden in inherited <c>class</c>.
		/// </remarks>
		protected virtual bool SupportsParameters => true;

		/// <summary>
		///   Initializes this Localize Tag Helpers, setting the html localizer and creating a stack
		///   list for child tag helpers to add parameters to.
		/// </summary>
		/// <inheritdoc />
		public override void Init(TagHelperContext context)
		{
			_localizer = _localizerFactory.ResolveLocalizer(ViewContext, _applicationName, Type, Name);

			if (!SupportsParameters)
				return;

			Stack<List<object>> currentStack;

			if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
			{
				currentStack = new Stack<List<object>>();
				context.Items.Add(typeof(GenericLocalizeTagHelper), currentStack);
			}
			else
			{
				currentStack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];
			}

			currentStack.Push(new List<object>());
		}

		/// <summary>
		///   The function responsible for acquiring the text to localize, getting the required
		///   parameters to pass to the localized text and replacing the old text with the new
		///   localized text.
		/// </summary>
		/// <inheritdoc />
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

		/// <summary>
		///   Gets the content/text that are to be localized.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		protected virtual async Task<string> GetContentAsync(TagHelperContext context, TagHelperOutput output)
		{
			var content = await output.GetChildContentAsync(true);
			if (output.IsContentModified)
				return output.Content.GetContent(NullHtmlEncoder.Default);
			return content.GetContent(NullHtmlEncoder.Default);
		}

		/// <summary>
		///   Gets the parameters to use when localizing the text.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		protected virtual IEnumerable<object> GetParameters(TagHelperContext context)
		{
			if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
			{
				return new object[0];
			}

			var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];

			return stack.Pop();
		}

		/// <summary>
		///   Sets the localized content back to where the original content was.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="outputContent">Content of the output tag helper.</param>
		/// <param name="content">The content to set.</param>
		protected virtual void SetContent(TagHelperContext context, TagHelperContent outputContent, string content)
		{
			outputContent.SetContent(content);
		}

		/// <summary>
		///   Sets the localized content without encoding it back to where the original content was.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="outputContent">Content of the output tag helper.</param>
		/// <param name="htmlContent">The content to set.</param>
		protected virtual void SetHtmlContent(TagHelperContext context, TagHelperContent outputContent, IHtmlContent htmlContent)
		{
			outputContent.SetHtmlContent(htmlContent);
		}
	}
}
