using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers.Tests
{
	public interface IViewCombined : IViewLocalizer, IViewContextAware
	{ }

	public interface IViewNotContextAware : IViewLocalizer
	{
		void Contextualize(ViewContext context);
	}

	public class LocalizeAttributeTagHelperTests
	{
		private Mock<IViewLocalizer> _locMock;

		#region Setup/Teardown

		[SetUp]
		public void Reset()
		{
			_locMock.Reset();
			_locMock.Setup(x => x.GetString(It.IsAny<string>())).Returns<string>(s => new LocalizedString(s, s, true));
			_locMock.Setup(x => x[It.IsAny<string>()]).Returns<string>(s => new LocalizedHtmlString(s, s, true));
		}

		[OneTimeSetUp]
		public void Setup()
		{
			_locMock = new Mock<IViewLocalizer>();
		}

		#endregion Setup/Teardown

		#region Constructor

		[Test]
		public void Constructor_ThrowsArgumentNullExceptionIfPassedIViewLocalizerIsNull()
		{
			Assert.That(() => new LocalizeAttributeTagHelper(null), Throws.ArgumentNullException.And.Message.Contains("localizer"));
		}

		#endregion Constructor

		#region Init

		[Test]
		public void Init_CallsContextualizeIfIViewLocalizeIsAlsoOfTypeIViewContextAware()
		{
			var mock = new Mock<IViewCombined>();
			var tagHelper = new LocalizeAttributeTagHelper(mock.Object);
			var viewContext = new ViewContext();
			tagHelper.ViewContext = viewContext;
			var context = CreateTagContext();

			tagHelper.Init(context);

			mock.Verify(x => x.Contextualize(viewContext), Times.Once());
		}

		[Test]
		public void Init_NeverCallsContextualizeIfIViewLocalizeIsNotOfTypeIViewContextAware()
		{
			var mock = new Mock<IViewNotContextAware>();
			var tagHelper = new LocalizeAttributeTagHelper(mock.Object);
			var viewContext = new ViewContext();
			tagHelper.ViewContext = viewContext;
			var context = CreateTagContext();

			tagHelper.Init(context);

			mock.Verify(x => x.Contextualize(viewContext), Times.Never());
		}

		#endregion Init

		#region Process

		[Test]
		public void Process_CanLocalizeMultipleAttributes()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("span", "Oh Yeah");
			tagHelper.AttributeValues = new Dictionary<string, string>
			{
				{"title","Localize Me" },
				{"alt", "Me too" }
			};
			_locMock.Setup(x => x.GetString("Localize Me")).Returns<string>(x => new LocalizedString(x, "I was localized"));
			_locMock.Setup(x => x.GetString("Me too")).Returns<string>(x => new LocalizedString(x, "I was also localized"));
			var expected = "<span title=\"I was localized\" alt=\"I was also localized\">Oh Yeah</span>";

			var actual = CreateHtmlOutput(tagHelper, context, output);

			Assert.That(output.Attributes.ContainsName("title"), Is.True, "Title attribute has not been set");
			Assert.That(output.Attributes.ContainsName("alt"), Is.True, "Alt attribute has not been set");
			Assert.That(actual, Is.EqualTo(expected));

			_locMock.Verify(x => x.GetString(It.IsAny<string>()), Times.Exactly(2));
		}

		[Test]
		public void Process_CanLocalizeSingleAttributeValue()
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("span", "Does not matter");
			tagHelper.AttributeValues.Add("title", "Localize-me");
			_locMock.Setup(x => x.GetString("Localize-me")).Returns<string>(x => new LocalizedString(x, "I Am Localized", false));
			var expected = "<span title=\"I Am Localized\">Does not matter</span>";

			var actual = CreateHtmlOutput(tagHelper, context, output);

			Assert.That(output.Attributes.ContainsName("title"), Is.True, "Title attribute is not set");
			Assert.That(actual, Is.EqualTo(expected));

			_locMock.Verify(x => x.GetString("Localize-me"), Times.Once());
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("           ")]
		public void Process_EmptyAttributesIsIgnored(string value)
		{
			var tagHelper = InitTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("span", "Yup I'm still here");
			tagHelper.AttributeValues.Add("title", value);
			var expected = "<span>Yup I&#x27;m still here</span>";

			var actual = CreateHtmlOutput(tagHelper, context, output);

			Assert.That(output.Attributes.ContainsName("title"), Is.False, "Title attribute has been set");
			Assert.That(actual, Is.EqualTo(expected));

			_locMock.Verify(x => x.GetString(value), Times.Never());
		}

		#endregion Process

		private string CreateHtmlOutput(LocalizeAttributeTagHelper tagHelper, TagHelperContext tagContext, TagHelperOutput tagOutput)
		{
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

		private LocalizeAttributeTagHelper InitTagHelper()
		{
			return new LocalizeAttributeTagHelper(_locMock.Object);
		}
	}
}
