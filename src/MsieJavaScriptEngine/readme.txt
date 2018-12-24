

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v3.0.0

   --------------------------------------------------------------------------------

           Copyright (c) 2012-2018 Andrey Taritsyn - http://www.taritsyn.ru


   ===========
   DESCRIPTION
   ===========
   This library is a .NET wrapper for working with the JavaScript engines of
   Internet Explorer and Edge (JsRT versions of Chakra, ActiveScript version of
   Chakra and Classic JavaScript Engine). Project was based on the code of
   SassAndCoffee.JavaScript (http://github.com/paulcbetts/SassAndCoffee),
   Chakra Sample Hosts (http://github.com/panopticoncentral/chakra-host) and
   jsrt-dotnet (http://github.com/robpaveza/jsrt-dotnet).

   =============
   RELEASE NOTES
   =============
   1.  Format of the error messages was unified;
   2.  Created a new exception classes: `JsCompilationException`,
       `JsEngineException`, `JsFatalException`, `JsInterruptedException`,
       `JsScriptException` and `JsUsageException`. These exceptions are responsible
       for handling errors, some of which were previously handled by the
       `JsRuntimeException` class;
   3.  In the `JsException` class was added two new properties: `Category` and
       `Description`;
   4.  From the `JsRuntimeException` class was removed one property - `ErrorCode`;
   5.  In the `JsRuntimeException` class was added three new properties: `Type`,
       `DocumentName` and `CallStack`;
   6.  `JsEngineLoadException` class now is inherited from the `JsEngineException`
       class;
   7.  `Format` method of the `JsErrorHelpers` class was renamed to the
       `GenerateErrorDetails`;
   8.  One part of the auxiliary code was removed, and other part moved to an
       external library - AdvancedStringBuilder;
   9.  Added a ability to interrupt execution of the script;
   10. In JsRT modes added a ability to pre-compile scripts;
   11. In `MsieJsEngine` class was added `SupportsScriptPrecompilation` property
       and four new methods: `Interrupt`, `Precompile`, `PrecompileFile` and
       `PrecompileResource`;
   12. In JavaScript engine settings was added one new property - `MaxStackSize`
       (default `492` or `984` KB);
   13. Added support of .NET Standard 2.0 (only supported `ChakraIeJsRt` and
       `ChakraEdgeJsRt` modes).

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine