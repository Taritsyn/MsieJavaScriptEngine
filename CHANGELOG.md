Change log
==========

## March 24, 2014 - v1.4.2
 * Fixed [JavaScriptEngineSwitcher.Msie's bug #5](http://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/5) "MSIE "Catastrophic failure" when disposing"

## March 22, 2014 - v1.4.1
 * Fixed minor bugs

## February 27, 2014 - v1.4.0
 * Removed following methods: `HasProperty`, `GetPropertyValue`, `SetPropertyValue` and `RemoveProperty`
 * Fixed [bug #3](http://github.com/Taritsyn/MsieJavaScriptEngine/issues/3) "execute code from different threads"
 * Now in the `ChakraJsRt` mode is available a more detailed information about errors
 * In ECMAScript 5 Polyfill improved a performance of the `String.prototype.trim` function
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