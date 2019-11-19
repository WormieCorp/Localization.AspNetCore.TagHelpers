// -----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="WormieCorp">
//   Copyright (c) WormieCorp. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
// <author>Kim Nordmo</author>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "Wishes to not prefix with 'this'")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Constants should be all uppercase with underscore")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Makes it harder to understand the code", Scope = "member", Target = "~M:Localization.AspNetCore.TagHelpers.Internals.HtmlLocalizerFactoryExtensions.ResolveLocalizer(Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory,Microsoft.AspNetCore.Mvc.Rendering.ViewContext,System.String,System.Type,System.String)~Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This needs to be able to be set", Scope = "member", Target = "~P:Localization.AspNetCore.TagHelpers.LocalizeAttributeTagHelper.ParameterValues")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This needs to be able to be set", Scope = "member", Target = "~P:Localization.AspNetCore.TagHelpers.LocalizeAttributeTagHelper.AttributeValues")]
