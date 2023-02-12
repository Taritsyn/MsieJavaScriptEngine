﻿using System;

using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// JS engine settings
	/// </summary>
	public sealed class JsEngineSettings
	{
#if !NETSTANDARD1_3
		/// <summary>
		/// The stack size is sufficient to run the code of modern JavaScript libraries in 32-bit process
		/// </summary>
		const int STACK_SIZE_32 = 492 * 1024; // like 32-bit Node.js

		/// <summary>
		/// The stack size is sufficient to run the code of modern JavaScript libraries in 64-bit process
		/// </summary>
		const int STACK_SIZE_64 = 984 * 1024; // like 64-bit Node.js

		/// <summary>
		/// The maximum stack size in bytes
		/// </summary>
		private int _maxStackSize;

#endif
		/// <summary>
		/// Gets or sets a flag for whether to enable script debugging features
		/// </summary>
		public bool EnableDebugging
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a JS engine mode
		/// </summary>
		public JsEngineMode EngineMode
		{
			get;
			set;
		}
#if !NETSTANDARD1_3

		/// <summary>
		/// Gets or sets a maximum stack size in bytes
		/// </summary>
		/// <remarks>
		/// Set a <c>0</c> to use the default maximum stack size specified in the header
		/// for the executable.
		/// </remarks>
		public int MaxStackSize
		{
			get { return _maxStackSize; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(
						nameof(value),
						CommonStrings.Engine_MaxStackSizeMustBeNonNegative
					);
				}

				_maxStackSize = value;
			}
		}
#endif

		/// <summary>
		/// Gets or sets a flag for whether to use the ECMAScript 5 Polyfill
		/// </summary>
		public bool UseEcmaScript5Polyfill
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to use the JSON2 library
		/// </summary>
		public bool UseJson2Library
		{
			get;
			set;
		}


		/// <summary>
		/// Constructs an instance of JS engine settings
		/// </summary>
		public JsEngineSettings()
		{
			EnableDebugging = false;
			EngineMode = JsEngineMode.Auto;
#if !NETSTANDARD1_3
			MaxStackSize = Utils.Is64BitProcess() ? STACK_SIZE_64 : STACK_SIZE_32;
#endif
			UseEcmaScript5Polyfill = false;
			UseJson2Library = false;
		}


		/// <summary>
		/// Creates a new object that is a copy of the current instance
		/// </summary>
		/// <returns>A new object that is a copy of this instance</returns>
		public JsEngineSettings Clone()
		{
			return (JsEngineSettings)MemberwiseClone();
		}
	}
}