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
		private readonly IViewLocalizer _localizer;
		private IDictionary<string, string> _attributeValues;

		public LocalizeAttributeTagHelper(IViewLocalizer localizer)
		{
			if (localizer == null)
				throw new ArgumentNullException(nameof(localizer));

			this._localizer = localizer;
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
			if (_localizer is IViewContextAware)
			{
				((IViewContextAware)_localizer).Contextualize(ViewContext);
			}

			base.Init(context);
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
