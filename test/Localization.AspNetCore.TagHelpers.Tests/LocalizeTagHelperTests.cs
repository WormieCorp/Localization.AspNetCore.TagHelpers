using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;

namespace Localization.AspNetCore.TagHelpers.Tests
{
	[TestFixture]
	public class LocalizeTagHelperTests
	{
		private readonly ViewContext DefaultViewContext = new ViewContext();
		private Mock<IHtmlLocalizer> _locMock;
		private Mock<IHtmlLocalizerFactory> _locMockFactory;

		#region Setup/Teardown

		[SetUp]
		public void Reset()
		{
			_locMock.Reset();
			_locMock.Setup(x => x.GetString(It.IsAny<string>())).Returns<string>(s => new LocalizedString(s, s, true));
			_locMock.Setup(x => x[It.IsAny<string>()]).Returns<string>(s => new LocalizedHtmlString(s, s, true));

			_locMockFactory.Reset();
			_locMockFactory.Setup(x => x.Create(It.IsAny<Type>())).Returns(_locMock.Object);
			_locMockFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(_locMock.Object);
		}

		[OneTimeSetUp]
		public void Setup()
		{
			_locMock = new Mock<IHtmlLocalizer>();
			_locMockFactory = new Mock<IHtmlLocalizerFactory>();

			var view = new Mock<IView>();
			view.Setup(x => x.Path).Returns("some/value.cshtml");
			DefaultViewContext.View = view.Object;
		}

		#endregion Setup/Teardown

		#region Constructor

		[Test]
		public void Constructor_ThrowsArgumentNullExceptionIfHostingEnvironmentIsNull()
		{
			Assert.That(() => new LocalizeTagHelper(_locMockFactory.Object, null), Throws.ArgumentNullException);
		}

		[Test]
		public void Constructor_ThrowsArgumentNullExceptionIfPassedIViewLocalizerIsNull()
		{
			Assert.That(() => new LocalizeTagHelper(null, new Mock<IHostingEnvironment>().Object), Throws.ArgumentNullException);
		}

		#endregion Constructor

		#region Init

		[TestCase("TestApplication", "Views/Home/Index.cshtml", "Views/Home/Index.cshtml", "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication", "/Views/Home/Index.cshtml", "/Views/Home/Index.cshtml", "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication", "\\Views\\Home\\Index.cshtml", "\\Views\\Home\\Index.cshtml", "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication.Web", "Views/Home/Index.cshtml", "Views/Home/Index.cshtml", "TestApplication.Web.Views.Home.Index")]
		[TestCase("TestApplication", "Views/Home/Index.cshtml", "Views/Shared/_Layout.cshtml", "TestApplication.Views.Shared._Layout")]
		[TestCase("TestApplication", "Views/Home/Index.cshtml", "Views/Shared/_MyPartial.cshtml", "TestApplication.Views.Shared._MyPartial")]
		[TestCase("TestApplication", "Views/Home/Index.cshtml", "Views/Home/_HomePartial.cshtml", "TestApplication.Views.Home._HomePartial")]
		[TestCase("TestApplication", "Views/Home/Index.cshtml", null, "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication", "Views/Home/Index.txt", null, "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication", "Views/Home/Index.cshtml", "", "TestApplication.Views.Home.Index")]
		[TestCase("TestApplication", "Views/Home/Index.txt", "", "TestApplication.Views.Home.Index")]
		public void Init_SetupsCorrectHtmlLocalizer(string appName, string viewPath, string executingPath, string expectedBaseName)
		{
			var hostingEnvironment = new Mock<IHostingEnvironment>();
			hostingEnvironment.Setup(a => a.ApplicationName).Returns(appName);
			var view = new Mock<IView>();
			view.Setup(v => v.Path).Returns(viewPath);
			var viewContext = new ViewContext();
			viewContext.ExecutingFilePath = executingPath;
			viewContext.View = view.Object;
			var tagHelper = new LocalizeTagHelper(_locMockFactory.Object, hostingEnvironment.Object);
			tagHelper.ViewContext = viewContext;
			var context = CreateTagContext();

			tagHelper.Init(context);

			_locMockFactory.Verify(x => x.Create(It.Is<string>(baseName => baseName == expectedBaseName),
				It.Is<string>(location => location == appName)), Times.Once());
		}

		#endregion Init

		#region ProcessAsync

		[Test]
		public async Task ProcessAsync_CanLocalizeAllTextInContent()
		{
			var tagHelper = InitTagHelper();
			var tagContext = CreateTagContext();
			tagHelper.Init(tagContext);
			var tagOutput = CreateTagOutput("localize", "Localize Me");
			var expected = "Jeg er oversatt!";
			_locMock.Setup(x => x.GetString("Localize Me")).Returns(new LocalizedString("Localize Me", expected, false));

			await tagHelper.ProcessAsync(tagContext, tagOutput);

			var actual = tagOutput.Content.GetContent();

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task ProcessAsync_DoesNotRemoveTagNameIfAspLocalizeAttributeIsUsed()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext(new TagHelperAttribute("asp-localize"));
			tagHelper.Init(context);
			var output = CreateTagOutput("span", "Localize Me");
			var expected = "<span>Localize Me</span>";

			string actual = await CreateHtmlOutput(tagHelper, context, output);

			Assert.That(output.TagName, Is.EqualTo("span"));
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task ProcessAsync_GetsLocalizedHtmlIfIsHtmlIsSetToTrue()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			tagHelper.Init(context);
			var output = CreateTagOutput("localize", "<p>Hello</p>");
			tagHelper.IsHtml = true;

			string actual = await CreateHtmlOutput(tagHelper, context, output);

			Assert.That(actual, Is.EqualTo("<p>Hello</p>"));

			_locMock.Verify(x => x["<p>Hello</p>"], Times.Once());
		}

		[Test]
		public async Task ProcessAsync_GetsLocalizedStringOnlyIfIsHtmlIsSetToFalse()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			tagHelper.Init(context);
			var output = CreateTagOutput("localize", "<p>Hello</p>");
			tagHelper.IsHtml = false;

			string actual = await CreateHtmlOutput(tagHelper, context, output);

			Assert.That(actual, Is.EqualTo("&lt;p&gt;Hello&lt;/p&gt;"));

			_locMock.Verify(x => x.GetString("<p>Hello</p>"), Times.Once());
		}

		[Test]
		public async Task ProcessAsync_NeverCallsLocalizerIfContentIsEmpty()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("localize", string.Empty);

			await tagHelper.ProcessAsync(context, output);

			_locMock.Verify(x => x.GetString(It.IsAny<string>()), Times.Never());
			_locMock.Verify(x => x[It.IsAny<string>()], Times.Never());
		}

		[Test]
		public async Task ProcessAsync_RemovesAspLocalizeAttributeIfIsSetInOutput()
		{
			var tagHelper = InitTagHelper();
			var tagContext = CreateTagContext(new TagHelperAttribute("asp-localize"));
			tagHelper.Init(tagContext);
			var tagOutput = CreateTagOutput("span", "Localize Me", new TagHelperAttribute("asp-localize"));

			await tagHelper.ProcessAsync(tagContext, tagOutput);

			Assert.That(tagOutput.Attributes.Any(x => x.Name == "asp-localize"), Is.False, "asp-localize attribute is not removed");
		}

		[Test]
		public async Task ProcessAsync_RemovesLocalizeTagName()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			tagHelper.Init(context);
			var output = CreateTagOutput("localize", "Localize Me");
			var expected = "Localize Me";

			string actual = await CreateHtmlOutput(tagHelper, context, output);

			Assert.That(output.TagName, Is.Null.Or.Empty);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task ProcessAsync_RemovesTagNameEvenIfContentIsEmpty()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("localize", string.Empty);

			await tagHelper.ProcessAsync(context, output);

			Assert.That(output.TagName, Is.Null.Or.Empty);
		}

		#endregion ProcessAsync

		private async Task<string> CreateHtmlOutput(LocalizeTagHelper tagHelper, TagHelperContext tagContext, TagHelperOutput tagOutput)
		{
			var sb = new StringBuilder();

			await tagHelper.ProcessAsync(tagContext, tagOutput);

			using (var writer = new StringWriter(sb))
			{
				tagOutput.WriteTo(writer, HtmlEncoder.Default);
			}

			return sb.ToString();
		}

		private TagHelperContext CreateTagContext(params TagHelperAttribute[] attributes)
		{
			return new TagHelperContext(new TagHelperAttributeList(attributes),
				new Dictionary<object, object>(),
				Guid.NewGuid().ToString());
		}

		private TagHelperOutput CreateTagOutput(string name, string content, params TagHelperAttribute[] attributes)
		{
			return new TagHelperOutput(name, new TagHelperAttributeList(attributes),
				(useCachedResult, encoder) =>
				{
					var tagHelperContent = new DefaultTagHelperContent();
					tagHelperContent.SetContent(content);
					return Task.FromResult<TagHelperContent>(tagHelperContent);
				});
		}

		private LocalizeTagHelper InitTagHelper()
		{
			var hostEnv = new Mock<IHostingEnvironment>();
			hostEnv.Setup(x => x.ApplicationName).Returns("TestApp");

			return new LocalizeTagHelper(_locMockFactory.Object, hostEnv.Object)
			{
				ViewContext = DefaultViewContext
			};
		}
	}
}
