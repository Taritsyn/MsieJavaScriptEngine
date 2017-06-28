

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v2.2.1

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
   1. In JsRT modes fixed a problems in calculation of error locations;
   2. An attempt was made to prevent occurrence of the access violation exception;
   3. Now the original exception is added to instance of the `JsRuntimeException`
      class as an inner exception;
   4. An attempt was made to prevent a blocking of finalizer's thread;
   5. Added support of identifier names compliant with ECMAScript 5.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine