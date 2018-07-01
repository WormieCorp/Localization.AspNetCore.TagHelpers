---
Title: Library Usage
Description: How to reference and make use of the library.
Order: 1
---

# Referencing Localization.AspNetCore.TagHelpers

The first thing you must to do make the tag helpers available to be used is to add
the necessary `addTagHelper` method in your view file.

This can be done in two ways:
1. Adding the necessary `addTagHelper` in every view file that you need to use the provided helpers.
2. Create a common `_ViewImports.cshtml` file where you have the necessary `addTagHelper` method.

The following `addTagHelper` method can be used to add the helpers in one of the ways you decide:
```
@addTagHelper *, Localization.AspNetCore.TagHelpers
```

We also recommend adding a using statement for the `Localization.AspNetCore.TagHelpers` namespace as
that is where all tag helpers and their options are located.

See [Available helper](helpers) for more information on which tag helpers can be used to localize content.

# Setting Global Options

To override the default options you can add the following to `Startup.cs` in the `ConfigureServices` method:

```cs
services.Configure<LocalizeTagHelperOptions>(options =>
{
  options.NewLineHandling = NewLineHandling.Unix,
  options.TrimWhitespace = false
});
```
See the [API Documentation](../api/Localization.AspNetCore.TagHelpers/LocalizeTagHelperOptions/) for options that can be globally overridden.
