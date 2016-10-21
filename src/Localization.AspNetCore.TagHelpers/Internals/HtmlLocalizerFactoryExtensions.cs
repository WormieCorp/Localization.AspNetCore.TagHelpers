using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Localization.AspNetCore.TagHelpers.Internals
{
	internal static class HtmlLocalizerFactoryExtensions
	{
		public static IHtmlLocalizer ResolveLocalizer(this IHtmlLocalizerFactory factory,
			ViewContext context,
			string applicationName)
		{
			return ResolveLocalizer(factory, context, applicationName, null, null);
		}

		public static IHtmlLocalizer ResolveLocalizer(
			this IHtmlLocalizerFactory factory, ViewContext context,
			string applicationName,
			Type resourceType,
			string resourceName)
		{
			if (resourceType != null)
				return factory.Create(resourceType);
			else
			{
				string name = resourceName;
				if (string.IsNullOrEmpty(name))
				{
					var path = context.ExecutingFilePath;
					if (string.IsNullOrEmpty(path))
						path = context.View.Path;

					Debug.Assert(!string.IsNullOrEmpty(path), "Couldn't determine a path for the view");

					name = BuildBaseName(path, applicationName);
				}

				return factory.Create(name, applicationName);
			}
		}

		private static string BuildBaseName(string path, string applicationName)
		{
			var extension = Path.GetExtension(path);
			var startIndex = path[0] == '/' || path[0] == '\\' ? 1 : 0;
			var length = path.Length - startIndex - extension.Length;
			var capacity = length + applicationName.Length + 1;
			var builder = new StringBuilder(path, startIndex, length, capacity);

			builder.Replace('/', '.').Replace('\\', '.');

			builder.Insert(0, '.');
			builder.Insert(0, applicationName);

			return builder.ToString();
		}
	}
}
