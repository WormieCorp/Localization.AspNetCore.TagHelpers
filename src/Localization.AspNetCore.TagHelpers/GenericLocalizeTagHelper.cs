// -----------------------------------------------------------------------
// <copyright file="GenericLocalizeTagHelper.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  using Localization.AspNetCore.TagHelpers.Internals;

  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Html;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.AspNetCore.Mvc.ViewFeatures;
  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Localization;
  using Microsoft.Extensions.Options;

  /// <summary>
  /// Adds support to localize the inner text for a tag, when one of the following attributes have
  /// been added: <c>localize</c>, <c>localize-html</c> or <c>localize-type</c>.
  /// </summary>
  /// <seealso cref="TagHelper"/>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// <span localize="">
  /// To text to localize goes here
  /// </span>
  /// ]]>
  /// </code>
  /// </example>
  [HtmlTargetElement(Attributes = "localize")]
  public class GenericLocalizeTagHelper : TagHelper
  {
    private const string LOCALIZE_HTML = "html";
    private const string LOCALIZE_NAME = "resource-name";
    private const string LOCALIZE_NEWLINE = "newline";
    private const string LOCALIZE_TRIM = "trim";
    private const string LOCALIZE_TRIM_LINES = "trimlines";
    private const string LOCALIZE_TYPE = "resource-type";
    private readonly string applicationName;
    private readonly IHtmlLocalizerFactory localizerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericLocalizeTagHelper"/> class.
    /// </summary>
    /// <param name="localizerFactory">
    /// The localizer factory to create a <see cref="IHtmlLocalizer"/> from.
    /// </param>
    /// <param name="hostingEnvironment">The hosting environment.</param>
    /// <param name="options">The default options unless overridden when calling the tag helper.</param>
#if NETCOREAPP3_0
    public GenericLocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#else
    public GenericLocalizeTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#endif
    {
      if (hostingEnvironment is null)
      {
        throw new ArgumentNullException(nameof(hostingEnvironment));
      }

      this.localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
      applicationName = hostingEnvironment.ApplicationName;

      if (options != null)
      {
        NewLineHandling = options.Value.NewLineHandling;
        TrimWhitespace = options.Value.TrimWhitespace;
        IsHtml = !options.Value.HtmlEncodeByDefault;
      }
      else
      {
        NewLineHandling = NewLineHandling.Auto;
        TrimWhitespace = true;
        IsHtml = false;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the inner text should be treated as HTML (no encoding).
    /// </summary>
    /// <value><c>true</c> if the inner text is HTML; otherwise, <c>false</c>.</value>
    /// <remarks>This defaults to false.</remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <ul>
    ///   <li localize-html="true">
    ///     <a href="~/home">Home</a>
    ///   </li>
    /// </ul>
    /// ]]>
    /// </code>
    /// </example>
    [HtmlAttributeName(LOCALIZE_HTML)]
    public virtual bool IsHtml { get; set; }

    /// <summary>
    /// Gets or sets the localizer to use when resolving the specified strings.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// @inject Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer loc;
    ///
    /// <localize localizer="@loc">
    ///   Text to localize
    /// </localize>
    /// ]]>
    /// </code>
    /// </example>
    [HtmlAttributeName("localizer")]
    public IHtmlLocalizer Localizer { get; set; }

    /// <summary>
    /// Gets or sets the name to optionally override the name/path of the resource file. If the name
    /// is empty it resolves to the current path and name of the view. i.e the view located at
    /// <c>"~/Views/Home/About.cshtml"</c> passes the following name to the html localizer as <c>Views/Home/About</c>.
    /// </summary>
    /// <value>The optional name/path to the resource file.</value>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <span localize="MyCustomResource">
    ///   The text to localize.
    /// </span>
    /// ]]>
    /// </code>
    /// Passes the path as <c>~/MyCustomResource</c>.
    /// </example>
    [HtmlAttributeName(LOCALIZE_NAME)]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new line handing method.
    /// </summary>
    [HtmlAttributeName(LOCALIZE_NEWLINE)]
    public virtual NewLineHandling NewLineHandling { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to trim whitespace on each line. <note type="note">
    /// If enabled, this will override the <see cref="TrimWhitespace"/>.</note>
    /// </summary>
    /// <value><see langword="true"/> to trim whitespace on each line; otherwise, <see langword="false"/>.</value>
    [HtmlAttributeName(LOCALIZE_TRIM_LINES)]
    public virtual bool TrimEachLine { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether beginning and ending whitespace.
    /// </summary>
    /// <value><c>true</c> to trim beginning and ending whitespace; otherwise, <c>false</c>.</value>
    [HtmlAttributeName(LOCALIZE_TRIM)]
    public virtual bool TrimWhitespace { get; set; }

    /// <summary>
    /// Gets or sets the type to use when looking up the resource file.
    /// </summary>
    /// <value>The type to use when looking up the resource file.</value>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <span localize-type="typeof(Localization.Demo.Models.SharedType)">
    ///   The text
    /// </span>
    /// ]]>
    /// </code>
    /// Creates a new html localizer passing the specified type.
    /// </example>
    [HtmlAttributeName(LOCALIZE_TYPE)]
    public virtual Type Type { get; set; }

    /// <summary>
    /// Gets or sets the view context (automatically set when using razor views).
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// Gets a value indicating whether this localizer tag helper supports parameters.
    /// </summary>
    /// <value><c>true</c> if this tag helper supports parameters; otherwise, <c>false</c>.</value>
    /// <remarks>Defaults to <see langword="true"/>, but may be overridden in inherited <c>class</c>.</remarks>
    protected virtual bool SupportsParameters => true;

    /// <summary>
    /// Initializes this Localize Tag Helpers, setting the html localizer and creating a stack list
    /// for child tag helpers to add parameters to.
    /// </summary>
    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      if (Localizer is null)
      {
        Localizer = localizerFactory.ResolveLocalizer(ViewContext, applicationName, Type, Name);
      }

      if (!SupportsParameters)
      {
        return;
      }

      Stack<List<object>> currentStack;

      if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
      {
        currentStack = new Stack<List<object>>();
        context.Items.Add(typeof(GenericLocalizeTagHelper), currentStack);
      }
      else
      {
        currentStack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];
      }

      currentStack.Push(new List<object>());
    }

    /// <summary>
    /// The function responsible for acquiring the text to localize, getting the required parameters
    /// to pass to the localized text and replacing the old text with the new localized text.
    /// </summary>
    /// <inheritdoc/>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      if (output is null)
      {
        throw new ArgumentNullException(nameof(output));
      }

      return ProcessInternalAsync(context, output);
    }

    /// <summary>
    /// Gets the content/text that are to be localized.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
    /// <returns>An asynchronous task with the found content.</returns>
    protected virtual Task<string> GetContentAsync(TagHelperContext context, TagHelperOutput output)
    {
      if (output is null)
      {
        throw new ArgumentNullException(nameof(output));
      }

      return GetContentInternalAsync(output);
    }

    /// <summary>
    /// Gets the parameters to use when localizing the text.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <returns>A Enumerable object filled with the necessary parameters.</returns>
    protected virtual IEnumerable<object> GetParameters(TagHelperContext context)
    {
      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      if (!context.Items.ContainsKey(typeof(GenericLocalizeTagHelper)))
      {
        return Array.Empty<object>();
      }

      var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];

      return stack.Pop();
    }

    /// <summary>
    /// Sets the localized content back to where the original content was.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <param name="outputContent">Content of the output tag helper.</param>
    /// <param name="content">The content to set.</param>
    protected virtual void SetContent(TagHelperContext context, TagHelperContent outputContent, string content)
    {
      if (outputContent is null)
      {
        throw new ArgumentNullException(nameof(outputContent));
      }

      outputContent.SetContent(content);
    }

    /// <summary>
    /// Sets the localized content without encoding it back to where the original content was.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <param name="outputContent">Content of the output tag helper.</param>
    /// <param name="htmlContent">The content to set.</param>
    protected virtual void SetHtmlContent(TagHelperContext context, TagHelperContent outputContent, IHtmlContent htmlContent)
    {
      if (outputContent is null)
      {
        throw new ArgumentNullException(nameof(outputContent));
      }

      outputContent.SetHtmlContent(htmlContent);
    }

    private static void AppendContent(ref StringBuilder newContent, in string content, in bool trimEachLine, in int lastIndex, in int index)
    {
      var substring = content.Substring(lastIndex, index - lastIndex);

      newContent.Append(trimEachLine ? substring.Trim() : substring.TrimEnd('\r'));
    }

    private static void AppendNewLine(ref StringBuilder newContent, in string content, in bool trimEachLine, in int index, in string newLine)
    {
      if (newLine is null)
      {
        if (trimEachLine && index > 0 && content[index - 1] == '\r')
        {
          newContent.Append("\r\n");
        }
        else
        {
          newContent.Append('\n');
        }
      }
      else
      {
        newContent.Append(newLine);
      }
    }

    private static async Task<string> GetContentInternalAsync(TagHelperOutput output)
    {
      var content = await output.GetChildContentAsync(true).ConfigureAwait(false);
      if (output.IsContentModified)
      {
        return output.Content.GetContent(NullHtmlEncoder.Default);
      }

      return content.GetContent(NullHtmlEncoder.Default);
    }

    private static string GetDefaultNewLine(in NewLineHandling newLineHandling)
    {
      var newLine = newLineHandling switch
      {
        NewLineHandling.Auto => Environment.NewLine,
        NewLineHandling.Windows => "\r\n",
        NewLineHandling.Unix => "\n",
        _ => null,
      };

      return newLine;
    }

    private static void HandleNormalization(ref string content, in NewLineHandling newLineHandling, in bool trimWhitespace, in bool trimEachLine)
    {
      if (string.IsNullOrEmpty(content))
      {
        return;
      }

      if (trimWhitespace || trimEachLine)
      {
        content = content.Trim();
      }

      if (newLineHandling == NewLineHandling.None && !trimEachLine)
      {
        return;
      }

      var newContent = new StringBuilder();
      var lastIndex = 0;
      int index;
      var newLine = GetDefaultNewLine(newLineHandling);

      while ((index = content.IndexOf('\n', lastIndex)) >= 0)
      {
        AppendContent(ref newContent, content, trimEachLine, lastIndex, index);
        AppendNewLine(ref newContent, content, trimEachLine, index, newLine);

        lastIndex = index + 1;
      }

      AppendContent(ref newContent, content, trimEachLine, lastIndex, content.Length);

      content = newContent.ToString();
    }

    private static void RemoveAttribute(ref TagHelperOutput output, in string name)
    {
      if (output.Attributes.ContainsName(name))
      {
        var index = output.Attributes.IndexOfName(name);
        output.Attributes.RemoveAt(index);
      }
    }

    private async Task ProcessInternalAsync(TagHelperContext context, TagHelperOutput output)
    {
      RemoveAttribute(ref output, "localize");

      var content = await GetContentAsync(context, output).ConfigureAwait(false);

      HandleNormalization(ref content, NewLineHandling, TrimWhitespace, TrimEachLine);

      var parameters = GetParameters(context);
      if (IsHtml)
      {
        var locString = parameters.Any()
          ? Localizer[content, parameters.ToArray()]
          : Localizer[content];

        SetHtmlContent(context, output.Content, locString);
      }
      else
      {
        var locString = parameters.Any()
          ? Localizer.GetString(content, parameters.ToArray())
          : Localizer.GetString(content);

        SetContent(context, output.Content, locString);
      }
    }
  }
}
