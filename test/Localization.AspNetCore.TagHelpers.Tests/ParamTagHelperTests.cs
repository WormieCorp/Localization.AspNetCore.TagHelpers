//-----------------------------------------------------------------------
// <copyright file="ParamTagHelperTests.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers.Tests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Text.Encodings.Web;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Xunit;

  public class ParamTagHelperTests
  {
    [Fact]
    public void ParameterOrderIsHigherThanLocalizeTagHelperOrder()
    {
      var tagHelper1 = TestHelper.CreateTagHelper<GenericLocalizeTagHelper>(null);
      var tagHelper2 = TestHelper.CreateTagHelper<LocalizeTagHelper>(null);
      var paramTagHelper = CreateTagHelper();

      Assert.True(paramTagHelper.Order > tagHelper1.Order);
      Assert.True(paramTagHelper.Order > tagHelper2.Order);
    }

    [Fact]
    public async Task ProcessAsync_AppendsLocalizedTextToExistingList()
    {
      var helper = CreateTagHelper();
      var context = CreateTagContext("First Localized", "Second Localized");
      var expected = new List<object>
      {
        "First Localized",
        "Second Localized",
        "Third Localized"
      };
      var output = CreateTagOutput("parameter", "Third Localized");
      var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];
      var parameters = stack.Peek();

      await helper.ProcessAsync(context, output);

      Assert.Equal(expected, parameters);
    }

    [Fact]
    public async Task ProcessAsync_DoesNothingIfNoStackIsAvailable()
    {
      var helper = CreateTagHelper();
      var context = CreateTagContext();
      context.Items.Remove(typeof(GenericLocalizeTagHelper));
      var output = CreateTagOutput("parameter", "My Localized String");

      var html = await CreateHtmlOutput(helper, context, output);

      Assert.Empty(context.Items);
      Assert.Equal("<parameter>My Localized String</parameter>", html);
    }

    [Fact]
    public async Task ProcessAsync_RemovesAllContent()
    {
      var helper = CreateTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("parameter", "My loc");

      await helper.ProcessAsync(context, output);

      Assert.True(output.Content.IsEmptyOrWhiteSpace);
    }

    [Fact]
    public async Task ProcessAsync_RemovesTagName()
    {
      var helper = CreateTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("parameter", "my Lc");

      await helper.ProcessAsync(context, output);

      Assert.Null(output.TagName);
    }

    private async Task<string> CreateHtmlOutput(TagHelper tagHelper, TagHelperContext tagContext, TagHelperOutput tagOutput)
    {
      var sb = new StringBuilder();

      await tagHelper.ProcessAsync(tagContext, tagOutput);

      using (var writer = new StringWriter(sb))
      {
        tagOutput.WriteTo(writer, HtmlEncoder.Default);
      }

      return sb.ToString();
    }

    private TagHelperContext CreateTagContext(params string[] localizedParameters)
    {
      var dictionary = new Dictionary<object, object>();
      var stack = new Stack<List<object>>();

      stack.Push(new List<object>(localizedParameters));
      dictionary.Add(typeof(GenericLocalizeTagHelper), stack);

      return new TagHelperContext(
        new TagHelperAttributeList(),
        dictionary,
        Guid.NewGuid().ToString());
    }

    private ParameterTagHelper CreateTagHelper()
    {
      return new ParameterTagHelper();
    }

    private TagHelperOutput CreateTagOutput(string name, string content, params TagHelperAttribute[] attributes)
    {
      return new TagHelperOutput(
        name,
        new TagHelperAttributeList(attributes),
        (useCachedResult, encoder) =>
        {
          var tagHelperContent = new DefaultTagHelperContent();
          tagHelperContent.SetContent(content);
          return Task.FromResult<TagHelperContent>(tagHelperContent);
        });
    }
  }
}
