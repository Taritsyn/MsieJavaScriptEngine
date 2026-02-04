using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#if NETFRAMEWORK
using MsieJavaScriptEngine.ActiveScript;
#endif
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.JsRt.Edge;
using MsieJavaScriptEngine.JsRt.Ie;
using MsieJavaScriptEngine.Resources;
using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// .NET-wrapper for working with the Internet Explorer's JS engines
	/// </summary>
	public sealed class MsieJsEngine : IDisposable
	{
		/// <summary>
		/// JS engine
		/// </summary>
		private IInnerJsEngine _jsEngine;

		/// <summary>
		/// Current JS engine mode
		/// </summary>
		private static JsEngineMode _currentMode;

		/// <summary>
		/// Synchronizer of JS engines creation
		/// </summary>
		private static readonly Lock _creationSynchronizer = new Lock();

		/// <summary>
		/// Unique document name manager
		/// </summary>
		private UniqueDocumentNameManager _documentNameManager = new UniqueDocumentNameManager("Script Document");

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private InterlockedStatedFlag _disposedFlag = new InterlockedStatedFlag();

		/// <summary>
		/// Gets a name of JS engine mode
		/// </summary>
		public string Mode
		{
			get { return _jsEngine.Mode; }
		}

		/// <summary>
		/// Gets a value that indicates if the JS engine supports script pre-compilation
		/// </summary>
		public bool SupportsScriptPrecompilation
		{
			get { return _jsEngine.SupportsScriptPrecompilation; }
		}


		/// <summary>
		/// Constructs an instance of MSIE JS engine
		/// </summary>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsEngineLoadException"/>
		public MsieJsEngine()
			: this(new JsEngineSettings())
		{ }

		/// <summary>
		/// Constructs an instance of MSIE JS engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsEngineLoadException"/>
		public MsieJsEngine(JsEngineSettings settings)
		{
			JsEngineMode engineMode = settings.EngineMode;
			JsEngineMode processedEngineMode = engineMode;
			JsEngineSettings processedSettings = settings;

			if (engineMode == JsEngineMode.Auto)
			{
				if (ChakraEdgeJsRtJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraEdgeJsRt;
				}
				else if (ChakraIeJsRtJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraIeJsRt;
				}
#if NETFRAMEWORK
				else if (ChakraActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.ChakraActiveScript;
				}
				else if (ClassicActiveScriptJsEngine.IsSupported())
				{
					processedEngineMode = JsEngineMode.Classic;
				}
#endif
				else
				{
					throw new JsEngineLoadException(
#if !NETFRAMEWORK
						NetCoreStrings.Engine_JsEnginesNotFound
#else
						NetFrameworkStrings.Engine_JsEnginesNotFound
#endif
					);
				}
			}

			if (processedEngineMode != engineMode)
			{
				processedSettings = settings.Clone();
				processedSettings.EngineMode = processedEngineMode;
			}

			lock (_creationSynchronizer)
			{
				JsEngineMode previousMode = _currentMode;

				switch (processedEngineMode)
				{
					case JsEngineMode.ChakraEdgeJsRt:
						if (previousMode != JsEngineMode.ChakraIeJsRt
							&& previousMode != JsEngineMode.ChakraActiveScript)
						{
							_jsEngine = new ChakraEdgeJsRtJsEngine(processedSettings);
						}
						else if (previousMode == JsEngineMode.ChakraIeJsRt)
						{
							throw new JsUsageException(
								string.Format(
									CommonStrings.Usage_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}
						else if (previousMode == JsEngineMode.ChakraActiveScript)
						{
							throw new JsUsageException(
								string.Format(
									CommonStrings.Usage_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraIeJsRt:
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{
							_jsEngine = new ChakraIeJsRtJsEngine(processedSettings);
						}
						else
						{
							throw new JsUsageException(
								string.Format(
									CommonStrings.Usage_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
					case JsEngineMode.ChakraActiveScript:
#if NETFRAMEWORK
						if (previousMode != JsEngineMode.ChakraEdgeJsRt)
						{

							_jsEngine = new ChakraActiveScriptJsEngine(processedSettings);
						}
						else
						{
							throw new JsUsageException(
								string.Format(
									CommonStrings.Usage_JsEnginesConflictInProcess,
									JsEngineModeHelpers.GetModeName(processedEngineMode),
									JsEngineModeHelpers.GetModeName(previousMode)
								)
							);
						}

						break;
#else
						throw new JsUsageException(
								string.Format(NetCoreStrings.Usage_JsEngineModeNotCompatibleWithNetCore, processedEngineMode));
#endif
					case JsEngineMode.Classic:
#if NETFRAMEWORK
						_jsEngine = new ClassicActiveScriptJsEngine(processedSettings);

						break;
#else
						throw new JsUsageException(
								string.Format(NetCoreStrings.Usage_JsEngineModeNotCompatibleWithNetCore, processedEngineMode));
#endif
					default:
						throw new JsUsageException(
							string.Format(CommonStrings.Usage_JsEngineModeNotSupported, processedEngineMode));
				}

				_currentMode = processedEngineMode;
			}
		}


		[MethodImpl((MethodImplOptions)256 /* AggressiveInlining */)]
		private void VerifyNotDisposed()
		{
			if (_disposedFlag.IsSet())
			{
				throw new ObjectDisposedException(ToString());
			}
		}

		/// <summary>
		/// Creates a pre-compiled script from JS code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsException"/>
		public PrecompiledScript Precompile(string code)
		{
			return Precompile(code, string.Empty);
		}

		/// <summary>
		/// Creates a pre-compiled script from JS code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <param name="documentName">Document name</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsException"/>
		public PrecompiledScript Precompile(string code, string documentName)
		{
			VerifyNotDisposed();

			if (code is null)
			{
				throw new ArgumentNullException(
					nameof(code),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(code))
				);
			}

			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(code)),
					nameof(code)
				);
			}

			if (!string.IsNullOrWhiteSpace(documentName)
				&& !ValidationHelpers.CheckDocumentNameFormat(documentName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidDocumentNameFormat, documentName),
					nameof(documentName)
				);
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);

			return _jsEngine.Precompile(code, uniqueDocumentName);
		}

		/// <summary>
		/// Creates a pre-compiled script from JS file
		/// </summary>
		/// <param name="path">Path to the JS file</param>
		/// <param name="encoding">Text encoding</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsException"/>
		public PrecompiledScript PrecompileFile(string path, Encoding encoding = null)
		{
			VerifyNotDisposed();

			if (path is null)
			{
				throw new ArgumentNullException(
					nameof(path),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(path))
				);
			}

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(path)),
					nameof(path)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(path))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidFileNameFormat, path),
					nameof(path)
				);
			}

			string code = Utils.GetFileTextContent(path, encoding);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotPrecompileEmptyFile, path),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(path);

			return _jsEngine.Precompile(code, uniqueDocumentName);
		}

		/// <summary>
		/// Creates a pre-compiled script from embedded JS resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name without the namespace of the specified type</param>
		/// <param name="type">The type, that determines the assembly and whose namespace is used to scope
		/// the resource name</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="NullReferenceException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsException"/>
		public PrecompiledScript PrecompileResource(string resourceName, Type type)
		{
			VerifyNotDisposed();

			if (resourceName is null)
			{
				throw new ArgumentNullException(
					nameof(resourceName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(resourceName))
				);
			}

			if (type is null)
			{
				throw new ArgumentNullException(
					nameof(type),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(type))
				);
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(resourceName)),
					nameof(resourceName)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidResourceNameFormat, resourceName),
					nameof(resourceName)
				);
			}

