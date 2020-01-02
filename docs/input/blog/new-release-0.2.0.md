---
Title: Release Notes v0.2.0
Published: 2016-10-26
Updated: 2020-01-02
Category: Releases
Author: AdmiringWorm
Lead: Release details of version 0.2.0
Description: Release details of Localization.AspNetCore.TagHelpers version 0.2.0
---

As part of this release we had [20 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/v0.1.0...0.2.0) which resulted in [5 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=2&state=closed) being closed.

## Breaking change

- [**#10**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/10) Renamed most tag attributes
  - The following tag names was changes
    - localize-html -> html
    - localize (with name) -> resource-name
    - localize-trim -> trim
    - localize-type -> resource-type

## Features

- [**#6**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/6) Add ability to normalize newlines
  - Available using tag attribute `newline`
- [**#5**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/5) Add line whitespace trimming
  - Available using tag attribute `trimlines`

## Bug

- [**#8**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/8) Cannot implicitly convert type 'bool' to 'string'

## Improvement

- [**#7**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/7) Allow Whitespace trimming to be set globally

### Where to get it

You can download this release from [NuGet](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/0.2.0),
or from our [Github Release page](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases/tag/0.2.0).
