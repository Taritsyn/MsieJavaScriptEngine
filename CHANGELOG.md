Change log
==========

## v2.2.6 - February 23, 2018
 * In JsRT modes during calling of the `CollectGarbage` method is no longer performed blocking

## v2.2.5 - December 23, 2017
 * Removed a redundant code
 * Fixed a error, that occurred in the `Classic` mode during removing the embedded host objects and types
 * Fixed a error, that occurred during finding the suitable method overload, that receives numeric values and interfaces as parameters, of the host object

## v2.2.4 - August 25, 2017
 * In ActiveScript modes now are uses the short names of error categories
 * In `Classic` mode during debugging now script error contains a full stack trace
 * In JsRT modes the compilation error messages now contains a information about the error location

## v2.2.3 - July 4, 2017
 * Now during the rethrowing of exceptions are preserved the full call stack trace
 * Reduced a number of delegate-wrappers

## v2.2.2 - June 28, 2017
 * Switched to Apache license
 * In JsRT modes fixed a problems in calculation of error locations
 * An attempt was made to prevent occurrence of the access violation exception
 * Now the original exception is added to instance of the `JsRuntimeException` class as an inner exception
 * An attempt was made to prevent a blocking of finalizer's thread
 * Added support of identifier names compliant with ECMAScript 5

## v2.2.1 - April 25, 2017
 * Now during debugging in ActiveScript modes the script error contains a error location

## v2.2.0 - April 21, 2017
 * Added support of .NET Core 1.0.4
 * In JsRT modes now script error contains a full stack trace
 * In `MsieJsEngine` class was added overloaded versions of the `Evaluate`, `Evaluate<T>` and `Execute` methods, which take the document name as second parameter
 * Now all modes support the possibility to debug in Visual Studio by adding the `debugger` statement to script code

## v2.1.2 - February 12, 2017
 * Fixed a error causing a crash during finalization

## v2.1.1 - February 10, 2017
 * Was made refactoring

## v2.1.0 - December 19, 2016
 * Added support of .NET Core 1.0.3
 * Downgraded .NET Framework version from 4.5.1 to 4.5
 * Now when you call the overloaded version of the `ExecuteResource` method, that takes the type, need to pass the resource name without the namespace
 * Fixed a error “Out of stack space”
 * JSON2 library was updated to version of October 28, 2016

## v2.0.2 - December 2, 2016
 * Another attempt to prevent occurrence of the access violation exception in the `CallFunction` method

## v2.0.1 - November 8, 2016
 * All exceptions made serializable

## v2.0.0 - September 19, 2016
 * Added support of .NET Core 1.0.1 (only supported `ChakraIeJsRt` and `ChakraEdgeJsRt` modes) and .NET Framework 4.5.1
 * Added the `CollectGarbage` method

## v2.0.0 Beta 2 - September 17, 2016
 * Added support of .NET Core 1.0.1

## v2.0.0 Beta 1 - September 9, 2016
 * Added the `CollectGarbage` method

## v2.0.0 Alpha 1 - September 3, 2016
 * Added support of .NET Core 1.0 (only supported `ChakraIeJsRt` and `ChakraEdgeJsRt` modes) and .NET Framework 4.5.1

## v1.7.2 - August 17, 2016
 * An attempt was made to prevent occurrence of the access violation exception in the `CallFunction` method

## v1.7.1 - May 24, 2016
 * JSON2 library was updated to version of May 10, 2016

## v1.7.0 - March 4, 2016
 * Added the `EmbedHostObject` method (embeds a instance of simple class, structure or delegate to script code)
 * Added the `EmbedHostType` method (embeds a host type to script code)
 * Added a possibility to debug in Visual Studio by adding the `debugger` statement to script code. This feature only works in the `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
 * In JavaScript engine settings was added one new property - `EnableDebugging` (default `false`)
 * Improved implementation of the `CallFunction` method for Chakra JsRT modes

## v1.7.0 Beta 1 - February 26, 2016
 * Added the `EmbedHostType` method (embeds a host type to script code)

## v1.7.0 Alpha 2 - January 16, 2016
 * Added a possibility to debug in Visual Studio by adding the `debugger` statement to script code. This feature only works in the `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
 * In JavaScript engine settings was added one new property - `EnableDebugging` (default `false`)

## v1.7.0 Alpha 1 - January 5, 2016
 * Added the `EmbedHostObject` method (embeds a instance of simple class, structure or delegate to script code)
 * Improved implementation of the `CallFunction` method for Chakra JsRT modes

## v1.6.0 - December 3, 2015
 * Added support of “Edge” JsRT version of Chakra JavaScript engine
 * `ChakraJsRt` mode was renamed to `ChakraIeJsRt`
 
## v1.5.6 - July 23, 2015
 * Source code of the `ChakraJsRtJsEngine` was synchronized with the Chakra Sample Hosts version of July 11, 2015

## v1.5.5 - June 29, 2015
 * Fixed an error, that occurs on computers with IE 6
 * Removed `Obsolete` attribute from parameterless constructor

## v1.5.4 - June 28, 2015
 * In `ChakraActiveScript` mode added native support of ECMAScript 5 (without polyfills)
 * Added `JsEngineSettings` class for any reason in the future to abandon redundant constructors

## v1.5.3 - May 5, 2015
 * JSON2 library was updated to version of May 3, 2015

## v1.5.2 - April 5, 2015
 * JSON2 library was updated to version of February 25, 2015

## v1.5.1 - January 13, 2015
 * In ECMAScript 5 Polyfill added polyfill for the `String.prototype.split` method

## v1.5.0 - October 12, 2014
 * Removed dependency on `System.Web.Extensions`
 * Assembly is now targeted on the .NET Framework 4 Client Profile

## v1.4.4 - July 22, 2014
 * Source code of the `ChakraJsRtJsEngine` was synchronized with the Chakra Sample Hosts version of July 22, 2014

## v1.4.3 - April 27, 2014
 * In solution was enabled NuGet package restore
 * Fixed [JavaScriptEngineSwitcher.Msie's bug #7](https://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/7) "MsieJavaScriptEngine.ActiveScript.ActiveScriptException not wrapped"

## v1.4.2 - March 24, 2014
 * Fixed [JavaScriptEngineSwitcher.Msie's bug #5](http://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/5) "MSIE "Catastrophic failure" when disposing"

## v1.4.1 - March 22, 2014
 * Fixed minor bugs

## v1.4.0 - February 27, 2014
 * Removed following methods: `HasProperty`, `GetPropertyValue`, `SetPropertyValue` and `RemoveProperty`
 * Fixed [bug #3](http://github.com/Taritsyn/MsieJavaScriptEngine/issues/3) "execute code from different threads"
 * Now in the `ChakraJsRt` mode is available a more detailed information about errors
 * In ECMAScript 5 Polyfill improved a performance of the `String.prototype.trim` method
 * JSON2 library was updated to version of February 4, 2014

## v1.3.0 - January 16, 2014
 * Added support of the JsRT version of Chakra
 * Now the MSIE JavaScript Engine can work in 4 modes: `Auto` (selected by default), `Classic`, `ChakraActiveScript` and `ChakraJsRt`
 * Following methods are obsolete: `HasProperty`, `GetPropertyValue`, `SetPropertyValue` and `RemoveProperty`

## v1.2.0 - December 30, 2013
 * Fixed errors in ECMAScript 5 Polyfill
 * Added support of JavaScript `undefined` type

## v1.1.3 - September 3, 2013
 * Access modifier of the `JsEngineLoadException` class has changed to public

## v1.1.2 - June 20, 2013
 * JSON2 library was updated to version of May 26, 2013

## v1.1.1 - October 15, 2012
 * Assembly `MsieJavaScriptEngine.dll` now signed

## v1.1.0 - October 11, 2012
 * Added ability of using the Douglas Crockford's [JSON2](http://github.com/douglascrockford/JSON-js) library
 * By default using of the JSON2 library is disabled

## v1.0.8 - September 21, 2012
 * Changed the format of error messages

## v1.0.7 - September 9, 2012
 * Added the `ActiveScriptErrorFormatter` class

## v1.0.5 - August 29, 2012
 * [JavaScript Array Polyfills from TutorialsPoint.com](http://www.tutorialspoint.com/javascript/) was replaced by the Douglas Crockford's [ECMAScript 5 Polyfill](http://nuget.org/packages/ES5)
 * By default using of the ECMAScript 5 Polyfill is disabled

## v1.0.1 - August 27, 2012
 * Added the `JsEngineLoadException` class

## v1.0.0 - August 26, 2012
 * Initial version uploaded