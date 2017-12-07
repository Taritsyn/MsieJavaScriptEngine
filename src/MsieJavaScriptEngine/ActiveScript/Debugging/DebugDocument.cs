#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MsieJavaScriptEngine.ActiveScript.Debugging
{
	/// <summary>
	/// Debug document
	/// </summary>
	internal sealed class DebugDocument : IDebugDocumentInfo, IDebugDocumentProvider, IDebugDocument,
		IDebugDocumentText
	{
		/// <summary>
		/// Regular expression for working with a line break
		/// </summary>
		private static readonly Regex _lineBreakRegex = new Regex("\r\n|\n|\r");

		/// <summary>
		/// Active Script wrapper
		/// </summary>
		private readonly IActiveScriptWrapper _activeScriptWrapper;

		/// <summary>
		/// Wrapper for debug application
		/// </summary>
		private readonly DebugApplicationWrapper _debugApplicationWrapper;

		/// <summary>
		/// Source context
		/// </summary>
		private readonly UIntPtr _sourceContext;

		/// <summary>
		/// Document name
		/// </summary>
		private readonly string _name;

		/// <summary>
		/// Script text
		/// </summary>
		private readonly string _code;

		/// <summary>
		/// List of source code lines
		/// </summary>
		private readonly List<DebugLineInfo> _lines = new List<DebugLineInfo>();

		/// <summary>
		/// Debug application node
		/// </summary>
		private IDebugApplicationNode _node;

		/// <summary>
		/// Gets a script text
		/// </summary>
		public string Code
		{
			get { return _code; }
		}


		/// <summary>
		/// Constructs an instance of the debug document
		/// </summary>
		/// <param name="activeScriptWrapper">Active Script wrapper</param>
		/// <param name="debugApplicationWrapper">Wrapper for debug application</param>
		/// <param name="sourceContext">Source context</param>
		/// <param name="name">Document name</param>
		/// <param name="code">Script text</param>
		public DebugDocument(IActiveScriptWrapper activeScriptWrapper,
			DebugApplicationWrapper debugApplicationWrapper, UIntPtr sourceContext, string name,
			string code)
		{
			_activeScriptWrapper = activeScriptWrapper;
			_debugApplicationWrapper = debugApplicationWrapper;
			_sourceContext = sourceContext;
			_name = name;
			_code = code;

			Initialize();
		}


		/// <summary>
		/// Finds a line break
		/// </summary>
		/// <param name="sourceCode">Source code</param>
		/// <param name="startPosition">Position in the input string that defines the leftmost
		/// position to be searched</param>
		/// <param name="length">Number of characters in the substring to include in the search</param>
		/// <param name="lineBreakPosition">Position of line break</param>
		/// <param name="lineBreakLength">Length of line break</param>
		private static void FindLineBreak(string sourceCode, int startPosition, int length,
			out int lineBreakPosition, out int lineBreakLength)
		{
			Match lineBreakMatch = _lineBreakRegex.Match(sourceCode, startPosition, length);
			if (lineBreakMatch.Success)
			{
				lineBreakPosition = lineBreakMatch.Index;
				lineBreakLength = lineBreakMatch.Length;
			}
			else
			{
				lineBreakPosition = -1;
				lineBreakLength = 0;
			}
		}

		/// <summary>
		/// Initializes a debug document
		/// </summary>
		private void Initialize()
		{
			int documentStartPosition = 0;
			int documentEndPosition = _code.Length - 1;
			int lineBreakPosition = int.MinValue;
			int lineBreakLength = 0;
			uint lineNumber = 1;

			do
			{
				int linePosition = lineBreakPosition == int.MinValue ?
					documentStartPosition : lineBreakPosition + lineBreakLength;
				int remainderLength = documentEndPosition - linePosition + 1;

				FindLineBreak(_code, linePosition, remainderLength,
					out lineBreakPosition, out lineBreakLength);

				int lineLength = lineBreakPosition != -1 ? lineBreakPosition - linePosition : 0;

				_lines.Add(new DebugLineInfo(lineNumber, (uint)linePosition, (uint)lineLength, (uint)lineBreakLength));
				lineNumber++;
			}
			while (lineBreakPosition != -1 && lineBreakPosition <= documentEndPosition);

			_debugApplicationWrapper.CreateApplicationNode(out _node);
			_node.SetDocumentProvider(this);

			IDebugApplicationNode rootNode;
			_debugApplicationWrapper.GetRootNode(out rootNode);
			_node.Attach(rootNode);
		}

		/// <summary>
		/// Closes a debug document
		/// </summary>
		public void Close()
		{
			if (_node != null)
			{
				_node.Detach();
				_node.Close();
				_node = null;
			}

			if (_lines != null)
			{
				_lines.Clear();
			}
		}

		#region IDebugDocumentInfo implementation

		public void GetName(DocumentNameType type, out string documentName)
		{
			documentName = _name;
		}

		public void GetDocumentClassId(out Guid clsid)
		{
			clsid = Guid.Empty;
		}

		#endregion

		#region IDebugDocumentProvider implementation

		public void GetDocument(out IDebugDocument document)
		{
			document = this;
		}

		#endregion

		#region IDebugDocumentText implementation

		public void GetDocumentAttributes(out TextDocAttrs attrs)
		{
			attrs = TextDocAttrs.ReadOnly;
		}

		public void GetSize(out uint numLines, out uint length)
		{
			numLines = (uint)_lines.Count;
			length = (uint)_code.Length;
		}

		public void GetPositionOfLine(uint lineNumber, out uint position)
		{
			position = 0;
			int lineCount = _lines.Count;

			if (lineNumber == 0 || lineNumber > lineCount)
			{
				throw new ArgumentOutOfRangeException("lineNumber");
			}

			if (lineCount > 0)
			{
				int lineIndex = (int)lineNumber - 1;
				position = _lines[lineIndex].Position;
			}
		}

		public void GetLineOfPosition(uint position, out uint lineNumber, out uint offsetInLine)
		{
			if (position >= _code.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}

			lineNumber = 0;
			offsetInLine = position;
			int lineCount = _lines.Count;

			for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
			{
				DebugLineInfo line = _lines[lineIndex];
				lineNumber = line.Number;
				uint fullLineLength = line.Length + line.BreakLength;

				if (offsetInLine < fullLineLength)
				{
					break;
				}

				offsetInLine -= fullLineLength;
			}
		}

		public void GetText(uint position, IntPtr pChars, IntPtr pAttrs, ref uint length, uint maxChars)
		{
			var codeLength = (uint)_code.Length;
			if (position < codeLength)
			{
				length = Math.Min(codeLength - position, maxChars);

				if (pChars != IntPtr.Zero)
				{
					Marshal.Copy(_code.ToCharArray((int)position, (int)length), 0, pChars, (int)length);
				}

				if (pAttrs != IntPtr.Zero)
				{
					short[] attrs = Enumerable.Repeat((short)SourceTextAttrs.None, (int)length).ToArray();
					Marshal.Copy(attrs, 0, pAttrs, (int)length);
				}
			}
		}

		public void GetPositionOfContext(IDebugDocumentContext context, out uint position, out uint length)
		{
			var documentContext = (DebugDocumentContext)context;
			position = documentContext.Position;
			length = documentContext.Length;
		}

		public void GetContextOfPosition(uint position, uint length, out IDebugDocumentContext context)
		{
			IEnumDebugCodeContexts enumCodeContexts;
			_activeScriptWrapper.EnumCodeContextsOfPosition(_sourceContext, position, length,
				out enumCodeContexts);
			context = new DebugDocumentContext(this, position, length, enumCodeContexts);
		}

		#endregion
	}
}
#endif