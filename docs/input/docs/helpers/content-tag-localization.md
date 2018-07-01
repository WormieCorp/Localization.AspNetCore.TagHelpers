--
Title: Localize content using a tag
Description: Make use of a specific tag to determine the content to localize.
Order: 1
--

# Usage

The most basic and simplest use of this tag helper is with the following:
```html
<localize>
  Whatever text you want to localize here
</localize>
```

# Attributes provided by helper
Below is the attributes that can be set with this tag helper:

# Overriding the generation of a resource to use
This can be done with two different attribute names, each having their own use case.

## Option 1
```html
<localize resource-name="ResourceFileNameOrPath">Text to localize</localize>
```
You can use the `resource-name` attribute to pass the name of the resource file.
This will be passed as it is typed to the underlying [IHtmlLocalizer](https://docs.microsoft.com/dotnet/api/Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer?view=aspnetcore-2.0)
which may point it to a resource file.

***NOTE: This would depend on the implementation of the underlying localizer***

## Option 2
```html
<localize resource-type="@typeof(MyClassName)">Text to localize</localize>
```
You can use the `resource-type attribute to pass a class type which will be resolved by the
underlying Resource Localizer.

# Disabling Html Encoding of text to be localized
```html
<localize html="true">Html content to be localized</localize>
```
You can use the `html` attribute to choose whether the content should be html encoded or not,
this defaults to `false` (meaning the content will be encoded) if no `html` attribute is used.

***NOTE: Can also be globally overridden***

# Enable/Disable trimming whitespace
Again there are two distinct ways to do this.

## Option 1
```html
<localize trim="true">  Text to localize  </localize>
```

You can use the `trim` attribute to enable or disable trimming of whitespace at the beginning
and the end of the content.
This defaults to `true`, unless overridden in the global settings.

***NOTE: If the attribute `trimlines` is used, this attribute is ignored.***

## Option 2
```html
<localize trimlines="true">
  Multiline text
  to localize  
</localize>
```

You can use the `trimlines` attribute if you need to trim each line for its beginning and ending whitespace.
This defaults to `false` and is not recommended to use when not needed.

***NOTE: This overrides the previously mentioned `trim` attribute if it is set to `true`.***

# Handling line endings
```html
<localize newline="NewLineHandling.Auto">
Text to
localize
with pseudo
different line
endings
</localize>
```

You can use the `newline` attribute to normalize eol or if the resource file line endings and view line
endings isn't the same.
Defaults to Automatically using the current environments line endings, unless overridden in the global settings.

***NOTE: the line endings are normalized on the text that are to be localized,
but nothing is done with the already localized text.***

See the [API documentation](../../api/Localization.AspNetCore.TagHelpers/NewLineHandling/) for available values.

# Reuse an existing [IHtmlLocalizer](https://docs.microsoft.com/dotnet/api/Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer?view=aspnetcore-2.0)/[IViewLocalizer](https://docs.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.localization.iviewlocalizer?view=aspnetcore-2.0)

If you're already have injected a html/view localizer into the current view, you may reuse that
localizer by passing it for every call to the localize tag helper.

```html
@inject Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer loc;

<localize localizer="@loc">
  Text to localize
</localize>
```

***NOTE: There is a wish to have this automatically detected, but no such luck for this have been found yet.
Any help for this would be appreciated.***
