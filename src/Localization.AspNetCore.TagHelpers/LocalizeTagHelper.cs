// -----------------------------------------------------------------------
// <copyright file="LocalizeTagHelper.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Options;

  /// <summary>
  ///   Adds support to localize the everything with the <c>localize</c> tag.
  /// </summary>
  /// <seealso cref="TagHelper" />
  /// <seealso cref="GenericLocalizeTagHelper" />
  /// <example>
  ///   <code>
  /// <![CDATA[
  /// <localize>
  /// To text to localize goes here
  /// </localize>
  /// ]]>
  ///   </code>
  /// </example>
  [HtmlTargetElement(LOCALIZE_TAG_NAME)]
  public class LocalizeTagHelper : GenericLocalizeTagHelper
  {
    private const string LOCALIZE_TAG_NAME = "localize";

    /// <summary>
    ///   Initializes a new instance of the <see cref="LocalizeTagHelper" /> class.
    /// </summary>
    /// <param name="localizerFactory">
    ///   The localizer factory to create a <see cref="IHtmlLocalizer" /> from.
    /// </param>
    /// <param name="hostingEnvironment">The hosting environment.</param>
    /// <param name="options">The default options unless overridden when calling the tag helper.</param>
#if NETCOREAPP3_0
    public LocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#else
    public LocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#endif
      : base(localizerFactory, hostingEnvironment, options)
    {
    }

    /// <summary>
    ///   This function first calls the base ProcessAsync method, then removes the tag name.
    /// </summary>
    /// <seealso cref="GenericLocalizeTagHelper.ProcessAsync(TagHelperContext, TagHelperOutput)" />
    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      if (output is null)
      {
        throw new System.ArgumentNullException(nameof(output));
      }

      output.TagName = null;

      return base.ProcessAsync(context, output);
    }
  }
}
