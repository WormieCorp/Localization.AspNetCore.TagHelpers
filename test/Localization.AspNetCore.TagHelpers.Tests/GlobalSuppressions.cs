//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Maintainability",
  "S1144:Unused private types or members should be removed",
  Justification = "This is necessary since base class needs these parameters",
  Scope = "member",
  Target = "~M:Localization.AspNetCore.TagHelpers.Tests.GenericLocalizeTagHelperTests.NoParametersSupported.#ctor(Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory,Microsoft.AspNetCore.Hosting.IHostingEnvironment)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability",
  "S1144:Unused private types or members should be removed",
  Justification = "This is necessary since base class needs these parameters",
  Scope = "member",
  Target = "~M:Localization.AspNetCore.TagHelpers.Tests.LocalizeTagHelperTests.LocalizeNoParametersTagHelper.#ctor(Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory,Microsoft.AspNetCore.Hosting.IHostingEnvironment)")]
