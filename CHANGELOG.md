# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

## [3.0.1] - 2021-03-15
### Changed
* Targeting `Umbraco.Cms.Infrastructure` as all of `Umbraco.Cms.Web.Common` is not needed

### Fixed
* Core Multi-Node Tree Picker converter was not being replaced correctly during startup

## [3.0.0] - 2021-03-03
### Added
* Initial release of SuperValueConverters for Umbraco 9+

## [2.1.0] - 2021-01-22
### Added
* Retreiving published content models from Umbraco's type cache
* Support for de-registering core converters

### Fixed
* Improved null checking for picker settings
* Accurately finding shared interfaces for types

## [2.0.1] - 2019-07-25
### Added
* Umbraco 8.1 as the minimum required Umbraco version

### Fixed
* Converters use `IPublishedPropertyType` for compatibility with Umbraco 8.1
* Broken links in Changelog

## [2.0.0] - 2019-07-22
### Added
* Support for Umbraco 8 and `IPublishedElement`
* A single unified model for picker settings across all converters
* Support for the cleaner SDK project format
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

[Unreleased]: https://github.com/callumbwhyte/super-value-converters/compare/release-3.0.1...HEAD
[3.0.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-3.0.0...release-3.0.1
[3.0.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-2.1.0...release-3.0.0
[2.1.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-2.0.1...release-2.1.0
[2.0.1]: https://github.com/callumbwhyte/super-value-converters/compare/release-2.0.0...release-2.0.1
[2.0.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-1.2.0...release-2.0.0
[1.2.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-1.1.0...release-1.2.0
[1.1.0]: https://github.com/callumbwhyte/super-value-converters/compare/release-1.0.1...release-1.1.0
[1.0.1]: https://github.com/callumbwhyte/super-value-converters/compare/release-1.0.0...release-1.0.1
[1.0.0]: https://github.com/callumbwhyte/super-value-converters/tree/release-1.0.0