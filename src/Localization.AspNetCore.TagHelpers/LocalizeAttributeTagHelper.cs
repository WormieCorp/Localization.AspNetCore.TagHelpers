using System;
using System.Collections.Generic;
using Localization.AspNetCore.TagHelpers.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Localization.AspNetCore.TagHelpers
{
	/// <summary>
	///   Adds support for localizing html attributes, when using a <c>localize-[attribute]</c>.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
	/// <example>
	///   <code>
	/// <![CDATA[
	/// <abbr localize-title="This will be localized">This will not</abbr>
	/// ]]>
	///   </code>
	/// </example>
	[HtmlTargetElement(Attributes = LOCALIZE_ATTRIBUTE_PREFIX + "*")]
	public class LocalizeAttributeTagHelper : TagHelper
	{
		private const string LOCALIZE_ATTRIBUTE_PREFIX = "localize-";
		private const string LOCALIZE_DICTIONARY_NAME = "localize-all";
		private readonly string _applicationName;
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private IDictionary<string, string> _attributeValues;
		private IHtmlLocalizer _localizer;

		/// <summary>
		///   Initializes a new instance of the <see cref="LocalizeAttributeTagHelper" /> class.
		/// </summary>
		/// <param name="localizerFactory">The localizer factory.</param>
		/// <param name="hostingEnvironment">The hosting environment.</param>
		public LocalizeAttributeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
		{
			Throws.NotNull(localizerFactory, nameof(localizerFactory));
			Throws.NotNull(hostingEnvironment, nameof(hostingEnvironment));

			this._localizerFactory = localizerFactory;
			this._applicationName = hostingEnvironment.ApplicationName;
		}

		/// <summary>
		///   Gets or sets all the attribute values that are to be localized.
		/// </summary>
		[HtmlAttributeName(LOCALIZE_DICTIONARY_NAME, DictionaryAttributePrefix = LOCALIZE_ATTRIBUTE_PREFIX)]
		public IDictionary<string, string> AttributeValues
		{
			get
			{
				if (_attributeValues == null)
				{
					_attributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				}
				return _attributeValues;
			}
			set
			{
				_attributeValues = value;
			}
		}

		/// <summary>
		///   Gets or sets the view context (automatically set when using razor views).
		/// </summary>
		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		/// <summary>
		///   Initializes the html localizer to use when localizing attributes.
		/// </summary>
		/// <inheritdoc />
		public override void Init(TagHelperContext context)
		{
			_localizer = _localizerFactory.ResolveLocalizer(ViewContext, _applicationName);
		}

		/// <summary>
		///   Synchronously executes the
		///   <see cref="T:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" /> with the given
		///   <paramref name="context" /> and <paramref name="output" />. This is the method
		///   responsible for localizing the html attributes
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			foreach (var attribute in AttributeValues)
			{
				string key = attribute.Key;
				string value = attribute.Value;
				if (!string.IsNullOrWhiteSpace(value))
				{
					string newValue = _localizer.GetString(value);

					output.Attributes.Add(key, newValue);
				}
			}
		}
	}
}
