

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v3.0.1

   --------------------------------------------------------------------------------

           Copyright (c) 2012-2019 Andrey Taritsyn - http://www.taritsyn.ru


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
   1. Fixed a error, that occurred in the `Classic` mode during calling an embedded
      delegate, which does not return a result;
   2. Fixed a error, that occurred during setting a value to field of embedded
      type;
   3. Improved a performance of the embedding of objects and types;
   4. Accelerated a conversion of script types to host types.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine