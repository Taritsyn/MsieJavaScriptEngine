

   ----------------------------------------------------------------------
           README file for MSIE JavaScript Engine for .NET v1.7.0

   ----------------------------------------------------------------------

      Copyright (c) 2012-2016 Andrey Taritsyn - http://www.taritsyn.ru


   ===========
   DESCRIPTION
   ===========
   This library is a .NET wrapper for working with the JavaScript engines
   of Internet Explorer and Edge (JsRT versions of Chakra, ActiveScript
   version of Chakra and Classic JavaScript Engine).
   Project was based on the code of SassAndCoffee.JavaScript
   (http://github.com/paulcbetts/SassAndCoffee) and Chakra Sample Hosts
   (http://github.com/panopticoncentral/chakra-host).

   =============
   RELEASE NOTES
   =============
   1. Added the `EmbedHostObject` method (embeds a instance of simple
      class, structure or delegate to script code);
   2. Added the `EmbedHostType` method (embeds a host type to script
      code);
   3. Added a possibility to debug in Visual Studio by adding the
      `debugger` statement to script code. This feature only works in the
      `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
   4. In JavaScript engine settings was added one new property -
      `EnableDebugging` (default `false`);
   5. Improved implementation of the `CallFunction` method for Chakra
      JsRT modes.

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine