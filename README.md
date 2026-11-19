# Umbraco SuperValueConverters

<img src="docs/img/logo.png?raw=true" alt="Umbraco SuperValueConverters" width="250" align="right" />

[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.SuperValueConverters.svg)](https://www.nuget.org/packages/Our.Umbraco.SuperValueConverters/)

SuperValueConverters is a collection of powerful property value converters for Umbraco, eliminating the need for casting and null checks on Umbraco picker values in views or controllers and helping to keep code cleaner.

## Getting started

This package is supported on Umbraco v10 (LTS) and v13 (LTS), but should also work on all intermediate versions.

Once installed you don't need to do anything to activate SuperValueConverters.

### Installation

SuperValueConverters is available via [NuGet](https://www.nuget.org/packages/Our.Umbraco.SuperValueConverters/).

To install with the .NET CLI, run the following command:

    $ dotnet add package Our.Umbraco.SuperValueConverters

To install from within Visual Studio, use the NuGet Package Manager UI or run the following command:

    PM> Install-Package Our.Umbraco.SuperValueConverters

## Usage

Where a picker that returns multiple items _(such as [Multi Node Picker](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/multinode-treepicker) or [Media Picker](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/media-picker-3) or [Block List](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/block-editor/block-list-editor))_ has been configured to only allow a single item to be selected, the returned value type will now be singular rather than `IEnumerable<T>`.

Currently the following datatypes are supported:

| Editor | Return Type |
|--------|-------------|
| [Block List Editor](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/block-editor/block-list-editor) | `BlockListItem<TContent, TSettings>` or `IEnumerable` |
| [Media Picker _(v3)_](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/media-picker-3) | `MediaWithCrops<TMedia>` or `IEnumerable` |
| [Multi-Node Tree Picker](https://docs.umbraco.com/umbraco-cms/13.latest/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/multinode-treepicker) | `IPublishedContent` or `IEnumerable` |
| Nested Content _(deprecated)_ | `IPublishedElement` or `IEnumerable` |

Supported value converters will no longer return `null` if no value has been picked - an empty collection will be returned instead, preventing the need for null checks.

### Models Builder

SuperValueConverters works seamlessly with [Models Builder](https://docs.umbraco.com/umbraco-cms/develop-with-umbraco/templating-and-rendering/templating/modelsbuilder) _(if you're using it)_ to return strongly typed models from pickers rather than the default type.

Where a picker has been configured to only allow items of a specific doctype, the returned value will be already cast to the relevant Models Builder generated model.

In cases where an allowed doctype uses [compositions](https://docs.umbraco.com/umbraco-cms/develop-with-umbraco/templating-and-rendering/templating/modelsbuilder/using-interfaces) Models Builder will generate an interface for that model. If more than one allowed doctype on a given picker implements the same interface, the returned value will be cast to the most commonly shared interface rather than the default type.

## Contribution guidelines

To raise a new bug, create an issue on the GitHub repository. To fix a bug or add new features, fork the repository and send a pull request with your changes. Feel free to add ideas to the repository's issues list if you would to discuss anything related to the library.

### Who do I talk to?

This project is maintained by [Callum Whyte](https://callumwhyte.com/) and contributors. If you have any questions about the project please get in touch on [Twitter](https://twitter.com/callumbwhyte), or by raising an issue on GitHub.

## Credits

The logo uses the [Energy](https://thenounproject.com/term/search/1603715/) icon from the [Noun Project](https://thenounproject.com) by [Scarlett McKay](https://thenounproject.com/scarlett.mckay/), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).

### A special #h5yr to our contributors

* [Lee Kelleher](https://github.com/leekelleher)
* [Robert Foster](https://github.com/robertjf)
* [Lukasz Koruba](https://github.com/lkoruba)
* [Nik Rimington](https://github.com/NikRimington)
* [Alejandro Ocampo](https://github.com/ja0b)
* [Richard Thompson](https://github.com/richarth)
* [Joe Glombek](https://github.com/glombek)

## License

Copyright &copy; 2026 [Callum Whyte](https://callumwhyte.com/), and other contributors

Licensed under the [MIT License](LICENSE.md).