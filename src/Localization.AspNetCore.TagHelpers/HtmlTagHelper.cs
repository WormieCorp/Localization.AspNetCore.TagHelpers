// -----------------------------------------------------------------------
// <copyright file="HtmlTagHelper.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license.
//   See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
// -----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using System.Globalization;

  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Options;

  /// <summary>
  /// Simple tag helper for automatically setting the lang and optionally xml:lang attributes.
  /// Implements the <see cref="TagHelper" />.
  /// </summary>
  /// <seealso cref="TagHelper" />
  [HtmlTargetElement("html")]
  public class HtmlTagHelper : TagHelper
  {
    private readonly bool setLanguageAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlTagHelper"/> class.
    /// </summary>
    /// <param name="options">The default options unless overridden when calling the tag helper.</param>
    public HtmlTagHelper(IOptions<LocalizeTagHelperOptions> options)
    {
      setLanguageAttribute = options?.Value?.SetHtmlLanguageAttribute ?? true;
    }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (output is null)
      {
        throw new ArgumentNullException(nameof(output));
      }

      if (!setLanguageAttribute)
      {
        return;
      }

      var cultureName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
      if (!output.Attributes.ContainsName("lang"))
      {
        output.Attributes.SetAttribute("lang", cultureName);
      }

      if (output.Attributes.ContainsName("xmlns") && !output.Attributes.ContainsName("xml:lang"))
      {
        output.Attributes.SetAttribute("xml:lang", cultureName);
      }
    }
  }
}
