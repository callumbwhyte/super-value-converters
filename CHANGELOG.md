# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

## [2.0.0] - 2019-07-22
### Added
* Support for Umbraco 8 and `IPublishedElement`
* A single unified model for picker settings across all converters
* Support for the cleaner VS 2017 `csproj` file format
* Azure DevOps build pipeline configuration

### Removed
* Media picker converter as the functionality is now hanlded by Umbraco 8

## [1.2.0] - 2019-07-22
### Added
* Abstract `SuperValueConverterBase` class to support custom implementations

### Fixed
* Protection level of picker settings models is now internal
* Handling nulls when discovering types by name

## [1.1.0] - 2019-04-14
### Added
* Returning composition interfaces where more than one doctype is allowed and both inherit from the same models
* Updates to documentation on using compositions with Models Builder

### Fixed
* Protection level of extensions, helpers and pre-value mapper is now internal

## [1.0.1] - 2019-03-26
### Added
* New logo for package

### Fixed
* Checking for missing prevalues when mapping
* Handling nulls when mapping prevalues to arrays
* Resolving Nested Content properties with only one doctype

## [1.0.0] - 2019-02-06
### Added
* Initial release of SuperValueConverters for Umbraco 7.7
* Support for Media Picker, Multi-Node Tree Picker and Nested Content
* README file with information about the project and screenshots
* MIT license in the form of a LICENSE.md file

[Unreleased]: https://github.com/callumbwhyte/super-value-converters/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/callumbwhyte/super-value-converters/compare/v1.2.0...v2.0.0
[1.2.0]: https://github.com/callumbwhyte/super-value-converters/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/callumbwhyte/super-value-converters/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/callumbwhyte/super-value-converters/compare/v1.0.0...v1.0.1