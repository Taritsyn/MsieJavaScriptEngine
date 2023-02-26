using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.JsRt.Embedding;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine.JsRt
{
	/// <summary>
	/// Type mapper
	/// </summary>
	/// <typeparam name="TValue">The type of the JavaScript value</typeparam>
	/// <typeparam name="TFunction">The type of the native function</typeparam>
	internal abstract class TypeMapper<TValue, TFunction> : IDisposable
		where TValue : struct
		where TFunction : Delegate
	{
		/// <summary>
		/// Name of property to store the external object
		/// </summary>
		protected const string ExternalObjectPropertyName = "_MsieJavaScriptEngine_externalObject";

		/// <summary>
		/// Flag for whether to allow the usage of reflection API in the script code
		/// </summary>
		protected readonly bool _allowReflection;

		/// <summary>
		/// Storage for lazy-initialized embedded objects
		/// </summary>
		private ConcurrentDictionary<EmbeddedObjectKey, Lazy<EmbeddedObject<TValue, TFunction>>> _lazyEmbeddedObjects;

		/// <summary>
		/// Callback for finalization of embedded object
		/// </summary>
		protected JsFinalizeCallback _embeddedObjectFinalizeCallback;

		/// <summary>
		/// Synchronizer of embedded object storage's initialization
		/// </summary>
		private readonly object _embeddedObjectStorageInitializationSynchronizer = new object();

		/// <summary>
		/// Flag indicating whether the embedded object storage is initialized
		/// </summary>
		private bool _embeddedObjectStorageInitialized;

		/// <summary>
		/// Storage for lazy-initialized embedded types
		/// </summary>
		private ConcurrentDictionary<string, Lazy<EmbeddedType<TValue, TFunction>>> _lazyEmbeddedTypes;

		/// <summary>
		/// Callback for finalization of embedded type
		/// </summary>
		protected JsFinalizeCallback _embeddedTypeFinalizeCallback;

		/// <summary>
		/// Synchronizer of embedded type storage's initialization
		/// </summary>
		private readonly object _embeddedTypeStorageInitializationSynchronizer = new object();

		/// <summary>
		/// Flag indicating whether the embedded type storage is initialized
		/// </summary>
		private bool _embeddedTypeStorageInitialized;

		/// <summary>
		/// Flag indicating whether this object is disposed
		/// </summary>
		private InterlockedStatedFlag _disposedFlag = new InterlockedStatedFlag();


		/// <summary>
		/// Constructs an instance of the type mapper
		/// </summary>
		/// <param name="allowReflection">Flag for whether to allow the usage of reflection API in the script code</param>
		protected TypeMapper(bool allowReflection)
		{
			_allowReflection = allowReflection;
		}


		/// <summary>
		/// Creates a JavaScript value from an host object if the it does not already exist
		/// </summary>
		/// <param name="obj">Instance of host type</param>
		/// <returns>JavaScript value created from an host object</returns>
		public virtual TValue GetOrCreateScriptObject(object obj)
		{

			if (!_embeddedObjectStorageInitialized)
			{
				lock (_embeddedObjectStorageInitializationSynchronizer)
				{
					if (!_embeddedObjectStorageInitialized)
					{
						_lazyEmbeddedObjects = new ConcurrentDictionary<EmbeddedObjectKey,
							Lazy<EmbeddedObject<TValue, TFunction>>>();
						_embeddedObjectFinalizeCallback = EmbeddedObjectFinalizeCallback;

						_embeddedObjectStorageInitialized = true;
					}
				}
			}

			var embeddedObjectKey = new EmbeddedObjectKey(obj);
			EmbeddedObject<TValue, TFunction> embeddedObject = _lazyEmbeddedObjects.GetOrAdd(
				embeddedObjectKey,
				key => new Lazy<EmbeddedObject<TValue, TFunction>>(() => CreateEmbeddedObjectOrFunction(obj))
			).Value;

			return embeddedObject.ScriptValue;
		}

		/// <summary>
		/// Creates a JavaScript value from an host type if the it does not already exist
		/// </summary>
		/// <param name="type">Host type</param>
		/// <returns>JavaScript value created from an host type</returns>
		public virtual TValue GetOrCreateScriptType(Type type)
		{
			if (!_embeddedTypeStorageInitialized)
			{
				lock (_embeddedTypeStorageInitializationSynchronizer)
				{
					if (!_embeddedTypeStorageInitialized)
					{
						_lazyEmbeddedTypes = new ConcurrentDictionary<string,
							Lazy<EmbeddedType<TValue, TFunction>>>();
						_embeddedTypeFinalizeCallback = EmbeddedTypeFinalizeCallback;

						_embeddedTypeStorageInitialized = true;
					}
				}
			}

			string embeddedTypeKey = type.AssemblyQualifiedName;
			EmbeddedType<TValue, TFunction> embeddedType = _lazyEmbeddedTypes.GetOrAdd(
				embeddedTypeKey,
				key => new Lazy<EmbeddedType<TValue, TFunction>>(() => CreateEmbeddedType(type))
			).Value;

			return embeddedType.ScriptValue;
		}

		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public abstract TValue MapToScriptType(object value);

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		public abstract object MapToHostType(TValue value);

		protected abstract EmbeddedObject<TValue, TFunction> CreateEmbeddedObjectOrFunction(object obj);

		private void EmbeddedObjectFinalizeCallback(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return;
			}

			GCHandle objHandle = GCHandle.FromIntPtr(ptr);
			object obj = objHandle.Target;
			var lazyEmbeddedObjects = _lazyEmbeddedObjects;

			if (obj != null && lazyEmbeddedObjects != null)
			{
				var embeddedObjectKey = new EmbeddedObjectKey(obj);
				Lazy<EmbeddedObject<TValue, TFunction>> lazyEmbeddedObject;

				if (lazyEmbeddedObjects.TryRemove(embeddedObjectKey, out lazyEmbeddedObject))
				{
					lazyEmbeddedObject.Value?.Dispose();
				}
			}

			objHandle.Free();
		}

		protected abstract EmbeddedType<TValue, TFunction> CreateEmbeddedType(Type type);

		private void EmbeddedTypeFinalizeCallback(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return;
			}

			GCHandle embeddedTypeHandle = GCHandle.FromIntPtr(ptr);
			var type = (Type)embeddedTypeHandle.Target;
			string embeddedTypeKey = type.AssemblyQualifiedName;
			var lazyEmbeddedTypes = _lazyEmbeddedTypes;

			if (!string.IsNullOrEmpty(embeddedTypeKey) && lazyEmbeddedTypes != null)
			{
				Lazy<EmbeddedType<TValue, TFunction>> lazyEmbeddedType;

				if (lazyEmbeddedTypes.TryRemove(embeddedTypeKey, out lazyEmbeddedType))
				{
					lazyEmbeddedType.Value?.Dispose();
				}
			}

			embeddedTypeHandle.Free();
		}

		protected bool IsAvailableProperty(PropertyInfo property)
		{
			if (_allowReflection)
			{
				return true;
			}

			bool isAvailable = ReflectionHelpers.IsAllowedProperty(property);

			return isAvailable;
		}

		protected IEnumerable<IGrouping<string, MethodInfo>> GetAvailableMethodGroups(MethodInfo[] methods)
		{
			IEnumerable<MethodInfo> availableMethods = methods.Where(ReflectionHelpers.IsFullyFledgedMethod);
			if (!_allowReflection)
			{
				availableMethods = availableMethods.Where(ReflectionHelpers.IsAllowedMethod);
			}
			IEnumerable<IGrouping<string, MethodInfo>> availableMethodGroups = availableMethods.GroupBy(m => m.Name);

			return availableMethodGroups;
		}

		protected object[] GetHostItemMemberArguments(TValue[] args, int maxArgCount = -1)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			int argCount = args.Length;
			const int skippedArgCount = 1;
			int processedArgCount = argCount >= skippedArgCount ? argCount - skippedArgCount : 0;
			if (maxArgCount >= 0 && processedArgCount > maxArgCount)
			{
				processedArgCount = maxArgCount;
			}

			object[] processedArgs;
			if (processedArgCount > 0)
			{
				processedArgs = args
					.Skip(skippedArgCount)
					.Take(processedArgCount)
					.Select(MapToHostType)
					.ToArray()
					;
			}
			else
			{
				processedArgs = new object[0];
			}

			return processedArgs;
		}

		protected static Exception UnwrapException(Exception exception)
		{
			Exception originalException = exception;
			var targetInvocationException = exception as TargetInvocationException;

			if (targetInvocationException != null)
			{
				Exception innerException = targetInvocationException.InnerException;
				if (innerException != null)
				{
					originalException = innerException;
				}
			}

			return originalException;
		}

		#region IDisposable implementation

		/// <summary>
		/// Disposes a type mapper
		/// </summary>
		public virtual void Dispose()
		{
			if (_disposedFlag.Set())
			{
				var lazyEmbeddedObjects = _lazyEmbeddedObjects;
				if (lazyEmbeddedObjects != null)
				{
					if (lazyEmbeddedObjects.Count > 0)
					{
						foreach (EmbeddedObjectKey key in lazyEmbeddedObjects.Keys)
						{
							Lazy<EmbeddedObject<TValue, TFunction>> lazyEmbeddedObject;

							if (lazyEmbeddedObjects.TryGetValue(key, out lazyEmbeddedObject))
							{
								lazyEmbeddedObject.Value?.Dispose();
							}
						}

						lazyEmbeddedObjects.Clear();
					}

					_lazyEmbeddedObjects = null;
				}

				_embeddedObjectFinalizeCallback = null;

				var lazyEmbeddedTypes = _lazyEmbeddedTypes;
				if (lazyEmbeddedTypes != null)
				{
					if (lazyEmbeddedTypes.Count > 0)
					{
						foreach (string key in lazyEmbeddedTypes.Keys)
						{
							Lazy<EmbeddedType<TValue, TFunction>> lazyEmbeddedType;

							if (lazyEmbeddedTypes.TryGetValue(key, out lazyEmbeddedType))
							{
								lazyEmbeddedType.Value?.Dispose();
							}
						}

						lazyEmbeddedTypes.Clear();
					}

					_lazyEmbeddedTypes = null;
				}

				_embeddedTypeFinalizeCallback = null;
			}
		}

		#endregion
	}
}