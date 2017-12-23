

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v2.2.5

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
   1. Removed a redundant code;
   2. Fixed a error, that occurred in the `Classic` mode during removing the
      embedded host objects and types;
   3. Fixed a error, that occurred during finding the suitable method overload,
      that receives numeric values and interfaces as parameters, of the host object.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine