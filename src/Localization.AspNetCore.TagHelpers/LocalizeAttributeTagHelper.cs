using Localization.AspNetCore.TagHelpers.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

namespace Localization.AspNetCore.TagHelpers
{
	// You may need to install the Microsoft.AspNet.Razor.Runtime package into
	// your project
	[HtmlTargetElement(Attributes = LOCALIZE_ATTRIBUTE_PREFIX + "*")]
	public class LocalizeAttributeTagHelper : TagHelper
	{
		private const string LOCALIZE_ATTRIBUTE_PREFIX = "asp-localize-";
		private const string LOCALIZE_DICTIONARY_NAME = "asp-localize-all";
		private readonly string _applicationName;
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private IDictionary<string, string> _attributeValues;
		private IHtmlLocalizer _localizer;

		public LocalizeAttributeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
		{
			Throws.NotNull(localizerFactory, nameof(localizerFactory));
			Throws.NotNull(hostingEnvironment, nameof(hostingEnvironment));

			this._localizerFactory = localizerFactory;
			this._applicationName = hostingEnvironment.ApplicationName;
		}

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

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Init(TagHelperContext context)
		{
			_localizer = _localizerFactory.ResolveLocalizer(ViewContext, _applicationName);
		}

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
