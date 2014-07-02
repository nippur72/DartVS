﻿using System;

namespace DanTup.DartAnalysis
{
	#region JSON deserialisation objects

	class AnalysisElementJson
	{
		public string kind = null;
		public string name = null;
		public AnalysisLocationJson location = null;
		public int flags = 0;
		public string parameters = null;
		public string returnType = null;
	}

	class AnalysisLocationJson
	{
		public string file = null;
		public int offset = 0;
		public int length = 0;
		public int startLine = 0;
		public int startColumn = 0;
	}

	#endregion

	public struct AnalysisElement
	{
		public ElementKind Kind { get; internal set; }
		public string Name { get; internal set; }
		public AnalysisLocation Location { get; internal set; }
		public AnalysisElementFlags Flags { get; internal set; }
		public string Parameters { get; internal set; }
		public string ReturnType { get; internal set; }
		public AnalysisOutline[] Children { get; internal set; }
	}

	public struct AnalysisLocation
	{
		public string File { get; internal set; }
		public int Offset { get; internal set; }
		public int Length { get; internal set; }
		public int StartLine { get; internal set; }
		public int StartColumn { get; internal set; }
	}

	public enum ElementKind
	{
		Class,
		ClassTypeAlias,
		CompilationUnit,
		Constructor,
		Getter,
		Field,
		Function,
		FunctionTypeAlias,
		Library,
		Method,
		Setter,
		TopLevelVariable,
		Unknown,
		UnitTestCase,
		UnitTestGroup
	}

	[Flags]
	public enum AnalysisElementFlags
	{
		None,
		Abstract = 0x01,
		Constant = 0x02,
		Deprecated = 0x20,
		Final = 0x04,
		Private = 0x10,
		Static = 0x08,
	}
}
