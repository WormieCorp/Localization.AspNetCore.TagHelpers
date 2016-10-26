//-----------------------------------------------------------------------
// <copyright file="Throws.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.AspNetCore.TagHelpers.Internals
{
  using System;
  using System.Runtime.CompilerServices;

  /// <summary>
  ///   Helper class from verifying parameters, and
  /// </summary>
  internal static class Throws
  {
    /// <summary>
    ///   Checks if the specified <paramref name="value" /> is <see langword="null" />, and throws an
    ///   Argument Null Exception if it is.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   If the <paramref name="value" /> is <see langword="null" />
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotNull(object value, string name)
    {
      if (value == null)
      {
        throw new ArgumentNullException(name);
      }
    }
  }
}
