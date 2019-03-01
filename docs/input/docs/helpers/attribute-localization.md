---
Title: Localize Attributes
Description: Localize html attributes.
Order: 3
---

# Usage
So you came to realize you also needed to localize your html attributes?
Then you came to the right place.
We also provide the means to do this.

Let us say we want to localize the `alt` parameter on an icon.
This can be easily achieved by making use of the following example.
```html
<img localize-alt="My Alternative localizable text" src="whatever.png" />
```

This can be used for any attribute on an html element, just by prefixing the html attribute with `localize-`.
Unfortunately only basic localization is supported here, all text in the attribute will/should be html encoded.
There is currently no support for adding html in html attributes, and currently no plans to add it either.

Since the release of version `0.6.0` you can now also pass in parameters when localizing attributes.
```html
<img localize-alt="My {0} localizable text" params-alt="Alternative" src="whatever.png" />
```

You may also pass in several parameters by using a semi colon delimited string, like:
```html
<img localize-alt="My {0} {1} text" params-alt="Alternative;localizable" src="whatever.png" />
```