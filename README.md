# Localization.AspNetCore.TagHelpers

[![All Contributors](https://img.shields.io/badge/all_contributors-7-orange.svg?style=flat-square)](#contributors)
[![Liberapay receiving](https://img.shields.io/liberapay/receives/WormieCorp.svg?logo=liberapay&style=flat-square)](https://liberapay.com/WormieCorp)

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

## Contributors

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/AdmiringWorm"><img src="https://avatars3.githubusercontent.com/u/1474648?v=4" width="100px;" alt=""/><br /><sub><b>Kim J. Nordmo</b></sub></a><br /><a href="#maintenance-AdmiringWorm" title="Maintenance">🚧</a></td>
    <td align="center"><a href="https://gitter.im"><img src="https://avatars2.githubusercontent.com/u/8518239?v=4" width="100px;" alt=""/><br /><sub><b>The Gitter Badger</b></sub></a><br /><a href="https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/commits?author=gitter-badger" title="Documentation">📖</a></td>
    <td align="center"><a href="https://www.codetriage.com"><img src="https://avatars0.githubusercontent.com/u/35302948?v=4" width="100px;" alt=""/><br /><sub><b>README Bot</b></sub></a><br /><a href="https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/commits?author=codetriage-readme-bot" title="Documentation">📖</a></td>
    <td align="center"><a href="https://encrypt0r.github.io/"><img src="https://avatars2.githubusercontent.com/u/16880059?v=4" width="100px;" alt=""/><br /><sub><b>Muhammad Azeez</b></sub></a><br /><a href="https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/issues?q=author%3Aencrypt0r" title="Ideas, Planning, & Feedback">🤔</a> <a href="https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/commits?author=encrypt0r" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/apps/dependabot"><img src="https://avatars3.githubusercontent.com/in/2141?v=4" width="100px;" alt=""/><br /><sub><b>dependabot[bot]</b></sub></a><br /><a href="https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/commits?author=dependabot[bot]" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/apps/whitesource-bolt-for-github"><img src="https://avatars2.githubusercontent.com/in/16809?v=4" width="100px;" alt=""/><br /><sub><b>whitesource-bolt-for-github[bot]</b></sub></a><br /><a href="#security-whitesource-bolt-for-github[bot]" title="Security">🛡️</a></td>
    <td align="center"><a href="https://github.com/apps/transifex-integration"><img src="https://avatars1.githubusercontent.com/in/18568?v=4" width="100px;" alt=""/><br /><sub><b>transifex-integration[bot]</b></sub></a><br /><a href="#translation-transifex-integration[bot]" title="Translation">🌍</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
