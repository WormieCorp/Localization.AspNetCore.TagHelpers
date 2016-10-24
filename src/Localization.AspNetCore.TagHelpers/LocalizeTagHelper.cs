//-----------------------------------------------------------------------
// <copyright file="LocalizeTagHelper.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Razor.TagHelpers;

  /// <summary>
  ///   Adds support to localize the everything with the <c>localize</c> tag.
  /// </summary>
  /// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
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
    public LocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
      : base(localizerFactory, hostingEnvironment)
    {
    }

#pragma warning disable S1185 // this must be overridden to change the html attribute to use

    /// <summary>
    ///   Gets or sets a value indicating whether the inner text should be treated as HTML (no encoding).
    /// </summary>
    /// <value><c>true</c> if the inner text is HTML; otherwise, <c>false</c>.</value>
    /// <example>
    ///   <code>
    /// <![CDATA[
    /// <localize html="true">
    /// <a href="~/home">Home</a>
    /// </localize>
    /// ]]>
    ///   </code>
    /// </example>
    /// <remarks>This defaults to false.</remarks>
    [HtmlAttributeName("html")]
    public override bool IsHtml
    {
      get
      {
        return base.IsHtml;
      }

      set
      {
        base.IsHtml = value;
      }
    }

    /// <summary>
    ///   Optionally overrides the name/path of the resource file. If the name is empty it resolves
    ///   to the current path and name of the view. i.e the view located at
    ///   <c>"~/Views/Home/About.cshtml"</c> passes the following name to the html localizer as <c>Views/Home/About</c>
    /// </summary>
    /// <value>The optional name/path to the resource file.</value>
    /// <example>
    ///   <code>
    /// <![CDATA[
    /// <localize name="MyCustomResource">
    /// The text to localize.
    /// </localize>
    /// ]]>
    ///   </code>
    ///   Passes the path as <c>~/MyCustomResource</c>
    /// </example>
    [HtmlAttributeName("name")]
    public override string Name
    {
      get
      {
        return base.Name;
      }

      set
      {
        base.Name = value;
      }
    }

    /// <inheritdoc />
    [HtmlAttributeName("trim")]
    public override bool TrimWhitespace
    {
      get
      {
        return base.TrimWhitespace;
      }

      set
      {
        base.TrimWhitespace = value;
      }
    }

    /// <summary>
    ///   Gets or sets the type to use when looking up the resource file.
    /// </summary>
    /// <value>The type to use when looking up the resource file.</value>
    /// <example>
    ///   <code>
    /// <![CDATA[
    /// <localize type="typeof(Localization.Demo.Models.SharedType)">
    /// The text
    /// </localize>
    /// ]]>
    ///   </code>
    ///   Creates a new html localizer passing the specified type.
    /// </example>
    [HtmlAttributeName("type")]
    public override Type Type
    {
      get
      {
        return base.Type;
      }

      set
      {
        base.Type = value;
      }
    }

#pragma warning restore S1185 // Overriding members should do more than simply call the same member in the super class

    /// <summary>
    ///   This function first calls the base ProcessAsync method, then removes the tag name.
    /// </summary>
    /// <seealso cref="GenericLocalizeTagHelper.ProcessAsync(TagHelperContext, TagHelperOutput)" />
    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      await base.ProcessAsync(context, output);

      output.TagName = null;
    }
  }
}
