using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;

namespace Localization.AspNetCore.TagHelpers.Tests
{
    [TestFixture]
	public class ParamTagHelperTests
	{
		[Test]
		public async Task ProcessAsync_AddLocalizedTextToTagContextItems()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("param", "My Localized String");

			await helper.ProcessAsync(context, output);

			Assert.That(context.Items, Contains.Key("LocalizeParameters"));
			Assert.That(context.Items, Contains.Value(new[] { "My Localized String" }));
		}

		[Test]
		public async Task ProcessAsync_RemovesAllContent()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("param", "My loc");

			await helper.ProcessAsync(context, output);

			Assert.That(output.Content.IsEmptyOrWhiteSpace, Is.True);
		}

		[Test]
		public async Task ProcessAsync_RemovesTagName()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext();
			var output = CreateTagOutput("param", "my Lc");

			await helper.ProcessAsync(context, output);

			Assert.That(output.TagName, Is.Null);
		}

		[Test]
		public async Task ProcessAsync_AppendsLocalizedTextToExistingList()
		{
			var helper = CreateTagHelper();
			var context = CreateTagContext("First Localized", "Second Localized");
			var expected = new List<string>
			{
				"First Localized",
				"Second Localized",
				"Third Localized"
			};
			var output = CreateTagOutput("param", "Third Localized");

			await helper.ProcessAsync(context, output);

			Assert.That(context.Items, Contains.Value(expected));
		}

		private ParamTagHelper CreateTagHelper()
		{
			return new ParamTagHelper();
		}

		private TagHelperContext CreateTagContext(params string[] localizedParameters)
		{
			var dictionary = new Dictionary<object, object>();
			if (localizedParameters.Any())
			{
				dictionary.Add("LocalizeParameters", new List<string>(localizedParameters));
			}

			return new TagHelperContext(new TagHelperAttributeList(),
				dictionary,
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
	}
}
