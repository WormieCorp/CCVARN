# CCVARN (dotnet-ccvarn)

[![All Contributors][all-contrib-badge]](#contributors)
[![Codecov Report][codecov-badge]][codecov]
[![Nuget][nuget-badge]][nuget]

Helper utility for asserting version and generating release notes based on conventional commits

CCVARN helps developers using [conventional-commits][] to have version and release notes automatically created based on these commits.

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Background](#background)
- [Install](#install)
- [Usage](#usage)
- [API](#api)
- [Maintainers](#maintainers)
- [Contributing](#contributing)
  - [Contributors](#contributors)
- [License](#license)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Background

CCVARN was created due to a personal need of having the ability to automatically version and create release notes when commits following [conventional-commits][] were being used.

## Install

CCVARN can be installed by using the .NET Core toolset by running the following:

```console
dotnet tool install -g dotnet-ccvarn
```

## Usage

The most basic form of using CCVARN in to run

```console
dotnet ccvarn parse
```

This will assert the version and extract any commits that follow conventional commit.
The results is by default written to a `JSON` file called `CCVARN.json` (can be overridden by passing in a different file path).

Additionally if you want to have release notes exported as plain text or markdown, you can additionally run:

```console
dotnet ccvarn --output ReleaseNotes.txt --output ReleaseNotes.md
```

The type of the output is determined based on the file extension specified (currently supported are `.txt` and `.md` for release notes, and `.json` for exporting all information).

## Maintainers

[@AdmiringWorm][]

## Contributing

CCVARN follows the [Contributor Covenant][] Code of Conduct.

We accept Pull Requests.
Please see [the contributing file][contributing] for how to contribute to CCVARN!

Small note: If editing the README, please conform to the [standard-readme][] specification.

This project follows the [all-contributors][] specification. Contributions of any kind is welcome!

### Contributors

Thanks goes to these wonderful people ([emoji key][]):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

## License

[GNU General Public License v3.0 or later © Kim J. Nordmo][license]

[@AdmiringWorm]:        https://github.com/AdmiringWorm
[all-contrib-badge]:    https://img.shields.io/github/all-contributors/WormieCorp/CCVARN.svg?color=orange&style=flat-square
[all-contributors]:     https://github.com/all-contributors/all-contributors
[codecov-badge]:        https://img.shields.io/codecov/c/github/WormieCorp/CCVARN.svg?logo=codecov&style=flat-square
[codecov]:              https://codecov.io/gh/WormieCorp/CCVARN
[contributing]:         CONTRIBUTING.md
[Contributor Covenant]: https://www.contributor-covenant.org/version/2/0/code_of_conduct/
[conventional-commits]: https://www.conventionalcommits.org/en/v1.0.0/
[emoji key]:            https://allcontributors.org/docs/en/emoji-key
[license]:              LICENSE
[nuget-badge]:          https://img.shields.io/nuget/v/dotnet-ccvarn?logo=nuget&style=flat-square
[nuget]:                https://nuget.org/packages/dotnet-ccvarn/
[standard-readme]:      https://github.com/RichardLitt/standard-readme
