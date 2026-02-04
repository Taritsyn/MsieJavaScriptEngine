

   --------------------------------------------------------------------------------
                README file for MSIE JavaScript Engine for .NET v3.2.5

   --------------------------------------------------------------------------------

           Copyright (c) 2012-2024 Andrey Taritsyn - http://www.taritsyn.ru


   ===========
   DESCRIPTION
   ===========
   This library is a .NET wrapper for working with the JavaScript engines of
   Internet Explorer and Edge Legacy (JsRT versions of Chakra, ActiveScript version
   of Chakra and Classic JavaScript Engine). Project was based on the code of
   SassAndCoffee.JavaScript (https://github.com/anaisbetts/SassAndCoffee),
   Chakra Sample Hosts (https://github.com/panopticoncentral/chakra-host) and
   jsrt-dotnet (https://github.com/robpaveza/jsrt-dotnet).

   =============
   RELEASE NOTES
   =============
   1. Optimized a memory usage in the `ReflectionHelpers.GetBestFitMethod` method;
   2. Added support for .NET Standard 2.1 and .NET 10;
   3. Performed a migration to the modern C# null/not-null checks;
   4. In the `lock` statements for .NET 10 target now uses a instances of the
      `System.Threading.Lock` class.

   ============
   PROJECT SITE
   ============
   https://github.com/Taritsyn/MsieJavaScriptEngine