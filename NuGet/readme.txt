

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v2.0.2

   --------------------------------------------------------------------------------

           Copyright (c) 2012-2016 Andrey Taritsyn - http://www.taritsyn.ru


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
   1. Now when you call the overloaded version of the `ExecuteResource` method, that
      takes the type, need to pass the resource name without the namespace;
   2. Fixed a error “Out of stack space”.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine