// -----------------------------------------------------------------------
// <copyright file="LocalizeTagHelperOptions.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  /// <summary>
  /// Global optionsto pass to the supported tag helpers.
  /// </summary>
  /// <seealso cref="GenericLocalizeTagHelper"/>
  /// <seealso cref="LocalizeTagHelper"/>
  /// <seealso cref="HtmlTagHelper"/>
  public class LocalizeTagHelperOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether the localize tag helpers should localize its content
    /// by default. (Can be overridden by using <c>html= <see langword="false"/></c> when calling
    /// one of the localize tag helpers).
    /// </summary>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool HtmlEncodeByDefault { get; set; } = true;

    /// <summary>
    /// Gets or sets what new lines should be normalized to, or <see cref="NewLineHandling.None"/>
    /// to use existing line endings.
    /// </summary>
    /// <remarks>Defaults to <see cref="NewLineHandling.Auto"/>.</remarks>
    public NewLineHandling NewLineHandling { get; set; } = NewLineHandling.Auto;

    /// <summary>
    /// Gets or sets a value indicating whether the <c>lang</c> attribute on the html element should
    /// automatically be set to the current ui culture or not.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>. Will also only set the two-letter iso code of the current ui culture.
    /// </remarks>
    public bool SetHtmlLanguageAttribute { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether beginning and ending whitespace.
    /// </summary>
    /// <value><c>true</c> to trim beginning and ending whitespace; otherwise, <c>false</c>.</value>
    /// <remarks>Defaults to <c>true</c>.</remarks>
    public bool TrimWhitespace { get; set; } = true;
  }
}
