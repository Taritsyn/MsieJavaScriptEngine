

   --------------------------------------------------------------------------------
             README file for MSIE JavaScript Engine for .NET v3.0.0 Beta 1

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
   1. Format of the error messages was unified;
   2. Created a new exception classes: `JsCompilationException`,
      `JsEngineException`, `JsFatalException` and `JsUsageException`. These
      exceptions are responsible for handling errors, some of which were previously
      handled by the `JsRuntimeException` class;
   3. In the `JsException` class was added two new properties: `Category` and
      `Description`;
   4. From the `JsRuntimeException` class was removed one property - `ErrorCode`;
   5. In the `JsRuntimeException` class was added three new properties: `Type`,
      `DocumentName` and `CallStack`;
   6. `JsScriptInterruptedException` class was renamed to the
      `JsInterruptedException` class and now is inherited from the
      `JsRuntimeException` class;
   7. `JsEngineLoadException` class now is inherited from the `JsEngineException`
      class;
   8. `Format` method of the `JsErrorHelpers` class was renamed to the
      `GenerateErrorDetails`.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine