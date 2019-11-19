//-----------------------------------------------------------------------
// <copyright file="LocalizeTagHelperTests.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Localization.AspNetCore.TagHelpers.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Text.Encodings.Web;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.Extensions.Localization;
  using Microsoft.Extensions.Options;
  using Moq;
  using Xunit;

  public class LocalizeTagHelperTests
  {
    public static IEnumerable<object[]> LocalizeTestData
    {
      get
      {
        var encoder = HtmlEncoder.Default;

        yield return new object[] { "localize", "This will be localized", "This is the localized text", true, false, "This is the localized text" };
        var text = "This the the <small>localized</small> text with <strong>html</strong>";
        yield return new object[] { "p", "This wi be localized", text, true, false, encoder.Encode(text) };
        yield return new object[] { "span", "This", text, true, true, text };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, false, $"    {encoder.Encode(text)}    " };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, true, $"    {text}    " };
        yield return new object[] { "div", "     Localize    ", $"{text}", true, false, encoder.Encode(text) };
        yield return new object[] { "div", "    Localize    ", $"{text}", true, true, text };
      }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionOnHostingEnvironmentIsNull()
    {
      Assert.Throws<ArgumentNullException>(() => new LocalizeTagHelper(TestHelper.CreateFactoryMock(false).Object, null, null));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionOnHtmlLocalizerFactoryIsNull()
    {
      var hostMock =
#if NETCOREAPP3_0
  new Mock<IWebHostEnvironment>();
#else
  new Mock<IHostingEnvironment>();
#endif
      Assert.Throws<ArgumentNullException>(() => new LocalizeTagHelper(null, hostMock.Object, null));
    }

    [Theory]
    [InlineData("MyCustomName")]
    [InlineData("MyBase.Name")]
    public void Init_CreatesHtmlLocalizerWithUserSpecifiedName(string name)
    {
      var factory = TestHelper.CreateFactoryMock(true);
      var helper = CreateTagHelper(factory.Object);
      var context = TestHelper.CreateTagContext();
      helper.Name = name;

      helper.Init(context);

      factory.Verify(x => x.Create(name, TestHelper.ApplicationName), Times.Once());
    }

    [Theory]
    [InlineData(typeof(GenericLocalizeTagHelper))]
    [InlineData(typeof(TestHelper))]
    [InlineData(typeof(ParamTagHelperTests))]
    public void Init_CreatesHtmlLocalizerWithUserSpecifiedType(Type type)
    {
      var factory = TestHelper.CreateFactoryMock(true);
      var helper = CreateTagHelper(factory.Object);
      var context = TestHelper.CreateTagContext();
      helper.Type = type;

      helper.Init(context);

      factory.Verify(x => x.Create(type), Times.Once());
    }

    [Fact]
    public void Init_SkipsCreatingParameterStackIfInheritedClassSetsSupportsParametersToFalse()
    {
      var tagHelper = TestHelper.CreateTagHelper<LocalizeNoParametersTagHelper>(null);
      var tagContext = TestHelper.CreateTagContext();

      tagHelper.Init(tagContext);

      Assert.DoesNotContain(tagContext.Items, (item) => (Type)item.Key == typeof(GenericLocalizeTagHelper));
    }

    [Theory]
    [MemberData(nameof(LocalizeTestData))]
    public async Task ProcessAsync_CanLocalizeText(string tagName, string text, string expectedText, bool trim, bool isHtml, string expected)
    {
      if (text is null)
      {
        throw new ArgumentNullException(nameof(text));
      }

      var textToLocalize = trim ? text.Trim() : text;
      var localizer = TestHelper.CreateLocalizerMock(false);
      SetupLocalizer(localizer, textToLocalize, expectedText, isHtml);
      var factory = TestHelper.CreateFactoryMock(localizer.Object);
      var helper = CreateTagHelper(factory.Object);
      helper.TrimWhitespace = trim;
      helper.IsHtml = isHtml;

      var output = await TestHelper.GenerateHtmlAsync(helper, tagName, text).ConfigureAwait(false);

      if (isHtml)
      {
        localizer.Verify(x => x[textToLocalize], Times.Once);
      }
      else
      {
        localizer.Verify(x => x.GetString(textToLocalize), Times.Once);
      }

      Assert.Equal(expected, output);
    }

    [Fact]
    public async Task ProcessAsync_RemovesTagName()
    {
      var tagHelper = CreateTagHelper();
      var expected = "This should be the only content";

      var output = await TestHelper.GenerateHtmlAsync(tagHelper, "localize", expected).ConfigureAwait(false);

      Assert.Equal(expected, output);
    }

    protected static LocalizeTagHelper CreateTagHelper()
    {
      return TestHelper.CreateTagHelper<LocalizeTagHelper>(null);
    }

    protected static LocalizeTagHelper CreateTagHelper(IHtmlLocalizerFactory factory)
    {
      return TestHelper.CreateTagHelper<LocalizeTagHelper>(factory);
    }

    private void SetupLocalizer(Mock<IHtmlLocalizer> localizer, string textToLocalize, string expectedText, bool isHtml)
    {
      if (isHtml)
      {
        localizer.Setup(x => x[textToLocalize]).Returns<string>(s => new LocalizedHtmlString(s, expectedText, s == expectedText));
      }
      else
      {
        localizer.Setup(x => x.GetString(textToLocalize)).Returns<string>(s => new LocalizedString(s, expectedText, s == expectedText));
      }
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    private class LocalizeNoParametersTagHelper : LocalizeTagHelper
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
#if NETCOREAPP3_0
      public LocalizeNoParametersTagHelper(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#else
      public LocalizeNoParametersTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
#endif
        : base(localizerFactory, hostingEnvironment, options)
      {
      }

      protected override bool SupportsParameters => false;
    }
  }
}
