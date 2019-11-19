//-----------------------------------------------------------------------
// <copyright file="LocalizeAttributeTagHelperTests.cs">
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
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.AspNetCore.Mvc.ViewEngines;
  using Microsoft.AspNetCore.Razor.TagHelpers;
  using Microsoft.Extensions.Localization;
  using Moq;
  using Xunit;

  public class LocalizeAttributeTagHelperTests
  {
#if NETCOREAPP3_0
    private Mock<IWebHostEnvironment> hostingMock;
#else
    private Mock<IHostingEnvironment> hostingMock;
#endif
    private Mock<IHtmlLocalizerFactory> locFactoryMock;
    private Mock<IHtmlLocalizer> locMock;

    public LocalizeAttributeTagHelperTests()
    {
      locMock = new Mock<IHtmlLocalizer>();
      hostingMock =
#if NETCOREAPP3_0
  new Mock<IWebHostEnvironment>();
#else
  new Mock<IHostingEnvironment>();
#endif
      hostingMock.Setup(x => x.ApplicationName).Returns("Localization.AspNetCore.TagHelpers.Tests");
      locFactoryMock = new Mock<IHtmlLocalizerFactory>();
      locFactoryMock.Setup(x => x.Create(It.IsAny<string>(), hostingMock.Object.ApplicationName)).Returns(locMock.Object);
      locMock.Reset();
      locMock.Setup(x => x.GetString(It.IsAny<string>())).Returns<string>(s => new LocalizedString(s, s, true));
      locMock.Setup(x => x[It.IsAny<string>()]).Returns<string>(s => new LocalizedHtmlString(s, s, true));
    }

    #region Constructor

    [Fact]
    public void Constructor_ThrowsArgumentNullExceptionIfPassedIViewLocalizerIsNull()
    {
      var hostMock =
#if NETCOREAPP3_0
  new Mock<IWebHostEnvironment>();
#else
  new Mock<IHostingEnvironment>();
#endif
      Assert.Throws<ArgumentNullException>(() => new LocalizeAttributeTagHelper(null, hostMock.Object));
    }

    #endregion Constructor

    #region Init

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
      var hostingEnvironment =
#if NETCOREAPP3_0
  new Mock<IWebHostEnvironment>();
#else
  new Mock<IHostingEnvironment>();
#endif
      hostingEnvironment.Setup(a => a.ApplicationName).Returns(appName);
      var factoryMock = TestHelper.CreateFactoryMock(true);
      var view = new Mock<IView>();
      view.Setup(v => v.Path).Returns(viewPath);
      var viewContext = new ViewContext
      {
        ExecutingFilePath = executionPath,
        View = view.Object
      };
      var tagHelper = new LocalizeAttributeTagHelper(factoryMock.Object, hostingEnvironment.Object)
      {
        ViewContext = viewContext
      };
      var context = TestHelper.CreateTagContext();

      tagHelper.Init(context);

      factoryMock.Verify(x => x.Create(expectedBaseName, appName), Times.Once());
    }

    #endregion Init

    #region Process

    [Fact]
    public void Process_CanLocalizeMultipleAttributes()
    {
      var tagHelper = InitTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("span", "Oh Yeah");
      tagHelper.AttributeValues = new Dictionary<string, string>
      {
        { "title", "Localize Me" },
        { "alt", "Me too" }
      };
      locMock.Setup(x => x.GetString("Localize Me")).Returns<string>(x => new LocalizedString(x, "I was localized"));
      locMock.Setup(x => x.GetString("Me too")).Returns<string>(x => new LocalizedString(x, "I was also localized"));
      var expected = "<span title=\"I was localized\" alt=\"I was also localized\">Oh Yeah</span>";

      var actual = CreateHtmlOutput(tagHelper, context, output);

      Assert.Contains(output.Attributes, (attr) => attr.Name == "title");
      Assert.Contains(output.Attributes, (attr) => attr.Name == "alt");
      Assert.Equal(expected, actual);

      locMock.Verify(x => x.GetString(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void Process_CanLocalizeSingleAttributeValue()
    {
      var tagHelper = InitTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("span", "Does not matter");
      tagHelper.AttributeValues.Add("title", "Localize-me");
      locMock.Setup(x => x.GetString("Localize-me")).Returns<string>(x => new LocalizedString(x, "I Am Localized", false));
      var expected = "<span title=\"I Am Localized\">Does not matter</span>";

      var actual = CreateHtmlOutput(tagHelper, context, output);

      Assert.Contains(output.Attributes, (attr) => attr.Name == "title");
      Assert.Equal(expected, actual);

      locMock.Verify(x => x.GetString("Localize-me"), Times.Once());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("           ")]
    public void Process_EmptyAttributesIsIgnored(string value)
    {
      var tagHelper = InitTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("span", "Yup I'm still here");
      tagHelper.AttributeValues.Add("title", value);
      var expected = "<span>Yup I&#x27;m still here</span>";

      var actual = CreateHtmlOutput(tagHelper, context, output);

      Assert.DoesNotContain(output.Attributes, (attr) => attr.Name == "title");
      Assert.Equal(expected, actual);

      locMock.Verify(x => x.GetString(value), Times.Never());
    }

    [Theory]
    [InlineData("Parameters")]
    [InlineData("")]
    public void Process_ShouldFormatContentWhenParamsWithSingleValue(string paramValue)
    {
      var tagHelper = InitTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("abbr", "IUP");
      tagHelper.AttributeValues.Add("title", "I Use {0}");
      tagHelper.ParameterValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
      {
        { "title", paramValue }
      };
      locMock.Setup(x => x.GetString("I Use {0}", paramValue))
        .Returns<string, string[]>((x,y) => new LocalizedString(x, string.Format(x, y) , true));

      var expected = $"<abbr title=\"I Use {paramValue}\">IUP</abbr>";

      var actual = CreateHtmlOutput(tagHelper, context, output);

      Assert.Equal(expected, actual);

      locMock.Verify(x => x.GetString("I Use {0}", paramValue), Times.Once());
    }

    [Fact]
    public void Process_ShouldFormatContentWhenParamsWithSemiColorDelimitedValues()
    {
      var tagHelper = InitTagHelper();
      var context = CreateTagContext();
      var output = CreateTagOutput("abbr", "IUMP");
      tagHelper.AttributeValues.Add("title", "I Use {0} {1}");
      tagHelper.ParameterValues.Add("title", "Multiple;Parameters");
      locMock.Setup(x => x.GetString("I Use {0} {1}", "Multiple", "Parameters"))
        .Returns<string, string[]>((x, y) => new LocalizedString(x, string.Format(x, y), true));

      var expected = "<abbr title=\"I Use Multiple Parameters\">IUMP</abbr>";

      var actual = CreateHtmlOutput(tagHelper, context, output);

      Assert.Equal(expected, actual);

      locMock.Verify(x => x.GetString("I Use {0} {1}", "Multiple", "Parameters"), Times.Once());
    }

    #endregion Process

    private string CreateHtmlOutput(LocalizeAttributeTagHelper tagHelper, TagHelperContext tagContext, TagHelperOutput tagOutput)
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

    private TagHelperContext CreateTagContext(params TagHelperAttribute[] attributes)
    {
      return new TagHelperContext(
        new TagHelperAttributeList(attributes),
        new Dictionary<object, object>(),
        Guid.NewGuid().ToString());
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

    private LocalizeAttributeTagHelper InitTagHelper()
    {
      return new LocalizeAttributeTagHelper(locFactoryMock.Object, hostingMock.Object)
      {
        ViewContext = new ViewContext()
        {
          ExecutingFilePath = "/Views/Home/Index.cshtml"
        }
      };
    }
  }
}
