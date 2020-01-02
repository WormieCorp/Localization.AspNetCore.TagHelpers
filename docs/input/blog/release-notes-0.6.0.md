---
Title: Release Notes v0.6.0
Published: 2019-03-01
Updated: 2020-01-02
Category: Releases
Author: AdmiringWorm
Lead: Release details of version 0.6.0
---

As with the 0.5.0 release, we completely forgot to create a blog post for the 0.6.0 release as well.

This release finally saw the addition of allowing parameters to be passed when localizing html attributes, instead
of only allowing this to be used when translating html content.

This new change can be used with the following:

```html
<img localize-alt="My {0} localizable text" params-alt="Alternative" src="whatever.png" />
```

Please see the documentation for additional usage of this new functionality: [Attribute Localization](../docs/helpers/attribute-localization)

As part of this release we had [10 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/milestone/6?closed=1) closed.

## Feature

- [__#65__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/65) Add ability to pass parameters when localizing html attributes

## Improvements

- [__#33__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/pull/33) Bump Microsoft.AspNetCore.Mvc.Localization from 2.1.3 to 2.2.0
- [__#30__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/pull/30) Bump Microsoft.AspNetCore.Razor.Runtime from 2.1.2 to 2.2.0

## Documentation

- [__#43__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/43) Fix 'The properties padding-top, padding-bottom, padding-left, padding-right can be replaced by padding.' issue in src\Localization.Demo\wwwroot\css\site.css
- [__#40__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/40) Fix 'Values of 0 shouldn't have units specified.' issue in docs\input\assets\css\override.css

### Where to get it

You can download this release from [NuGet](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/0.6.0),
or from our [Github Releases page](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases/tag/0.5.0).
