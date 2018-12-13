# Localization.AspNetCore.TagHelpers

[![Join the chat at https://gitter.im/WormieCorp/Localization.AspNetCore.TagHelpers](https://badges.gitter.im/WormieCorp/Localization.AspNetCore.TagHelpers.svg)](https://gitter.im/WormieCorp/Localization.AspNetCore.TagHelpers?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![CLA assistant](https://cla-assistant.io/readme/badge/WormieCorp/Localization.AspNetCore.TagHelpers)](https://cla-assistant.io/WormieCorp/Localization.AspNetCore.TagHelpers)
[![Open Source Helpers](https://www.codetriage.com/wormiecorp/localization.aspnetcore.taghelpers/badges/users.svg)](https://www.codetriage.com/wormiecorp/localization.aspnetcore.taghelpers)
[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=WormieCorp/Localization.AspNetCore.TagHelpers)](https://dependabot.com)

| AppVeyor | NuGet | GitHub | Codecov |
| :------: | :---: | :----: | :-----: |
| [![AppVeyor](https://img.shields.io/appveyor/ci/AdmiringWorm/localization-aspnetcore-taghelpers.svg)](https://ci.appveyor.com/project/AdmiringWorm/localization-aspnetcore-taghelpers) | [![NuGet](https://img.shields.io/nuget/v/Localization.AspNetCore.TagHelpers.svg)](https://www.nuget.org/packages/Localization.AspNetCore.TagHelpers/) | [![GitHub release](https://img.shields.io/github/release/WormieCorp/Localization.AspNetCore.TagHelpers.svg)](https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/releases) | [![Codecov](https://codecov.io/github/WormieCorp/Localization.AspNetCore.TagHelpers/coverage.svg)](https://codecov.io/github/WormieCorp/Localization.AspNetCore.TagHelpers) |

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

You can read more about this on the [Documentation](https://wormiecorp.github.io/Localization.AspNetCore.TagHelpers/docs/helpers) page.
Please also see the Localization.Demo project for a working template with the use of these localization helpers (based on default asp.net core template)
