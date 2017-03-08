//-----------------------------------------------------------------------
// <copyright file="GenericLocalizeTagHelperTests.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers.Tests
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Text.Encodings.Web;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.AspNetCore.Mvc.ViewEngines;
  using Microsoft.Extensions.Localization;
  using Microsoft.Extensions.Options;
  using Moq;
  using Xunit;

  public class GenericLocalizeTagHelperTests
  {
    public static IEnumerable<object[]> LocalizeNewLinesTestData
    {
      get
      {
        var text = "This is\r\nThe\nUnormalized Text\r\n";
        yield return new object[] { text, text.Trim(), NewLineHandling.None, true, false };
        var expectedText = text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        yield return new object[] { text, expectedText.Trim(), NewLineHandling.Auto, true, false };
        expectedText = text.Replace("\n", "\r\n").Replace("\r\r", "\r");
        yield return new object[] { text, expectedText.Trim(), NewLineHandling.Windows, true, false };
        expectedText = text.Replace("\r", "");
        yield return new object[] { text, expectedText.Trim(), NewLineHandling.Unix, true, false };

        text = "  This\r\n   Will\r\nAlways\r\n    Trimmed \r\nDown   ";
        expectedText = "This\r\nWill\r\nAlways\r\nTrimmed\r\nDown";
        yield return new object[] { text, expectedText.Trim(), NewLineHandling.Windows, true, false };

        yield return new object[]{"This will transform\nNew Lines   \r\n   But not trim\n  whitespace   ",
          "This will transform\nNew Lines   \n   But not trim\n  whitespace   ", NewLineHandling.Unix, false, false };

        yield return new object[]{"\r\n   \r\n   This should trim everything before and after \r\n \n \r\n",
          "This should trim everything before and after", NewLineHandling.Auto, false, true };
      }
    }

    public static IEnumerable LocalizeTestData
    {
      get
      {
        var encoder = HtmlEncoder.Default;

        yield return new object[] { "p", "This will be localized", "This is the localized text", true, false, "<p>This is the localized text</p>" };
        var text = "This the the <small>localized</small> text with <strong>html</strong>";
        yield return new object[] { "p", "This wi be localized", text, true, false, $"<p>{encoder.Encode(text)}</p>" };
        yield return new object[] { "span", "This", text, true, true, $"<span>{text}</span>" };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, false, $"<div>    {encoder.Encode(text)}    </div>" };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, true, $"<div>    {text}    </div>" };
        yield return new object[] { "div", "     Localize    ", $"{text}", true, false, $"<div>{encoder.Encode(text)}</div>" };
        yield return new object[] { "div", "    Localize    ", $"{text}", true, true, $"<div>{text}</div>" };
      }
    }

    public static IEnumerable LocalizeTestDataWithParameters
    {
      get
      {
        var encoder = HtmlEncoder.Default;

        yield return new object[] { "p", "This will be {0}", "This is the {0} text", true, false, new[] { "Localized" }, "<p>This is the Localized text</p>" };
        var text = "This the the <small>{0}</small> {1} with <strong>html</strong>";
        var parameters = new[] { "Localized", "text" };
        yield return new object[] { "p", "This wi be localized", text, true, false, parameters, $"<p>{encoder.Encode(string.Format(text, parameters))}</p>" };
        yield return new object[] { "span", "This", text, true, true, parameters, $"<span>{string.Format(text, parameters)}</span>" };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, false, parameters, $"<div>    {encoder.Encode(string.Format(text, parameters))}    </div>" };
        yield return new object[] { "div", "Localize", $"    {text}    ", false, true, parameters, $"<div>    {string.Format(text, parameters)}    </div>" };
        yield return new object[] { "div", "     Localize    ", $"{text}", true, false, parameters, $"<div>{encoder.Encode(string.Format(text, parameters))}</div>" };
        yield return new object[] { "div", "    Localize    ", $"{text}", true, true, parameters, $"<div>{string.Format(text, parameters)}</div>" };
      }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionOnHostingEnvironmentIsNull()
    {
      Assert.Throws<ArgumentNullException>(() => new GenericLocalizeTagHelper(TestHelper.CreateFactoryMock(false).Object, null, null));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionOnHtmlLocalizerFactoryIsNull()
    {
      Assert.Throws<ArgumentNullException>(() => new GenericLocalizeTagHelper(null, new Mock<IHostingEnvironment>().Object, null));
    }

    [Fact]
    public void Init_AddsNewParameterListToExistingStack()
    {
      var tagHelper = CreateTagHelper();
      var tagContext = TestHelper.CreateTagContext();

      tagHelper.Init(tagContext);
      tagHelper.Init(tagContext);

      Assert.Contains(tagContext.Items, (contextItems) => contextItems.Key == typeof(GenericLocalizeTagHelper));
      var item = tagContext.Items[typeof(GenericLocalizeTagHelper)];
      Assert.NotNull(item);
      Assert.IsType<Stack<List<object>>>(item);
      Assert.Equal(2, ((Stack<List<object>>)item).Count);
    }

    [Fact]
    public void Init_CreatesANewParameterStack()
    {
      var tagHelper = CreateTagHelper();
      var tagContext = TestHelper.CreateTagContext();

      tagHelper.Init(tagContext);

      Assert.Contains(tagContext.Items, (contextItem) => contextItem.Key == typeof(GenericLocalizeTagHelper));
      var item = tagContext.Items[typeof(GenericLocalizeTagHelper)];
      Assert.NotNull(item);
      Assert.IsType<Stack<List<object>>>(item);
      Assert.Equal(1, ((Stack<List<object>>)item).Count);
    }

    [Theory]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", "Views/Home/Index.cshtml", "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication", "/Views/Home/Index.cshtml", "/Views/Home/Index.cshtml", "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication", "\\Views\\Home\\Index.cshtml", "\\Views\\Home\\Index.cshtml", "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication.Web", "Views/Home/Index.cshtml", "Views/Home/Index.cshtml", "TestApplication.Web.Views.Home.Index")]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", "Views/Shared/_Layout.cshtml", "TestApplication.Views.Shared._Layout")]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", "Views/Shared/_MyPartial.cshtml", "TestApplication.Views.Shared._MyPartial")]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", "Views/Home/_HomePartial.cshtml", "TestApplication.Views.Home._HomePartial")]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", null, "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication", "Views/Home/Index.txt", null, "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication", "Views/Home/Index.cshtml", "", "TestApplication.Views.Home.Index")]
    [InlineData("TestApplication", "Views/Home/Index.txt", "", "TestApplication.Views.Home.Index")]
    public void Init_CreatesHtmlLocalizerFromViewContext(string appName, string viewPath, string executionPath, string expectedBaseName)
    {
      var hostingEnvironment = new Mock<IHostingEnvironment>();
      hostingEnvironment.Setup(a => a.ApplicationName).Returns(appName);
      var factoryMock = TestHelper.CreateFactoryMock(true);
      var view = new Mock<IView>();
      view.Setup(v => v.Path).Returns(viewPath);
      var viewContext = new ViewContext();
      viewContext.ExecutingFilePath = executionPath;
      viewContext.View = view.Object;
      var tagHelper = new GenericLocalizeTagHelper(factoryMock.Object, hostingEnvironment.Object, null);
      tagHelper.ViewContext = viewContext;
      var context = TestHelper.CreateTagContext();

      tagHelper.Init(context);

      factoryMock.Verify(x => x.Create(expectedBaseName, appName), Times.Once());
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
      var tagHelper = TestHelper.CreateTagHelper<NoParametersSupported>(null);
      var tagContext = TestHelper.CreateTagContext();

      tagHelper.Init(tagContext);

      Assert.DoesNotContain(tagContext.Items, (item) => item.Key == typeof(GenericLocalizeTagHelper));
    }

    [Theory]
    [MemberData(nameof(LocalizeNewLinesTestData))]
    public async Task ProcessAsync_CanHandleNewLineNormalization(string text, string expectedText, NewLineHandling handling, bool trimEachLine, bool trimWhitespace)
    {
      var localizer = TestHelper.CreateLocalizerMock(false);
      SetupLocalizer(localizer, expectedText, expectedText, false);
      var factory = TestHelper.CreateFactoryMock(localizer.Object);
      var helper = CreateTagHelper(factory.Object);
      helper.TrimWhitespace = trimWhitespace;
      helper.IsHtml = false;
      helper.NewLineHandling = handling;
      helper.TrimEachLine = trimEachLine;

      await TestHelper.GenerateHtmlAsync(helper, "span", text);

      localizer.Verify(x => x.GetString(expectedText), Times.Once);
    }

    [Theory]
    [MemberData(nameof(LocalizeTestData))]
    public async Task ProcessAsync_CanLocalizeText(string tagName, string text, string expectedText, bool trim, bool isHtml, string expected)
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

      Assert.Equal(expected, output);
    }

    [Theory]
    [MemberData(nameof(LocalizeTestDataWithParameters))]
    public async Task ProcessAsync_CanLocalizeTextWithParameters(string tagName, string text, string expectedText, bool trim, bool isHtml, object[] parameters, string expected)
    {
      var textToLocalize = trim ? text.Trim() : text;
      var localizer = TestHelper.CreateLocalizerMock(false);
      SetupLocalizerWithParameters(localizer, textToLocalize, expectedText, isHtml);
      var factory = TestHelper.CreateFactoryMock(localizer.Object);
      var helper = CreateTagHelper(factory.Object);
      helper.TrimWhitespace = trim;
      helper.IsHtml = isHtml;
      var context = TestHelper.CreateTagContext();
      helper.Init(context);
      var stack = (Stack<List<object>>)context.Items[typeof(GenericLocalizeTagHelper)];
      var list = stack.Peek();
      foreach (var parameter in parameters)
      {
        list.Add(parameter);
      }

      var output = TestHelper.CreateTagOutput(tagName, text);

      var htmlOutput = await TestHelper.GenerateHtmlAsync(helper, context, output);

      if (isHtml)
      {
        localizer.Verify(x => x[textToLocalize, parameters], Times.Once);
      }
      else
      {
        localizer.Verify(x => x.GetString(textToLocalize, parameters), Times.Once);
      }

      Assert.Equal(expected, htmlOutput);
    }

    protected GenericLocalizeTagHelper CreateTagHelper()
      => CreateTagHelper(null);

    protected GenericLocalizeTagHelper CreateTagHelper(IHtmlLocalizerFactory factory)
      => TestHelper.CreateTagHelper<GenericLocalizeTagHelper>(factory);

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

    private void SetupLocalizerWithParameters(Mock<IHtmlLocalizer> localizer, string textToLocalize, string expectedText, bool isHtml)
    {
      if (isHtml)
      {
        localizer.Setup(x => x[textToLocalize, It.IsAny<object[]>()]).Returns<string, object[]>((s, o) => new LocalizedHtmlString(s, string.Format(expectedText, o), s == expectedText));
      }
      else
      {
        localizer.Setup(x => x.GetString(textToLocalize, It.IsAny<object[]>())).Returns<string, object[]>((s, o) => new LocalizedString(s, string.Format(expectedText, o), s == expectedText));
      }
    }

    private class NoParametersSupported : GenericLocalizeTagHelper
    {
      public NoParametersSupported(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment, IOptions<LocalizeTagHelperOptions> options)
        : base(localizerFactory, hostingEnvironment, options)
      {
      }

      protected override bool SupportsParameters => false;
    }
  }
}
