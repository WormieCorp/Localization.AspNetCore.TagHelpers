// -----------------------------------------------------------------------
// <copyright file="LocalizeAttributeTagHelper.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using System.Collections.Generic;
  using Localization.AspNetCore.TagHelpers.Internals;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.AspNetCore.Mvc.ViewFeatures;
  using Microsoft.AspNetCore.Razor.TagHelpers;

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
  /// <example>
  ///   <code>
  /// <![CDATA[
  /// <abbr localize-title="This will be localized. {0}" params-title="This will not, but will be inserted as a parameter for title">This will not</abbr>
  /// ]]>
  ///   </code>
  /// </example>
  [HtmlTargetElement(Attributes = LOCALIZE_ATTRIBUTE_PREFIX + "*")]
  public class LocalizeAttributeTagHelper : TagHelper
  {
    private const string LOCALIZE_ATTRIBUTE_PARAMETER_PREFIX = "params-";
    private const string LOCALIZE_ATTRIBUTE_PREFIX = "localize-";
    private const string LOCALIZE_DICTIONARY_NAME = "localize-all";
    private const string LOCALIZE_DICTIONARY_PARAMETER_NAME = "params-all";
    private readonly string applicationName;
    private readonly IHtmlLocalizerFactory localizerFactory;
    private IDictionary<string, string> attributeValues;
    private IHtmlLocalizer localizer;

    private IDictionary<string, string> parameterValues;

    /// <summary>
    ///   Initializes a new instance of the <see cref="LocalizeAttributeTagHelper"/> class.
    /// </summary>
    /// <param name="localizerFactory">The localizer factory.</param>
    /// <param name="hostingEnvironment">The hosting environment.</param>
#if NETCOREAPP3_0
    public LocalizeAttributeTagHelper(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment)
#else
    public LocalizeAttributeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
#endif
    {
      if (hostingEnvironment is null)
      {
        throw new ArgumentNullException(nameof(hostingEnvironment));
      }

      this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
      applicationName = hostingEnvironment.ApplicationName;
    }

    /// <summary>
    ///   Gets or sets all the attribute values that are to be localized.
    /// </summary>
    [HtmlAttributeName(LOCALIZE_DICTIONARY_NAME, DictionaryAttributePrefix = LOCALIZE_ATTRIBUTE_PREFIX)]
    public IDictionary<string, string> AttributeValues
    {
      get
      {
        if (attributeValues == null)
        {
          attributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return attributeValues;
      }

      set => attributeValues = value;
    }

    /// <summary>
    /// Gets or sets the parameter values that are to be formatted into the localized attributes.
    /// </summary>
    [HtmlAttributeName(LOCALIZE_DICTIONARY_PARAMETER_NAME, DictionaryAttributePrefix = LOCALIZE_ATTRIBUTE_PARAMETER_PREFIX)]
    public IDictionary<string, string> ParameterValues
    {
      get
      {
        if (parameterValues == null)
        {
          parameterValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return parameterValues;
      }

      set => parameterValues = value;
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
      localizer = localizerFactory.ResolveLocalizer(ViewContext, applicationName);
    }

    /// <summary>
    ///   Synchronously executes the <see cref="TagHelper" />
    ///   with the given <paramref name="context" /> and <paramref name="output" />. This is the
    ///   method responsible for localizing the html attributes.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (output is null)
      {
        throw new ArgumentNullException(nameof(output));
      }

      foreach (var attribute in AttributeValues)
      {
        var key = attribute.Key;
        var value = attribute.Value;
        if (!string.IsNullOrWhiteSpace(value))
        {
          string newValue = null;

          if (ParameterValues.ContainsKey(key))
          {
            newValue = localizer.GetString(value, ParameterValues[key].Split(';'));
          }
          else
          {
            newValue = localizer.GetString(value);
          }

          output.Attributes.Add(key, newValue);
        }
      }
    }
  }
}
