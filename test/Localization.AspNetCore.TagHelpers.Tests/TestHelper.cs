using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers.Tests
{
	public static class TestHelper
	{
		public static readonly string ApplicationName = typeof(TestHelper).GetTypeInfo().Assembly.GetName().Name;

		public static readonly ViewContext DefaultViewContext = new ViewContext();

		static TestHelper()
		{
			var view = new Mock<IView>();
			view.SetupGet(x => x.Path).Returns("some/value.cshtml");
			DefaultViewContext.View = view.Object;
		}

		public static Mock<IHtmlLocalizerFactory> CreateFactoryMock(bool setup)
		{
			return CreateFactoryMock(CreateLocalizerMock(setup).Object);
		}

		public static Mock<IHtmlLocalizerFactory> CreateFactoryMock(IHtmlLocalizer localizer)
		{
			var mock = new Mock<IHtmlLocalizerFactory>();
			mock.Setup(x => x.Create(It.IsAny<Type>())).Returns(localizer);
			mock.Setup(x => x.Create(It.IsAny<string>(), ApplicationName)).Returns(localizer);
			return mock;
		}

		public static Mock<IHtmlLocalizer> CreateLocalizerMock(bool setup)
		{
			var mock = new Mock<IHtmlLocalizer>();

			if (setup)
			{
				mock.Setup(x => x.GetString(It.IsAny<string>())).Returns<string>(s => new LocalizedString(s, s, true));
				mock.Setup(x => x[It.IsAny<string>()]).Returns<string>(s => new LocalizedHtmlString(s, s, true));
				mock.Setup(x => x.GetString(It.IsAny<string>(), It.IsAny<object[]>()))
					.Returns<string, object[]>((s, o) => new LocalizedString(s, string.Format(s, o), true));
				mock.Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
					.Returns<string, object[]>((s, o) => new LocalizedHtmlString(s, string.Format(s, o), true));
			}

			return mock;
		}

		public static TagHelperContext CreateTagContext(params TagHelperAttribute[] attributes)
		{
			return new TagHelperContext(new TagHelperAttributeList(attributes),
				new Dictionary<object, object>(),
				Guid.NewGuid().ToString());
		}

		public static T CreateTagHelper<T>(IHtmlLocalizerFactory factory)
			where T : AspLocalizeTagHelper
		{
			var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
			hostingEnvironmentMock.SetupGet(x => x.ApplicationName).Returns(ApplicationName);
			if (factory == null)
				factory = CreateFactoryMock(true).Object;

			var instance = (T)Activator.CreateInstance(typeof(T), factory, hostingEnvironmentMock.Object);
			instance.ViewContext = DefaultViewContext;

			return instance;
		}

		public static TagHelperOutput CreateTagOutput(string tagName, string content, params TagHelperAttribute[] attributes)
		{
			return new TagHelperOutput(tagName, new TagHelperAttributeList(attributes),
				(useCachedResult, encoder) =>
				{
					var tagHelperContent = new DefaultTagHelperContent();
					tagHelperContent.SetContent(content);
					return Task.FromResult<TagHelperContent>(tagHelperContent);
				});
		}

		public static async Task<string> GenerateHtmlAsync(TagHelper helper, TagHelperContext context, TagHelperOutput output)
		{
			var sb = new StringBuilder();

			await helper.ProcessAsync(context, output);

			using (var writer = new StringWriter(sb))
			{
				output.WriteTo(writer, HtmlEncoder.Default);
			}

			return sb.ToString();
		}

		public static Task<string> GenerateHtmlAsync(TagHelper helper, string tagName, string content)
		{
			return GenerateHtmlAsync(helper, tagName, content, new TagHelperAttribute[0]);
		}

		public static Task<string> GenerateHtmlAsync(TagHelper helper, string tagName, string content, params TagHelperAttribute[] attributes)
		{
			var tagContext = CreateTagContext();
			var tagOutput = CreateTagOutput(tagName, content, attributes);

			helper.Init(tagContext);

			return GenerateHtmlAsync(helper, tagContext, tagOutput);
		}

		public static Task<string> GenerateHtmlAsync(TagHelper helper, string tagName, string content, params object[] parameters)
		{
			var tagContext = CreateTagContext();
			var tagOutput = CreateTagOutput(tagName, content);

			helper.Init(tagContext);

			var stack = (Stack<List<object>>)tagContext.Items[typeof(AspLocalizeTagHelper)];
			var list = stack.Peek();
			list.AddRange(parameters);

			return GenerateHtmlAsync(helper, tagContext, tagOutput);
		}
	}
}
