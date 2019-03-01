// -----------------------------------------------------------------------
// <copyright file="NewLineHandling.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

namespace Localization.AspNetCore.TagHelpers
{
  /// <summary>
  ///   Enumeration for what kind of newline normalization the supported tag helpers should apply.
  /// </summary>
  public enum NewLineHandling
  {
    /// <summary>
    ///   Do not normalize line endings.
    /// </summary>
    None = 0,

    /// <summary>
    ///   Use the current Operating Systems preferred line ending.
    /// </summary>
    Auto,

    /// <summary>
    ///   Convert all line endings to Windows preferred (CRLF)
    /// </summary>
    Windows,

    /// <summary>
    ///   Convert all line endings to Unix preferred (LF)
    /// </summary>
    Unix,
  }
}
