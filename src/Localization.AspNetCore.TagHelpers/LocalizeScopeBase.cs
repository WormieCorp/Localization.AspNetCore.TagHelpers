// -----------------------------------------------------------------------
// <copyright file="LocalizeScopeBase.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using System.Collections.Generic;

  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Options;

  public abstract class LocalizeScopeBase : TagHelper
  {
    private readonly LocalizeScopeOptions globalOptions;
    private bool scopeSaved;

    protected LocalizeScopeBase(IOptions<LocalizeScopeOptions> options)
    {
      globalOptions = options?.Value ?? new LocalizeScopeOptions();
    }

    protected LocalizeScopeOptions Options { get; } = new LocalizeScopeOptions();

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      var currentStack = UpdateWithScopedOptions(context);

      Options.UpdateDefaultValues(globalOptions);

      if (!Options.IsDefault())
      {
        scopeSaved = true;

        currentStack.Push(Options);

        context.Items[typeof(LocalizeScopeBase)] = currentStack;
      }
    }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      base.Process(context, output);

      if (scopeSaved)
      {
        var currentStack = (Stack<LocalizeScopeOptions>)context.Items[typeof(LocalizeScopeBase)];
        currentStack.Pop();
        context.Items[typeof(LocalizeScopeBase)] = currentStack;
      }
    }

    private Stack<LocalizeScopeOptions> UpdateWithScopedOptions(TagHelperContext context)
    {
      if (context.Items.ContainsKey(typeof(LocalizeScopeBase)))
      {
        var currentStack = (Stack<LocalizeScopeOptions>)context.Items[typeof(LocalizeScopeBase)];
        var options = currentStack.Pop();

        if (options != null)
        {
          Options.UpdateDefaultValues(options);
        }

        return currentStack;
      }

      return new Stack<LocalizeScopeOptions>();
    }
  }
}
