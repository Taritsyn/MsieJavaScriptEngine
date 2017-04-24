

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v2.2.0

   --------------------------------------------------------------------------------

           Copyright (c) 2012-2017 Andrey Taritsyn - http://www.taritsyn.ru


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
   1. Added support of .NET Core 1.0.4;
   2. In JsRT modes now script error contains a full stack trace;
   3. In `MsieJsEngine` class was added overloaded versions of the `Evaluate`,
      `Evaluate<T>` and `Execute` methods, which take the document name as second
      parameter;
   4. Now all modes support the possibility to debug in Visual Studio by adding the
      `debugger` statement to script code;
   5. Now during debugging in ActiveScript modes the script error contains a error
      location.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine