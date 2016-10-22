using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers.Tests
{
	public class AspLocalizeTagHelperTests
	{
		public static IEnumerable LocalizeTestData
		{
			get
			{
				var encoder = HtmlEncoder.Default;

				yield return new TestCaseData("p", "This will be localized", "This is the localized text", true, false)
					.Returns("<p>This is the localized text</p>");
				var text = "This the the <small>localized</small> text with <strong>html</strong>";
				yield return new TestCaseData("p", "This wi be localized", text, true, false)
					.Returns($"<p>{encoder.Encode(text)}</p>");
				yield return new TestCaseData("span", "This", text, true, true)
					.Returns($"<span>{text}</span>");
				yield return new TestCaseData("div", "Localize", $"    {text}    ", false, false)
					.Returns($"<div>    {encoder.Encode(text)}    </div>");
				yield return new TestCaseData("div", "Localize", $"    {text}    ", false, true)
					.Returns($"<div>    {text}    </div>");
				yield return new TestCaseData("div", "     Localize    ", $"{text}", true, false)
					.Returns($"<div>{encoder.Encode(text)}</div>");
				yield return new TestCaseData("div", "    Localize    ", $"{text}", true, true)
					.Returns($"<div>{text}</div>");
			}
		}

		public static IEnumerable LocalizeTestDataWithParameters
		{
			get
			{
				var encoder = HtmlEncoder.Default;

				yield return new TestCaseData("p", "This will be {0}", "This is the {0} text", true, false, new[] { "Localized" })
					.Returns("<p>This is the Localized text</p>");
				var text = "This the the <small>{0}</small> {1} with <strong>html</strong>";
				var parameters = new[] { "Localized", "text" };
				yield return new TestCaseData("p", "This wi be localized", text, true, false,
					parameters)
					.Returns($"<p>{encoder.Encode(string.Format(text, parameters))}</p>");
				yield return new TestCaseData("span", "This", text, true, true,
					parameters)
					.Returns($"<span>{string.Format(text, parameters)}</span>");
				yield return new TestCaseData("div", "Localize", $"    {text}    ", false, false,
					parameters)
					.Returns($"<div>    {encoder.Encode(string.Format(text, parameters))}    </div>");
				yield return new TestCaseData("div", "Localize", $"    {text}    ", false, true,
					parameters)
					.Returns($"<div>    {string.Format(text, parameters)}    </div>");
				yield return new TestCaseData("div", "     Localize    ", $"{text}", true, false,
					parameters)
					.Returns($"<div>{encoder.Encode(string.Format(text, parameters))}</div>");
				yield return new TestCaseData("div", "    Localize    ", $"{text}", true, true,
					parameters)
					.Returns($"<div>{string.Format(text, parameters)}</div>");
			}
		}

		[Test]
		public void Constructor_ThrowsArgumentNullExceptionOnHostingEnvironmentIsNull()
		{
			Assert.That(() => new GenericLocalizeTagHelper(TestHelper.CreateFactoryMock(false).Object, null), Throws.ArgumentNullException);
		}

		[Test]
		public void Constructor_ThrowsArgumentNullExceptionOnHtmlLocalizerFactoryIsNull()
		{
			Assert.That(() => new GenericLocalizeTagHelper(null, new Mock<IHostingEnvironment>().Object), Throws.ArgumentNullException);
		}

		[Test]
		public void Init_AddsNewParameterListToExistingStack()
		{
			var tagHelper = CreateTagHelper();
			var tagContext = TestHelper.CreateTagContext();

			tagHelper.Init(tagContext);
			tagHelper.Init(tagContext);

			Assert.That(tagContext.Items, Contains.Key(typeof(GenericLocalizeTagHelper)));
			var item = tagContext.Items[typeof(GenericLocalizeTagHelper)];
			Assert.That(item, Is.Not.Null.And.TypeOf<Stack<List<object>>>().And.Count.EqualTo(2));
		}

		[Test]
		public void Init_CreatesANewParameterStack()
		{
			var tagHelper = CreateTagHelper();
			var tagContext = TestHelper.CreateTagContext();

			tagHelper.Init(tagContext);

			Assert.That(tagContext.Items, Contains.Key(typeof(GenericLocalizeTagHelper)));
			var item = tagContext.Items[typeof(GenericLocalizeTagHelper)];
			Assert.That(item, Is.Not.Null.And.TypeOf<Stack<List<object>>>().And.Count.EqualTo(1));
		}

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
			var tagHelper = new GenericLocalizeTagHelper(factoryMock.Object, hostingEnvironment.Object);
			tagHelper.ViewContext = viewContext;
			var context = TestHelper.CreateTagContext();

			tagHelper.Init(context);

			factoryMock.Verify(x => x.Create(expectedBaseName, appName), Times.Once());
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
			var tagHelper = TestHelper.CreateTagHelper<NoParametersSupported>(null);
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
				localizer.Verify(x => x[textToLocalize], Times.Once);
			else
				localizer.Verify(x => x.GetString(textToLocalize), Times.Once);

			return output;
		}

		[TestCaseSource(nameof(LocalizeTestDataWithParameters))]
		public async Task<string> ProcessAsync_CanLocalizeTextWithParameters(string tagName, string text, string expectedText, bool trim, bool isHtml, object[] parameters)
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
				localizer.Verify(x => x[textToLocalize, parameters], Times.Once);
			else
				localizer.Verify(x => x.GetString(textToLocalize, parameters), Times.Once);

			return htmlOutput;
		}

		protected GenericLocalizeTagHelper CreateTagHelper()
		{
			return TestHelper.CreateTagHelper<GenericLocalizeTagHelper>(null);
		}

		protected GenericLocalizeTagHelper CreateTagHelper(IHtmlLocalizerFactory factory)
		{
			return TestHelper.CreateTagHelper<GenericLocalizeTagHelper>(factory);
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
#pragma warning disable S1144 // Unused private types or members should be removed

			public NoParametersSupported(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
				: base(localizerFactory, hostingEnvironment)
			{
			}

#pragma warning restore S1144 // Unused private types or members should be removed

			protected override bool SupportsParameters => false;
		}
	}
}