#if NET40
			Assembly assembly = type.Assembly;
#else
			Assembly assembly = type.GetTypeInfo().Assembly;
#endif
			string nameSpace = type.Namespace;
			string resourceFullName = nameSpace is not null ? nameSpace + "." + resourceName : resourceName;

			string code = Utils.GetResourceAsString(resourceFullName, assembly);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotPrecompileEmptyResource, resourceFullName),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(resourceFullName);

			return _jsEngine.Precompile(code, uniqueDocumentName);
		}

		/// <summary>
		/// Creates a pre-compiled script from embedded JS resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		/// <returns>A pre-compiled script that can be executed by different instances of JS engine</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="NullReferenceException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsException"/>
		public PrecompiledScript PrecompileResource(string resourceName, Assembly assembly)
		{
			VerifyNotDisposed();

			if (resourceName is null)
			{
				throw new ArgumentNullException(
					nameof(resourceName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(resourceName))
				);
			}

			if (assembly is null)
			{
				throw new ArgumentNullException(
					nameof(assembly),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(assembly))
				);
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(resourceName)),
					nameof(resourceName)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidResourceNameFormat, resourceName),
					nameof(resourceName)
				);
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotPrecompileEmptyResource, resourceName),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(resourceName);

			return _jsEngine.Precompile(code, uniqueDocumentName);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public object Evaluate(string expression)
		{
			return Evaluate(expression, string.Empty);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <param name="expression">JS expression</param>
		/// <param name="documentName">Document name</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public object Evaluate(string expression, string documentName)
		{
			VerifyNotDisposed();

			if (expression is null)
			{
				throw new ArgumentNullException(
					nameof(expression),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(expression))
				);
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(expression)),
					nameof(expression)
				);
			}

			if (!string.IsNullOrWhiteSpace(documentName)
				&& !ValidationHelpers.CheckDocumentNameFormat(documentName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidDocumentNameFormat, documentName),
					nameof(documentName)
				);
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);

			return _jsEngine.Evaluate(expression, uniqueDocumentName);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JS expression</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public T Evaluate<T>(string expression)
		{
			return Evaluate<T>(expression, string.Empty);
		}

		/// <summary>
		/// Evaluates an expression
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="expression">JS expression</param>
		/// <param name="documentName">Document name</param>
		/// <returns>Result of the expression</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public T Evaluate<T>(string expression, string documentName)
		{
			VerifyNotDisposed();

			if (expression is null)
			{
				throw new ArgumentNullException(
					nameof(expression),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(expression))
				);
			}

			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(expression)),
					nameof(expression)
				);
			}

			if (!string.IsNullOrWhiteSpace(documentName)
				&& !ValidationHelpers.CheckDocumentNameFormat(documentName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidDocumentNameFormat, documentName),
					nameof(documentName)
				);
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_ReturnValueTypeNotSupported, returnValueType.FullName),
					nameof(T)
				);
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);
			object result = _jsEngine.Evaluate(expression, uniqueDocumentName);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void Execute(string code)
		{
			Execute(code, string.Empty);
		}

		/// <summary>
		/// Executes a code
		/// </summary>
		/// <param name="code">JS code</param>
		/// <param name="documentName">Document name</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void Execute(string code, string documentName)
		{
			VerifyNotDisposed();

			if (code is null)
			{
				throw new ArgumentNullException(
					nameof(code),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(code))
				);
			}

			if (string.IsNullOrWhiteSpace(code))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(code)),
					nameof(code)
				);
			}

			if (!string.IsNullOrWhiteSpace(documentName)
				&& !ValidationHelpers.CheckDocumentNameFormat(documentName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidDocumentNameFormat, documentName),
					nameof(documentName)
				);
			}

			string uniqueDocumentName = _documentNameManager.GetUniqueName(documentName);
			_jsEngine.Execute(code, uniqueDocumentName);
		}

		/// <summary>
		/// Executes a pre-compiled script
		/// </summary>
		/// <param name="precompiledScript">A pre-compiled script that can be executed by different
		/// instances of JS engine</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void Execute(PrecompiledScript precompiledScript)
		{
			VerifyNotDisposed();

			if (precompiledScript is null)
			{
				throw new ArgumentNullException(
					nameof(precompiledScript),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(precompiledScript))
				);
			}

			if (precompiledScript.EngineMode != Mode)
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotExecutePrecompiledScriptForAnotherJsEngineMode,
						precompiledScript.EngineMode),
					_jsEngine.Mode
				);
			}

			_jsEngine.Execute(precompiledScript);
		}

		/// <summary>
		/// Executes a code from JS file
		/// </summary>
		/// <param name="path">Path to the JS file</param>
		/// <param name="encoding">Text encoding</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="FileNotFoundException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void ExecuteFile(string path, Encoding encoding = null)
		{
			VerifyNotDisposed();

			if (path is null)
			{
				throw new ArgumentNullException(
					nameof(path),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(path))
				);
			}

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(path)),
					nameof(path)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(path))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidFileNameFormat, path),
					nameof(path)
				);
			}

			string code = Utils.GetFileTextContent(path, encoding);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotExecuteEmptyFile, path),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(path);

			_jsEngine.Execute(code, uniqueDocumentName);
		}

		/// <summary>
		/// Executes a code from embedded JS resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name without the namespace of the specified type</param>
		/// <param name="type">The type, that determines the assembly and whose namespace is used to scope
		/// the resource name</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="NullReferenceException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void ExecuteResource(string resourceName, Type type)
		{
			VerifyNotDisposed();

			if (resourceName is null)
			{
				throw new ArgumentNullException(
					nameof(resourceName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(resourceName))
				);
			}

			if (type is null)
			{
				throw new ArgumentNullException(
					nameof(type),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(type))
				);
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(resourceName)),
					nameof(resourceName)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidResourceNameFormat, resourceName),
					nameof(resourceName)
				);
			}

