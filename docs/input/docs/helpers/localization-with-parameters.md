---
Title: Localization with parameters
Description: Use parameters when localizing content.
---

# Usage

Yes, we also provide the ability to pass in parameters when localizing content.
By only using html elements and attributes.

The most basic use is with the following:

## Using the tag element helper
```html
<localize>
  <parameter>My Parameter</parameter>
  This content is localized, but {0} isn't.
</localize>
```

## Using the tag attribute helper
```html
<span localize>
  <parameter>MyParameter</parameter>
  This content is localized, but {0} isn't.
</span>
```

# Localizing the parameter
Sometimes there is a need to localize the parameter as well, this can be done by reusing the localize attribute tag helpers.

## Using the tag element helper
```html
<localize>
  <parameter localize>My Parameter</localize>
  This content is localized, and {0} is also localized this time.
</localize>
```

## Using the tag attribute helper
```html
<span localize>
  <parameter localize>MyParameter</parameter>
  This content is localized, but {0} isn't.
</span>
```

***NOTE: Be careful of nesting `<parameter>` elements, this scenario hasn't been tested yet and is expected
to result in problems.***
