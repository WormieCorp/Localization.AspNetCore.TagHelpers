---
Title: Release Notes v0.5.0
Published: 2018-07-01
Updated: 2020-01-02
Category: Releases
Author: AdmiringWorm
Lead: Release details of version 0.5.0
---

This blog post was supposed to be written a long time ago, but better late than never.

This release included an important addition of caching the created
[IHtmlLocalizer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.localization.ihtmllocalizer)
classes, so it do not have to be created every time the tag helpers are ran.

This inclusion was made possible thanks to the awesome work from [@encrypt0r](https://github.com/encrypt0r).

As part of this release we had [17 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/0.4.0...0.5.0) which resulted in [1 issue](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=5&state=closed) being closed.

## Improvement

- [__#25__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/25) Cache IHtmlLocalizer in views

### Where to get it

You can download this release from [NuGet](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/0.5.0),
or from our [Github Releases page](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases/tag/0.5.0).
