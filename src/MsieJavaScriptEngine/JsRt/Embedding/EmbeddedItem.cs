#if NETSTANDARD
using System;
using System.Collections.Generic;

using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.JsRt.Embedding
{
	/// <summary>
	/// Embedded item
	/// </summary>
	/// <typeparam name="TValue">The type of the JavaScript value</typeparam>
	/// <typeparam name="TFunction">The type of the native function</typeparam>
	internal abstract class EmbeddedItem<TValue, TFunction> : IDisposable
		where TValue : struct
		where TFunction : Delegate
	{
		/// <summary>
		/// Host type
		/// </summary>
		private Type _hostType;

		/// <summary>
		/// Instance of host type
		/// </summary>
		private object _hostObject;

		/// <summary>
		/// JavaScript value created from an host item
		/// </summary>
		private readonly TValue _scriptValue;

		/// <summary>
		/// List of native functions, that used to access to members of host item
		/// </summary>
		private IList<TFunction> _nativeFunctions;

		/// <summary>
		/// Flag indicating whether this object is disposed
		/// </summary>
		private readonly InterlockedStatedFlag _disposedFlag = new InterlockedStatedFlag();

		/// <summary>
		/// Gets a host type
		/// </summary>
		public Type HostType
		{
			get { return _hostType; }
		}

		/// <summary>
		/// Gets a instance of host type
		/// </summary>
		public object HostObject
		{
			get { return _hostObject; }
		}

		/// <summary>
		/// Gets a JavaScript value created from an host item
		/// </summary>
		public TValue ScriptValue
		{
			get { return _scriptValue; }
		}

		/// <summary>
		/// Gets a list of native functions, that used to access to members of host item
		/// </summary>
		public IList<TFunction> NativeFunctions
		{
			get { return _nativeFunctions; }
		}

		/// <summary>
		/// Gets a value that indicates if the host item is an instance
		/// </summary>
		public abstract bool IsInstance
		{
			get;
		}


		/// <summary>
		/// Constructs an instance of the embedded item
		/// </summary>
		/// <param name="hostType">Host type</param>
		/// <param name="hostObject">Instance of host type</param>
		/// <param name="scriptValue">JavaScript value created from an host item</param>
		/// <param name="nativeFunctions">List of native functions, that used to access to members of host item</param>
		protected EmbeddedItem(Type hostType, object hostObject, TValue scriptValue,
			IList<TFunction> nativeFunctions)
		{
			_hostType = hostType;
			_hostObject = hostObject;
			_scriptValue = scriptValue;
			_nativeFunctions = nativeFunctions;
		}


		#region IDisposable implementation

		/// <summary>
		/// Disposes the embedded item
		/// </summary>
		public void Dispose()
		{
			if (_disposedFlag.Set())
			{
				_hostType = null;
				_hostObject = null;

				IList<TFunction> nativeFunctions = _nativeFunctions;
				if (nativeFunctions != null)
				{
					nativeFunctions.Clear();
					_nativeFunctions = null;
				}
			}
		}

		#endregion
	}
}
#endif