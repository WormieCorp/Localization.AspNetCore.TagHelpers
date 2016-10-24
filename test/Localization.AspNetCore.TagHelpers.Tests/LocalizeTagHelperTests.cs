//-----------------------------------------------------------------------
// <copyright file="LocalizeTagHelperTests.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers.Tests
{
  using System;
  using System.Collections;
  using System.Text.Encodings.Web;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.Extensions.Localization;
  using Moq;
  using NUnit.Framework;

  public class LocalizeTagHelperTests
  {
    public static IEnumerable LocalizeTestData
    {
      get
      {
        var encoder = HtmlEncoder.Default;

        yield return new TestCaseData("localize", "This will be localized", "This is the localized text", true, false)
          .Returns("This is the localized text");
        var text = "This the the <small>localized</small> text with <strong>html</strong>";
        yield return new TestCaseData("p", "This wi be localized", text, true, false)
          .Returns(encoder.Encode(text));
        yield return new TestCaseData("span", "This", text, true, true)
          .Returns(text);
        yield return new TestCaseData("div", "Localize", $"    {text}    ", false, false)
          .Returns($"    {encoder.Encode(text)}    ");
        yield return new TestCaseData("div", "Localize", $"    {text}    ", false, true)
          .Returns($"    {text}    ");
        yield return new TestCaseData("div", "     Localize    ", $"{text}", true, false)
          .Returns(encoder.Encode(text));
        yield return new TestCaseData("div", "    Localize    ", $"{text}", true, true)
          .Returns(text);
      }
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionOnHostingEnvironmentIsNull()
    {
      Assert.That(() => new LocalizeTagHelper(TestHelper.CreateFactoryMock(false).Object, null), Throws.ArgumentNullException);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionOnHtmlLocalizerFactoryIsNull()
    {
      Assert.That(() => new LocalizeTagHelper(null, new Mock<IHostingEnvironment>().Object), Throws.ArgumentNullException);
    }

    [TestCase("MyCustomName")]
    [TestCase("MyBase.Name")]
    public void Init_CreatesHtmlLocalizerWithUserSpecifiedName(string name)
    {
      var factory = TestHelper.CreateFactoryMock(true);
      var helper = CreateTagHelper(factory.Object);
      var context = TestHelper.CreateTagContext();
      helper.Name = name;

      helper.Init(context);

      factory.Verify(x => x.Create(name, TestHelper.ApplicationName), Times.Once());
    }

    [TestCase(typeof(GenericLocalizeTagHelper))]
    [TestCase(typeof(TestHelper))]
    [TestCase(typeof(ParamTagHelperTests))]
    public void Init_CreatesHtmlLocalizerWithUserSpecifiedType(Type type)
    {
      var factory = TestHelper.CreateFactoryMock(true);
      var helper = CreateTagHelper(factory.Object);
      var context = TestHelper.CreateTagContext();
      helper.Type = type;

      helper.Init(context);

      factory.Verify(x => x.Create(type), Times.Once());
    }

    [Test]
    public void Init_SkipsCreatingParameterStackIfInheritedClassSetsSupportsParametersToFalse()
    {
      var tagHelper = TestHelper.CreateTagHelper<LocalizeNoParametersTagHelper>(null);
      var tagContext = TestHelper.CreateTagContext();

      tagHelper.Init(tagContext);

      Assert.That(tagContext.Items, !Contains.Key(typeof(GenericLocalizeTagHelper)));
    }

    [TestCaseSource(nameof(LocalizeTestData))]
    public async Task<string> ProcessAsync_CanLocalizeText(string tagName, string text, string expectedText, bool trim, bool isHtml)
    {
      var textToLocalize = trim ? text.Trim() : text;
      var localizer = TestHelper.CreateLocalizerMock(false);
      SetupLocalizer(localizer, textToLocalize, expectedText, isHtml);
      var factory = TestHelper.CreateFactoryMock(localizer.Object);
      var helper = CreateTagHelper(factory.Object);
      helper.TrimWhitespace = trim;
      helper.IsHtml = isHtml;

      var output = await TestHelper.GenerateHtmlAsync(helper, tagName, text);

      if (isHtml)
      {
        localizer.Verify(x => x[textToLocalize], Times.Once);
      }
      else
      {
        localizer.Verify(x => x.GetString(textToLocalize), Times.Once);
      }

      return output;
    }

    [Test]
    public async Task ProcessAsync_RemovesTagName()
    {
      var tagHelper = CreateTagHelper();
      var expected = "This should be the only content";

      var output = await TestHelper.GenerateHtmlAsync(tagHelper, "localize", expected);

      Assert.That(output, Is.EqualTo(expected));
    }

    protected LocalizeTagHelper CreateTagHelper()
    {
      return TestHelper.CreateTagHelper<LocalizeTagHelper>(null);
    }

    protected LocalizeTagHelper CreateTagHelper(IHtmlLocalizerFactory factory)
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

    private class LocalizeNoParametersTagHelper : LocalizeTagHelper
    {
      public LocalizeNoParametersTagHelper(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
        : base(localizerFactory, hostingEnvironment)
      {
      }

      protected override bool SupportsParameters => false;
    }
  }
}
