// -----------------------------------------------------------------------
// <copyright file="HtmlTagHelperTests.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license.
//   See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
// -----------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace Localization.AspNetCore.TagHelpers.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Text;
  using System.Text.Encodings.Web;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Options;

  using Moq;

  using Xunit;

  public class HtmlTagHelperTests
  {
    [Theory]
    [InlineData("nb-NO")]
    [InlineData("en-UK")]
    [InlineData("da")]
    [InlineData("ee")]
    public void Process_ShouldAddLangAttributeToCurrentUiCulture(string culture)
    {
      var cultureInfo = CultureInfo.CreateSpecificCulture(culture);
      Thread.CurrentThread.CurrentUICulture = cultureInfo;
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput);

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal($"<html lang=\"{cultureInfo.TwoLetterISOLanguageName}\"></html>", htmlOutput);
    }

    [Theory]
    [InlineData("nb-NO")]
    [InlineData("en-UK")]
    [InlineData("da")]
    [InlineData("ee")]
    public void Process_ShouldAddXmlLangAttributeWhenXmlnsIsUsed(string culture)
    {
      var cultureInfo = CultureInfo.CreateSpecificCulture(culture);
      Thread.CurrentThread.CurrentUICulture = cultureInfo;
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput);
      tagOutput.Attributes.Add("xmlns", "http://www.w3.org/1999/xhtml");

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal($"<html xmlns=\"http://www.w3.org/1999/xhtml\" lang=\"{cultureInfo.TwoLetterISOLanguageName}\" xml:lang=\"{cultureInfo.TwoLetterISOLanguageName}\"></html>", htmlOutput);
    }

    [Fact]
    public void Process_ShouldNotAddLangElementWhenUserHaveDisabledInGlobalOptions()
    {
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput, new LocalizeTagHelperOptions { SetHtmlLanguageAttribute = false });

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal("<html></html>", htmlOutput);
    }

    [Fact]
    public void Process_ShouldNotAddXmlLangElementWhenUserHaveDisabledInGlobalOptions()
    {
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput, new LocalizeTagHelperOptions { SetHtmlLanguageAttribute = false });
      tagOutput.Attributes.Add("xmlns", "http://www.w3.org/1999/xhtml");

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal("<html xmlns=\"http://www.w3.org/1999/xhtml\"></html>", htmlOutput);
    }

    [Fact]
    public void Process_ShouldNotReplaceExistingLangAttribute()
    {
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput);
      tagOutput.Attributes.Add("lang", "nope");

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal("<html lang=\"nope\"></html>", htmlOutput);
    }

    [Fact]
    public void Process_ShouldNotReplaceExistingXmlLangAttribute()
    {
      var tagHelper = CreateTagHelper(out var tagContext, out var tagOutput);
      tagOutput.Attributes.Add("xmlns", "http://www.w3.org/1999/xhtml");
      tagOutput.Attributes.Add("xml:lang", "nope");

      var htmlOutput = CreateHtmlOutput(tagHelper, tagContext, tagOutput);

      Assert.Equal($"<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"nope\" lang=\"{Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName}\"></html>", htmlOutput);
    }

    [Fact]
    public void Process_ShouldThrowArgumentNullExceptienWhenTagOutputIsNull()
    {
      var tagHelper = CreateTagHelper(out var tagContext, out var _);

      Assert.Throws<ArgumentNullException>(() => CreateHtmlOutput(tagHelper, tagContext, null));
    }

    private static TagHelper CreateTagHelper(out TagHelperContext tagContext, out TagHelperOutput tagOutput, LocalizeTagHelperOptions options = null)
    {
      var optionsMock = new Mock<IOptions<LocalizeTagHelperOptions>>();
      optionsMock.SetupGet(s => s.Value).Returns(options);

      var tagHelper = new HtmlTagHelper(optionsMock.Object);
      tagContext = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
      tagOutput = new TagHelperOutput(
        "html",
new TagHelperAttributeList(),
(useCachedResult, encoder) =>
{
  var tagHelperContent = new DefaultTagHelperContent();
  return Task.FromResult<TagHelperContent>(tagHelperContent);
});
      return tagHelper;
    }

    private string CreateHtmlOutput(TagHelper tagHelper, TagHelperContext tagContext, TagHelperOutput tagOutput)
    {
      tagHelper.Init(tagContext);
      var sb = new StringBuilder();

      tagHelper.Process(tagContext, tagOutput);
      var contentTask = tagOutput.GetChildContentAsync();
      contentTask.Wait();
      tagOutput.Content = contentTask.Result;

      using (var writer = new StringWriter(sb))
      {
        tagOutput.WriteTo(writer, HtmlEncoder.Default);
      }

      return sb.ToString();
    }
  }
}
