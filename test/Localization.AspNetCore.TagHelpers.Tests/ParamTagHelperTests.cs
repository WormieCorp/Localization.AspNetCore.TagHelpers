using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Localization.AspNetCore.TagHelpers.Tests
{
	[Category("Parameter")]
	public class ParamTagHelperTests
	{
		[Test]
		public void ParameterOrderIsHigherThanLocalizeTagHelperOrder()
		{
			var tagHelper1 = TestHelper.CreateTagHelper<AspLocalizeTagHelper>(null);
			var tagHelper2 = TestHelper.CreateTagHelper<LocalizeTagHelper>(null);
			var paramTagHelper = CreateTagHelper();

			Assert.That(paramTagHelper.Order, Is.GreaterThan(tagHelper1.Order));
			Assert.That(paramTagHelper.Order, Is.GreaterThan(tagHelper2.Order));
		}

		[Test]
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
			var stack = (Stack<List<object>>)context.Items[typeof(AspLocalizeTagHelper)];
			var parameters = stack.Peek();

			await helper.ProcessAsync(context, output);

			Assert.That(parameters, Is.EqualTo(expected));
		}

		[Test]
		public async Task ProcessAsync_DoesNothingIfNoStackIsAvailable()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			context.Items.Remove(typeof(AspLocalizeTagHelper));
			var output = CreateTagOutput("parameter", "My Localized String");

			var html = await CreateHtmlOutput(helper, context, output);

			Assert.That(context.Items, Is.Empty);
			Assert.That(html, Is.EqualTo("<parameter>My Localized String</parameter>"));
		}

		[Test]
		public async Task ProcessAsync_RemovesAllContent()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("parameter", "My loc");

			await helper.ProcessAsync(context, output);

			Assert.That(output.Content.IsEmptyOrWhiteSpace, Is.True);
		}

		[Test]
		public async Task ProcessAsync_RemovesTagName()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("parameter", "my Lc");

			await helper.ProcessAsync(context, output);

			Assert.That(output.TagName, Is.Null);
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
			dictionary.Add(typeof(AspLocalizeTagHelper), stack);

			return new TagHelperContext(new TagHelperAttributeList(),
				dictionary,
				Guid.NewGuid().ToString());
		}

		private ParameterTagHelper CreateTagHelper()
		{
			return new ParameterTagHelper();
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
	}
}
