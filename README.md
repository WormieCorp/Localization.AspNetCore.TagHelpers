# Localization.AspNetCore.TagHelpers

[![CLA assistant](https://cla-assistant.io/readme/badge/WormieCorp/Localization.AspNetCore.TagHelpers)](https://cla-assistant.io/WormieCorp/Localization.AspNetCore.TagHelpers)

| AppVeyor | Travis | NuGet | GitHub | Codecov |
| :------: | :----: | :---: | :----: | :-----: |
| [![AppVeyor](https://img.shields.io/appveyor/ci/AdmiringWorm/localization-aspnetcore-taghelpers.svg)](https://ci.appveyor.com/project/AdmiringWorm/localization-aspnetcore-taghelpers) | [![Travis](https://img.shields.io/travis/WormieCorp/Localization.AspNetCore.TagHelpers.svg)](https://travis-ci.org/WormieCorp/Localization.AspNetCore.TagHelpers) | [![NuGet](https://img.shields.io/nuget/v/Localization.AspNetCore.TagHelpers.svg)](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/) | [![GitHub release](https://img.shields.io/github/release/WormieCorp/Localization.AspNetCore.TagHelpers.svg)](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases) | [![Codecov](https://codecov.io/github/WormieCorp/Localization.AspNetCore.TagHelpers/coverage.svg)](https://codecov.io/github/WormieCorp/Localization.AspNetCore.TagHelpers) |

Asp.Net Core Tag Helpers to use when localizing Asp.Net Core application instead of manually injecting IViewLocator

## Where to get it
Official published version are available on [NuGet](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/)
or on the [GitHub Release](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases) page.

## Usage
To use these tag helpers add the following to your `_ViewImports.cshtml` file
```
@addTagHelper *, Localization.AspNetCore.TagHelpers
```
#### The package currently supports three tag helpers with the following usage cases:
Usage with the tag name `<localize>Text to localize</localize>`  
Usage with a html attribute `<span localize>Text to localize</localize>`  
Ability to localize html attribute using the following: `<span localize-title="I'm the title to localize">This won't</span>`

See the full demo project based on the default asp.net core template for possible usages.


***WIP***
