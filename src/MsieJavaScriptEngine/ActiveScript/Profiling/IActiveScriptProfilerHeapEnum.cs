using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript.Profiling
{
	/// <summary>
	/// An iterator over the heap objects associated with a script engine, gathered by the
	/// <code>IActiveScriptProfilerControl3.EnumHeap</code> method
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Name defined in COM.")]
	[Guid("32E4694E-0D37-419B-B93D-FA20DED6E8EA")]
	internal interface IActiveScriptProfilerHeapEnum
	{ }
}