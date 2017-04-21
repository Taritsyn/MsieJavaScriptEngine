Change log
==========

## April 21, 2017 - v2.2.0
 * Added support of .NET Core 1.0.4
 * In JsRT modes now script error contains a full stack trace
 * In `MsieJsEngine` class was added overloaded versions of the `Evaluate`, `Evaluate<T>` and `Execute` methods, which take the document name as second parameter
 * Now all modes support the possibility to debug in Visual Studio by adding the `debugger` statement to script code

## February 12, 2017 - v2.1.2
 * Fixed a error causing a crash during finalization

## February 10, 2017 - v2.1.1
 * Was made refactoring

## December 19, 2016 - v2.1.0
 * Added support of .NET Core 1.0.3
 * Downgraded .NET Framework version from 4.5.1 to 4.5
 * Now when you call the overloaded version of the `ExecuteResource` method, that takes the type, need to pass the resource name without the namespace
 * Fixed a error “Out of stack space”
 * JSON2 library was updated to version of October 28, 2016

## December 2, 2016 - v2.0.2
 * Another attempt to prevent occurrence of the access violation exception in the `CallFunction` method

## November 8, 2016 - v2.0.1
 * All exceptions made serializable

## September 19, 2016 - v2.0.0
 * Added support of .NET Core 1.0.1 (only supported `ChakraIeJsRt` and `ChakraEdgeJsRt` modes) and .NET Framework 4.5.1
 * Added the `CollectGarbage` method

## September 17, 2016 - v2.0.0 Beta 2
 * Added support of .NET Core 1.0.1

## September 9, 2016 - v2.0.0 Beta 1
 * Added the `CollectGarbage` method

## September 3, 2016 - v2.0.0 Alpha 1
 * Added support of .NET Core 1.0 (only supported `ChakraIeJsRt` and `ChakraEdgeJsRt` modes) and .NET Framework 4.5.1

## August 17, 2016 - v1.7.2
 * An attempt was made to prevent occurrence of the access violation exception in the `CallFunction` method

## May 24, 2016 - v1.7.1
 * JSON2 library was updated to version of May 10, 2016

## March 4, 2016 - v1.7.0
 * Added the `EmbedHostObject` method (embeds a instance of simple class, structure or delegate to script code)
 * Added the `EmbedHostType` method (embeds a host type to script code)
 * Added a possibility to debug in Visual Studio by adding the `debugger` statement to script code. This feature only works in the `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
 * In JavaScript engine settings was added one new property - `EnableDebugging` (default `false`)
 * Improved implementation of the `CallFunction` method for Chakra JsRT modes

## February 26, 2016 - v1.7.0 Beta 1
 * Added the `EmbedHostType` method (embeds a host type to script code)

## January 16, 2016 - v1.7.0 Alpha 2
 * Added a possibility to debug in Visual Studio by adding the `debugger` statement to script code. This feature only works in the `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
 * In JavaScript engine settings was added one new property - `EnableDebugging` (default `false`)

## January 5, 2016 - v1.7.0 Alpha 1
 * Added the `EmbedHostObject` method (embeds a instance of simple class, structure or delegate to script code)
 * Improved implementation of the `CallFunction` method for Chakra JsRT modes

## December 3, 2015 - v1.6.0
 * Added support of “Edge” JsRT version of Chakra JavaScript engine
 * `ChakraJsRt` mode was renamed to `ChakraIeJsRt`
 
## July 23, 2015 - v1.5.6
 * Source code of the `ChakraJsRtJsEngine` was synchronized with the Chakra Sample Hosts version of July 11, 2015

## June 29, 2015 - v1.5.5
 * Fixed an error, that occurs on computers with IE 6
 * Removed `Obsolete` attribute from parameterless constructor

## June 28, 2015 - v1.5.4
 * In `ChakraActiveScript` mode added native support of ECMAScript 5 (without polyfills)
 * Added `JsEngineSettings` class for any reason in the future to abandon redundant constructors

## May 5, 2015 - v1.5.3
 * JSON2 library was updated to version of May 3, 2015

## April 5, 2015 - v1.5.2
 * JSON2 library was updated to version of February 25, 2015

## January 13, 2015 - v1.5.1
 * In ECMAScript 5 Polyfill added polyfill for the `String.prototype.split` method

## October 12, 2014 - v1.5.0
 * Removed dependency on `System.Web.Extensions`
 * Assembly is now targeted on the .NET Framework 4 Client Profile

## July 22, 2014 - v1.4.4
 * Source code of the `ChakraJsRtJsEngine` was synchronized with the Chakra Sample Hosts version of July 22, 2014

## April 27, 2014 - v1.4.3
 * In solution was enabled NuGet package restore
 * Fixed [JavaScriptEngineSwitcher.Msie's bug #7](https://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/7) "MsieJavaScriptEngine.ActiveScript.ActiveScriptException not wrapped"

## March 24, 2014 - v1.4.2
 * Fixed [JavaScriptEngineSwitcher.Msie's bug #5](http://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/5) "MSIE "Catastrophic failure" when disposing"

## March 22, 2014 - v1.4.1
 * Fixed minor bugs

## February 27, 2014 - v1.4.0
 * Removed following methods: `HasProperty`, `GetPropertyValue`, `SetPropertyValue` and `RemoveProperty`
 * Fixed [bug #3](http://github.com/Taritsyn/MsieJavaScriptEngine/issues/3) "execute code from different threads"
 * Now in the `ChakraJsRt` mode is available a more detailed information about errors
 * In ECMAScript 5 Polyfill improved a performance of the `String.prototype.trim` method
 * JSON2 library was updated to version of February 4, 2014

## January 16, 2014 - v1.3.0
 * Added support of the JsRT version of Chakra
 * Now the MSIE JavaScript Engine can work in 4 modes: `Auto` (selected by default), `Classic`, `ChakraActiveScript` and `ChakraJsRt`
 * Following methods are obsolete: `HasProperty`, `GetPropertyValue`, `SetPropertyValue` and `RemoveProperty`

## December 30, 2013 - v1.2.0
 * Fixed errors in ECMAScript 5 Polyfill
 * Added support of JavaScript `undefined` type

## September 3, 2013 - v1.1.3
 * Access modifier of the `JsEngineLoadException` class has changed to public

## June 20, 2013 - v1.1.2
 * JSON2 library was updated to version of May 26, 2013

## October 15, 2012 - v1.1.1
 * Assembly `MsieJavaScriptEngine.dll` now signed

## October 11, 2012 - v1.1.0
 * Added ability of using the Douglas Crockford's [JSON2](http://github.com/douglascrockford/JSON-js) library
 * By default using of the JSON2 library is disabled

## September 21, 2012 - v1.0.8
 * Changed the format of error messages

## September 9, 2012 - v1.0.7
 * Added the `ActiveScriptErrorFormatter` class

## August 29, 2012 - v1.0.5
 * [JavaScript Array Polyfills from TutorialsPoint.com](http://www.tutorialspoint.com/javascript/) was replaced by the Douglas Crockford's [ECMAScript 5 Polyfill](http://nuget.org/packages/ES5)
 * By default using of the ECMAScript 5 Polyfill is disabled

## August 27, 2012 - v1.0.1
 * Added the `JsEngineLoadException` class

## August 26, 2012 - v1.0.0
 * Initial version uploaded