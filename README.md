MSIE JavaScript Engine for .NET
===============================

![MSIE JS Engine Logo](http://i.imgur.com/T3K5q.png)

This project is a .NET wrapper for working with the Internet Explorer's JavaScript engines (JsRT version of Chakra, ActiveScript version of Chakra and Classic JavaScript Engine). 
Project was based on the code of [SassAndCoffee.JavaScript](http://github.com/paulcbetts/SassAndCoffee) and [Chakra Sample Hosts](http://github.com/panopticoncentral/chakra-host).

## Installation
This library can be installed through NuGet - [http://nuget.org/packages/MsieJavaScriptEngine](http://nuget.org/packages/MsieJavaScriptEngine).

## Release History
See the [changelog](CHANGELOG.md).

## License
[Microsoft Public License (Ms-PL)](http://github.com/Taritsyn/MsieJavaScriptEngine/blob/master/LICENSE.md)

## Credits
 * [SassAndCoffee.JavaScript](http://github.com/xpaulbettsx/SassAndCoffee) - [License: Microsoft Public License (Ms-PL)](http://github.com/paulcbetts/SassAndCoffee/blob/master/COPYING) Part of the code of this library served as the basis for the ActiveScript version of Chakra and Classic JavaScript Engine.
 * [Chakra Sample Hosts](http://github.com/panopticoncentral/chakra-host) - [License: Apache License 2.0 (Apache)](http://github.com/panopticoncentral/chakra-host/blob/master/LICENSE) C# example from this project served as the basis for the JsRT version of Chakra.
 * [ECMAScript 5 Polyfill](http://nuget.org/packages/ES5) - Adds support for many of the new functions in ECMAScript 5 to downlevel browsers using the samples provided by Douglas Crockford in his ["ECMAScript 5: The New Parts"](http://channel9.msdn.com/Events/MIX/MIX11/EXT13) talk.
 * [JSON2 library](http://github.com/douglascrockford/JSON-js) - Adds support of the JSON object from ECMAScript 5 to downlevel browsers.
 * [WebGrease](http://webgrease.codeplex.com/) - [License: Apache License 2.0 (Apache)](http://webgrease.codeplex.com/license) JS-files, that used MSIE JS Engine, minificated by using WG.exe.