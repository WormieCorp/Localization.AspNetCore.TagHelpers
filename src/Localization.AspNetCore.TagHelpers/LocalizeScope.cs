// -----------------------------------------------------------------------
// <copyright file="LocalizeScope.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Options;

  [HtmlTargetElement("localize-scope")]
  public class LocalizeScope : LocalizeScopeBase
  {
    private const string LOCALIZE_HTML = "html";
    private const string LOCALIZE_NAME = "resource-name";
    private const string LOCALIZE_NEWLINE = "newline";
    private const string LOCALIZE_TRIM = "trim";
    private const string LOCALIZE_TRIM_LINES = "trimlines";
    private const string LOCALIZE_TYPE = "resource-type";

    public LocalizeScope(IOptions<LocalizeTagHelperOptions> options)
      : base(options)
    {
    }

    [HtmlAttributeName(LOCALIZE_HTML)]
    public virtual bool IsHtml
    {
      get => Options.IsHtml.GetValueOrDefault();
      set => Options.IsHtml = value;
    }

    [HtmlAttributeName("localizer")]
    public IHtmlLocalizer Localizer
    {
      get => Options.Localizer;
      set => Options.Localizer = value;
    }

    [HtmlAttributeName(LOCALIZE_NAME)]
    public virtual string Name
    {
      get => Options.Name ?? string.Empty;
      set => Options.Name = value;
    }

    [HtmlAttributeName(LOCALIZE_NEWLINE)]
    public virtual NewLineHandling NewLineHandling
    {
      get => Options.NewLineHandling.GetValueOrDefault(NewLineHandling.Auto);
      set => Options.NewLineHandling = value;
    }

    [HtmlAttributeName(LOCALIZE_TRIM_LINES)]
    public virtual bool TrimEachLine
    {
      get => Options.TrimEachLine.GetValueOrDefault();
      set => Options.TrimEachLine = value;
    }

    [HtmlAttributeName(LOCALIZE_TRIM)]
    public virtual bool TrimWhitespace
    {
      get => Options.TrimWhitespace.GetValueOrDefault(true);
      set => Options.TrimWhitespace = value;
    }

    [HtmlAttributeName(LOCALIZE_TYPE)]
    public virtual Type Type
    {
      get => Options.Type;
      set => Options.Type = value;
    }
  }
}
