

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v2.1.0

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
   1. Added support of .NET Core 1.0.3;
   2. Downgraded .NET Framework version from 4.5.1 to 4.5;
   3. Now when you call the overloaded version of the `ExecuteResource` method,
      that takes the type, need to pass the resource name without the namespace;
   4. Fixed a error “Out of stack space”;
   5. JSON2 library was updated to version of October 28, 2016.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine