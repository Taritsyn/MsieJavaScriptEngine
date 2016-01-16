

   ----------------------------------------------------------------------
       README file for MSIE JavaScript Engine for .NET v1.7.0 Alpha 2

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
   1. Added a possibility to debug in Visual Studio by adding the
      `debugger` statement to script code. This feature only works in the
      `ChakraIeJsRt` and `ChakraEdgeJsRt` modes.
   2. In JavaScript engine settings was added one new property -
      `EnableDebugging` (default `false`).

   ============
   PROJECT SITE
   ============
   http://github.com/Taritsyn/MsieJavaScriptEngine