// -----------------------------------------------------------------------
// <copyright file="ParameterTagHelper.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Localization.AspNetCore.TagHelpers.Internals;

  using Microsoft.AspNetCore.Razor.TagHelpers;

  /// <summary>
  ///   Adds parameters to <see cref="GenericLocalizeTagHelper" /> and
  ///   <see cref="LocalizeTagHelper" />. Used through a <c>parameter</c> html tag.
  /// </summary>
  /// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
  [HtmlTargetElement("parameter", TagStructure = TagStructure.NormalOrSelfClosing)]
  public class ParameterTagHelper : TagHelper
  {
    /// <inheritdoc />
    public override int Order => 2;

    /// <summary>
    ///   This method adds parameters to the parent tag helper if they are a
    ///   <see cref="GenericLocalizeTagHelper" /> or a <see cref="LocalizeTagHelper" />.
    /// </summary>
    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      if (context is null)
      {
        throw new System.ArgumentNullException(nameof(context));
      }

      if (output is null)
      {
        throw new System.ArgumentNullException(nameof(output));
      }

      var content = await output.GetChildContentAsync(NullHtmlEncoder.Default).ConfigureAwait(false);
      if (output.IsContentModified)
      {
        content = output.Content;
      }

      if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
      {
        output.Content = content;
        return;
      }

      var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];

      var existingItems = stack.Peek();
      existingItems.Add(content.GetContent(NullHtmlEncoder.Default));

      output.SuppressOutput();
      output.TagName = null;
    }
  }
}
