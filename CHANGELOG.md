## 0.6.0 (03-01-2019)


As part of this release we had [10 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/milestone/6?closed=1) closed.


__Feature__

- [__#65__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/65) Add ability to pass parameters when localizing html attributes

__Improvements__

- [__#33__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/pull/33) Bump Microsoft.AspNetCore.Mvc.Localization from 2.1.3 to 2.2.0
- [__#30__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/pull/30) Bump Microsoft.AspNetCore.Razor.Runtime from 2.1.2 to 2.2.0

__Documentation__

- [__#43__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/43) Fix 'The properties padding-top, padding-bottom, padding-left, padding-right can be replaced by padding.' issue in src\Localization.Demo\wwwroot\css\site.css
- [__#40__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/40) Fix 'Values of 0 shouldn't have units specified.' issue in docs\input\assets\css\override.css


## 0.5.0 (10-13-2018)


As part of this release we had [17 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/0.4.0...0.5.0) which resulted in [1 issue](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=5&state=closed) being closed.


__Improvement__

- [__#25__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/25) Cache IHtmlLocalizer in views (thanks to @encrypt0r)


## 0.4.0 (07-01-2018)


As part of this release we had [46 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/0.3.0...0.4.0) which resulted in [5 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=4&state=closed) being closed.


__Breaking change__

- [__#24__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/24) Change to only target .net standard 2.0

__Feature__

- [__#21__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/21) Allow global override of html encoding

__Improvement__

- [__#18__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/18) Update dependencies to the latest version

__Documentation__

- [__#19__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/19) Implemented API documentation using Wyam


## 0.3.0 (03-14-2017)


As part of this release we had [25 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/0.2.0...0.3.0) which resulted in [3 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=3&state=closed) being closed.


__Feature__

- [__#13__](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/13) Add ability to reuse an existing instantiated IViewLocalizer/IHtmlLocalizer


## 0.2.0 (10-26-2016)


As part of this release we had [20 commits](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/compare/v0.1.0...0.2.0) which resulted in [5 issues](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?milestone=2&state=closed) being closed.

**Breaking change**
- [**#10**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/10) Renamed most tag attributes
  - The following tag names was changes  
    - localize-html -> html  
    - localize (with name) -> resource-name  
    - localize-trim -> trim  
    - localize-type -> resource-type  

**Features**
- [**#6**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/6) Add ability to normalize newlines
  - Available using tag attribute `newline`
- [**#5**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/5) Add line whitespace trimming
  - Available using tag attribute `trimlines`

**Bug**
- [**#8**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/8) Cannot implicitly convert type 'bool' to 'string'

**Improvement**
- [**#7**](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues/7) Allow Whitespace trimming to be set globally



## v0.1.0 (10-22-2016)


First (**NOT**) Working Release

**NOTE: Unfortunately this release was a little premature, and contains a bug
which causes tag helpers to fail if `localize-html` is set**

