// -----------------------------------------------------------------------
// <copyright file="LocalizeScopeOptions.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers
{
  using System;

  using Microsoft.AspNetCore.Mvc.Localization;

  public sealed class LocalizeScopeOptions
  {
    public bool? IsHtml { get; set; }

    public IHtmlLocalizer Localizer { get; set; }

    public string Name { get; set; }

    public NewLineHandling? NewLineHandling { get; set; }

    public bool? TrimEachLine { get; set; }

    public bool? TrimWhitespace { get; set; } = true;

    public Type Type { get; set; }

    public bool IsDefault()
    {
      return IsHtml == null
        && Localizer == null
        && Name == null
        && NewLineHandling == null
        && TrimEachLine == null
        && TrimWhitespace == null
        && Type == null;
    }

    internal void UpdateDefaultValues(in LocalizeScopeOptions options)
    {
      IsHtml ??= options.IsHtml;
      Localizer ??= options.Localizer;
      Name ??= options.Name;
      NewLineHandling ??= options.NewLineHandling;
      TrimEachLine ??= options.TrimEachLine;
      TrimWhitespace ??= options.TrimWhitespace;
      Type ??= options.Type;
    }
  }
}