#if NET40
			Assembly assembly = type.Assembly;
#else
			Assembly assembly = type.GetTypeInfo().Assembly;
#endif
			string nameSpace = type.Namespace;
			string resourceFullName = nameSpace is not null ? nameSpace + "." + resourceName : resourceName;

			string code = Utils.GetResourceAsString(resourceFullName, assembly);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotExecuteEmptyResource, resourceFullName),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(resourceFullName);

			_jsEngine.Execute(code, uniqueDocumentName);
		}

		/// <summary>
		/// Executes a code from embedded JS resource
		/// </summary>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="NullReferenceException"/>
		/// <exception cref="JsUsageException"/>
		/// <exception cref="JsCompilationException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void ExecuteResource(string resourceName, Assembly assembly)
		{
			VerifyNotDisposed();

			if (resourceName is null)
			{
				throw new ArgumentNullException(
					nameof(resourceName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(resourceName))
				);
			}

			if (assembly is null)
			{
				throw new ArgumentNullException(
					nameof(assembly),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(assembly))
				);
			}

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(resourceName)),
					nameof(resourceName)
				);
			}

			if (!ValidationHelpers.CheckDocumentNameFormat(resourceName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidResourceNameFormat, resourceName),
					nameof(resourceName)
				);
			}

			string code = Utils.GetResourceAsString(resourceName, assembly);
			if (string.IsNullOrWhiteSpace(code))
			{
				throw new JsUsageException(
					string.Format(CommonStrings.Usage_CannotExecuteEmptyResource, resourceName),
					_jsEngine.Mode
				);
			}
			string uniqueDocumentName = _documentNameManager.GetUniqueName(resourceName);

			_jsEngine.Execute(code, uniqueDocumentName);
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public object CallFunction(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (functionName is null)
			{
				throw new ArgumentNullException(
					nameof(functionName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(functionName))
				);
			}

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(functionName)),
					nameof(functionName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidFunctionNameFormat, functionName),
					nameof(functionName)
				);
			}

			int argumentCount = args.Length;
			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object argument = args[argumentIndex];

					if (argument is not null)
					{
						Type argType = argument.GetType();

						if (!ValidationHelpers.IsSupportedType(argType))
						{
							throw new ArgumentException(
								string.Format(CommonStrings.Usage_FunctionParameterTypeNotSupported,
									functionName, argType.FullName),
								nameof(args)
							);
						}
					}
				}
			}

			object result = _jsEngine.CallFunction(functionName, args);

			return result;
		}

		/// <summary>
		/// Calls a function
		/// </summary>
		/// <typeparam name="T">Type of function result</typeparam>
		/// <param name="functionName">Function name</param>
		/// <param name="args">Function arguments</param>
		/// <returns>Result of the function execution</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsInterruptedException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public T CallFunction<T>(string functionName, params object[] args)
		{
			VerifyNotDisposed();

			if (functionName is null)
			{
				throw new ArgumentNullException(
					nameof(functionName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(functionName))
				);
			}

			if (string.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(functionName)),
					nameof(functionName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(functionName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidFunctionNameFormat, functionName),
					nameof(functionName)
				);
			}

			int argumentCount = args.Length;
			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					object argument = args[argumentIndex];

					if (argument is not null)
					{
						Type argType = argument.GetType();

						if (!ValidationHelpers.IsSupportedType(argType))
						{
							throw new ArgumentException(
								string.Format(CommonStrings.Usage_FunctionParameterTypeNotSupported,
									functionName, argType.FullName),
								nameof(args)
							);
						}
					}
				}
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_ReturnValueTypeNotSupported, returnValueType.FullName),
					nameof(T)
				);
			}

			object result = _jsEngine.CallFunction(functionName, args);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Checks for the existence of a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Result of check (<c>true</c> - exists; <c>false</c> - not exists</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public bool HasVariable(string variableName)
		{
			VerifyNotDisposed();

			if (variableName is null)
			{
				throw new ArgumentNullException(
					nameof(variableName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(variableName))
				);
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(variableName)),
					nameof(variableName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidVariableNameFormat, variableName),
					nameof(variableName)
				);
			}

			return _jsEngine.HasVariable(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public object GetVariableValue(string variableName)
		{
			VerifyNotDisposed();

			if (variableName is null)
			{
				throw new ArgumentNullException(
					nameof(variableName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(variableName))
				);
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(variableName)),
					nameof(variableName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidVariableNameFormat, variableName),
					nameof(variableName)
				);
			}

			return _jsEngine.GetVariableValue(variableName);
		}

		/// <summary>
		/// Gets a value of variable
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="variableName">Name of variable</param>
		/// <returns>Value of variable</returns>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public T GetVariableValue<T>(string variableName)
		{
			VerifyNotDisposed();

			if (variableName is null)
			{
				throw new ArgumentNullException(
					nameof(variableName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(variableName))
				);
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(variableName)),
					nameof(variableName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidVariableNameFormat, variableName),
					nameof(variableName)
				);
			}

			Type returnValueType = typeof(T);
			if (!ValidationHelpers.IsSupportedType(returnValueType))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_ReturnValueTypeNotSupported, returnValueType.FullName),
					nameof(T)
				);
			}

			object result = _jsEngine.GetVariableValue(variableName);

			return TypeConverter.ConvertToType<T>(result);
		}

		/// <summary>
		/// Sets a value of variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="value">Value of variable</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void SetVariableValue(string variableName, object value)
		{
			VerifyNotDisposed();

			if (variableName is null)
			{
				throw new ArgumentNullException(
					nameof(variableName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(variableName))
				);
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(variableName)),
					nameof(variableName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidVariableNameFormat, variableName),
					nameof(variableName)
				);
			}

			if (value is not null)
			{
				Type variableType = value.GetType();

				if (!ValidationHelpers.IsSupportedType(variableType))
				{
					throw new ArgumentException(
						string.Format(CommonStrings.Usage_VariableTypeNotSupported,
							variableName, variableType.FullName),
						nameof(value)
					);
				}
			}

			_jsEngine.SetVariableValue(variableName, value);
		}

		/// <summary>
		/// Removes a variable
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsRuntimeException"/>
		/// <exception cref="JsException"/>
		public void RemoveVariable(string variableName)
		{
			VerifyNotDisposed();

			if (variableName is null)
			{
				throw new ArgumentNullException(
					nameof(variableName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(variableName))
				);
			}

			if (string.IsNullOrWhiteSpace(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(variableName)),
					nameof(variableName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(variableName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidVariableNameFormat, variableName),
					nameof(variableName)
				);
			}

			_jsEngine.RemoveVariable(variableName);
		}

		/// <summary>
		/// Embeds a host object to script code
		/// </summary>
		/// <remarks>
		/// Allows to embed instances of simple classes (or structures) and delegates.
		/// </remarks>
		/// <param name="itemName">The name for the new global variable or function that will represent the object</param>
		/// <param name="value">The object to expose</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsException"/>
		public void EmbedHostObject(string itemName, object value)
		{
			VerifyNotDisposed();

			if (itemName is null)
			{
				throw new ArgumentNullException(
					nameof(itemName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(itemName))
				);
			}

			if (value is null)
			{
				throw new ArgumentNullException(
					nameof(value),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(value))
				);
			}

			if (string.IsNullOrWhiteSpace(itemName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(itemName)),
					nameof(itemName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidScriptItemNameFormat, itemName),
					nameof(itemName)
				);
			}

			Type itemType = value.GetType();

			if (ValidationHelpers.IsPrimitiveType(itemType) || itemType == typeof(Undefined))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_EmbeddedHostObjectTypeNotSupported,
						itemName, itemType.FullName),
					nameof(value)
				);
			}

			_jsEngine.EmbedHostObject(itemName, value);
		}

		/// <summary>
		/// Embeds a host type to script code
		/// </summary>
		/// <remarks>
		/// Host types are exposed to script code in the form of objects whose properties and
		/// methods are bound to the type's static members.
		/// </remarks>
		/// <param name="itemName">The name for the new global variable that will represent the type</param>
		/// <param name="type">The type to expose</param>
		/// <exception cref="ObjectDisposedException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="JsException"/>
		public void EmbedHostType(string itemName, Type type)
		{
			VerifyNotDisposed();

			if (itemName is null)
			{
				throw new ArgumentNullException(
					nameof(itemName),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(itemName))
				);
			}

			if (type is null)
			{
				throw new ArgumentNullException(
					nameof(type),
					string.Format(CommonStrings.Common_ArgumentIsNull, nameof(type))
				);
			}

			if (string.IsNullOrWhiteSpace(itemName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Common_ArgumentIsEmpty, nameof(itemName)),
					nameof(itemName)
				);
			}

			if (!ValidationHelpers.CheckNameFormat(itemName))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_InvalidScriptItemNameFormat, itemName),
					nameof(itemName)
				);
			}

			if (ValidationHelpers.IsPrimitiveType(type) || type == typeof(Undefined))
			{
				throw new ArgumentException(
					string.Format(CommonStrings.Usage_EmbeddedHostTypeNotSupported, type.FullName),
					nameof(type)
				);
			}

			_jsEngine.EmbedHostType(itemName, type);
		}

		/// <summary>
		/// Interrupts script execution and causes the JS engine to throw an exception
		/// </summary>
		public void Interrupt()
		{
			VerifyNotDisposed();

			_jsEngine.Interrupt();
		}

		/// <summary>
		/// Performs a full garbage collection
		/// </summary>
		public void CollectGarbage()
		{
			VerifyNotDisposed();

			_jsEngine.CollectGarbage();
		}

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			if (_disposedFlag.Set())
			{
				if (_jsEngine is not null)
				{
					_jsEngine.Dispose();
					_jsEngine = null;
				}

				_documentNameManager = null;
			}
		}

		#endregion
	}
}