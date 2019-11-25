// -----------------------------------------------------------------------
// <copyright file="HtmlLocalizerFactoryExtensions.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers.Internals
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Text;

  using Microsoft.AspNetCore.Mvc.Localization;
  using Microsoft.AspNetCore.Mvc.Rendering;

  /// <summary>
  ///   Extension methods for extending the <see cref="IHtmlLocalizerFactory" />.
  /// </summary>
  internal static class HtmlLocalizerFactoryExtensions
  {
    private const string CACHED_LOCALIZER_KEY = "CachedLocalizerFor";

    /// <summary>
    ///   Resolves a HTML localizer using the view name from the specified
    ///   <paramref name="context">view context</paramref>.
    /// </summary>
    /// <param name="factory">The html factory to create the HTML localizer from.</param>
    /// <param name="context">The view context to resolve the view name from.</param>
    /// <param name="applicationName">Name of the application (Usually the Assembly name).</param>
    /// <returns>The created HTML localizer.</returns>
    /// <seealso cref="ResolveLocalizer(IHtmlLocalizerFactory, ViewContext, string, Type, string)" />
    /// <seealso cref="IHtmlLocalizerFactory.Create(string, string)" />
    public static IHtmlLocalizer ResolveLocalizer(
      this IHtmlLocalizerFactory factory,
      ViewContext context,
      string applicationName)
    {
      return ResolveLocalizer(factory, context, applicationName, null, null);
    }

    /// <summary>
    ///   Resolves a HTML localizer using all the specified parameters.
    /// </summary>
    /// <param name="factory">The html factory to create the HTML localizer from.</param>
    /// <param name="context">
    ///   The view context to resolve the name if both <paramref name="resourceType" /> and
    ///   <paramref name="resourceType" /> is <see langword="null" /> or <c>empty</c>.
    /// </param>
    /// <param name="applicationName">Name of the application (Usually the Assembly name).</param>
    /// <param name="resourceType">
    ///   Type of the resource to create the HTML localizer from (can be <see langword="null" />).
    /// </param>
    /// <param name="resourceName">
    ///   Name of the resource to create the HTML localizer from (can be <see langword="null" /> or empty).
    /// </param>
    /// <returns>The created HTML localizer.</returns>
    /// <seealso cref="ResolveLocalizer(IHtmlLocalizerFactory, ViewContext, string)" />
    /// <seealso cref="IHtmlLocalizerFactory.Create(string, string)" />
    /// <seealso cref="IHtmlLocalizerFactory.Create(Type)" />
    public static IHtmlLocalizer ResolveLocalizer(
      this IHtmlLocalizerFactory factory,
      ViewContext context,
      string applicationName,
      Type resourceType,
      string resourceName)
    {
      if (resourceType != null)
      {
        return GetLocalizerForType(factory, context, resourceType);
      }
      else
      {
        return string.IsNullOrEmpty(resourceName)
          ? GetLocalizerForPath(factory, context, applicationName)
          : GetLocalizerForName(factory, context, resourceName, applicationName);
      }
    }

    private static void AddLocalizerToCache(IHtmlLocalizer localizer, ViewContext context, string name)
    {
      var cacheName = CACHED_LOCALIZER_KEY + name;

      context.ViewData[cacheName] = localizer;
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

    private static IHtmlLocalizer GetLocalizerForName(IHtmlLocalizerFactory factory, ViewContext context, string resourceName, string applicationName)
    {
      if (resourceName.IndexOfAny(new[] { '/', '\\' }) >= 0)
      {
        resourceName = BuildBaseName(resourceName, applicationName);
      }

      var cacheName = $"{applicationName}:{resourceName}";
      var localizer = GetLocalizerFromCache(context, cacheName);

      if (localizer == null)
      {
        localizer = factory.Create(resourceName, applicationName);
        AddLocalizerToCache(localizer, context, cacheName);
      }

      return localizer;
    }

    private static IHtmlLocalizer GetLocalizerForPath(IHtmlLocalizerFactory factory, ViewContext context, string applicationName)
    {
      var name = string.IsNullOrEmpty(context.ExecutingFilePath)
        ? context.View.Path
        : context.ExecutingFilePath;

      Debug.Assert(!string.IsNullOrEmpty(name), "Couldn't determine a path for the view");

      return GetLocalizerForName(factory, context, name, applicationName);
    }

    private static IHtmlLocalizer GetLocalizerForType(IHtmlLocalizerFactory factory, ViewContext context, Type resourceType)
    {
      var typeName = resourceType.FullName;

      var localizer = GetLocalizerFromCache(context, typeName);

      if (localizer == null)
      {
        localizer = factory.Create(resourceType);
        AddLocalizerToCache(localizer, context, typeName);
      }

      return localizer;
    }

    private static IHtmlLocalizer GetLocalizerFromCache(ViewContext context, string name)
    {
      var cacheName = CACHED_LOCALIZER_KEY + name;

      return context.ViewData.ContainsKey(cacheName)
        ? context.ViewData[cacheName] as IHtmlLocalizer
        : null;
    }
  }
}
