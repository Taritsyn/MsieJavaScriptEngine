MSIE JavaScript Engine for .NET
===============================

![MSIE JS Engine Logo](http://i.imgur.com/T3K5q.png)

This project is a .NET wrapper for working with the JavaScript engines of Internet Explorer and Edge (JsRT versions of Chakra, ActiveScript version of Chakra and Classic JavaScript Engine). 
Project was based on the code of [SassAndCoffee.JavaScript](http://github.com/paulcbetts/SassAndCoffee) and [Chakra Sample Hosts](http://github.com/panopticoncentral/chakra-host).

MSIE JavaScript Engine requires a installation of Internet Explorer or Edge on the machine and can work in 5 modes, that are defined in the `MsieJavaScriptEngine.JsEngineMode` enumeration:

 * `Auto`. Automatically selects the most modern JavaScript engine from available on the machine.
 * `Classic`. Classic MSIE JavaScript engine (supports ECMAScript 3 with possibility of using the ECMAScript 5 Polyfill and the JSON2 library). Requires Internet Explorer 6 or higher on the machine.
 * `ChakraActiveScript`. ActiveScript version of Chakra JavaScript engine (supports ECMAScript 5). Requires Internet Explorer 9 or higher on the machine.
 * `ChakraIeJsRt`. “Legacy” JsRT version of Chakra JavaScript engine (supports ECMAScript 5). Requires Internet Explorer 11 or higher on the machine.
 * `ChakraEdgeJsRt`. “Edge” JsRT version of Chakra JavaScript engine (supports ECMAScript 5). Requires Microsoft Edge on the machine.

The supported .NET types are as follows:

 * `MsieJavaScriptEngine.Undefined`
 * `System.Boolean`
 * `System.Int32`
 * `System.Double`
 * `System.String`

## Installation
This library can be installed through NuGet - [http://nuget.org/packages/MsieJavaScriptEngine](http://nuget.org/packages/MsieJavaScriptEngine).

## Usage
Consider a simple example of usage of the MSIE JavaScript Engine:

	namespace MsieJavaScriptEngine.Example.Console
	{
		using System;

		using MsieJavaScriptEngine;
		using MsieJavaScriptEngine.Helpers;

		class Program
		{
			static void Main(string[] args)
			{
				try
				{
					using (var jsEngine = new MsieJsEngine(new JsEngineSettings
					{
						EngineMode = JsEngineMode.Auto,
						UseEcmaScript5Polyfill = false,
						UseJson2Library = false
					}))
					{
						const string expression = "7 * 8 - 20";
						var result = jsEngine.Evaluate<int>(expression);

						Console.WriteLine("{0} = {1}", expression, result);
					}
				}
				catch (JsEngineLoadException e)
				{
					Console.WriteLine("During loading of JavaScript engine an error occurred.");
					Console.WriteLine();
					Console.WriteLine(JsErrorHelpers.Format(e));
				}
				catch (JsRuntimeException e)
				{
					Console.WriteLine("During execution of JavaScript code an error occurred.");
					Console.WriteLine();
					Console.WriteLine(JsErrorHelpers.Format(e));
				}

				Console.ReadLine();
			}
		}
	}

First we create an instance of the `MsieJsEngine` class and pass the following parameters to the constructor:

 1. `engineMode` - JavaScript engine mode;
 2. `useEcmaScript5Polyfill` - flag for whether to use the ECMAScript 5 Polyfill;
 3. `useJson2Library` - flag for whether to use the [JSON2](http://github.com/douglascrockford/JSON-js) library.

The values of constructor parameters in the above code correspond to the default values.

Then we evaluate a JavaScript expression by using of the `Evaluate` method and output its result to the console.

In addition, we provide handling of the following exception types: `JsEngineLoadException` and `JsRuntimeException`.

## Release History
See the [changelog](CHANGELOG.md).

## License
[Microsoft Public License (Ms-PL)](http://github.com/Taritsyn/MsieJavaScriptEngine/blob/master/LICENSE.md)

## Credits
 * [SassAndCoffee.JavaScript](http://github.com/xpaulbettsx/SassAndCoffee) - [License: Microsoft Public License (Ms-PL)](http://github.com/paulcbetts/SassAndCoffee/blob/master/COPYING) Part of the code of this library served as the basis for the ActiveScript version of Chakra and Classic JavaScript Engine.
 * [Chakra Sample Hosts](http://github.com/panopticoncentral/chakra-host) - [License: Apache License 2.0 (Apache)](http://github.com/panopticoncentral/chakra-host/blob/master/LICENSE) C# example from this project served as the basis for the JsRT version of Chakra.
 * [ECMAScript 5 Polyfill](http://nuget.org/packages/ES5) and [MDN JavaScript Polyfills](http://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference) - Adds support for many of the new functions in ECMAScript 5 to downlevel browsers.
 * [Cross-Browser Split](http://blog.stevenlevithan.com/archives/cross-browser-split) - Adds ECMAScript compliant and uniform cross-browser split method.
 * [JSON2 library](http://github.com/douglascrockford/JSON-js) - Adds support of the JSON object from ECMAScript 5 to downlevel browsers.
 * [Microsoft Ajax Minifier](http://ajaxmin.codeplex.com/) - [License: Apache License 2.0 (Apache)](http://ajaxmin.codeplex.com/license) JS-files, that used MSIE JS Engine, minificated by using `AjaxMinifier.exe`.

## Who's Using MSIE JavaScript Engine
If you use the MSIE JavaScript Engine in some project, please send me a message so I can include it in this list:

 * [Chevron](http://github.com/SimonCropp/Chevron) by Simon Cropp
 * [JavaScript Engine Switcher](http://github.com/Taritsyn/JavaScriptEngineSwitcher) by Andrey Taritsyn
 * [SquishIt](http://github.com/jetheredge/SquishIt) by Justin Etheredge and Alex Cuse
 * [Strike](http://github.com/SimonCropp/Strike) by Simon Cropp
