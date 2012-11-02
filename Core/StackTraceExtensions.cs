using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides additional check methods on stack traces to find out where we are (e.g. in tests)
	/// </summary>
	public static class StackTraceExtensions
	{
		public static bool ContainsNoTestOrIsVisualTest()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			return frames == null || StartedFromMain(frames) || HasNoTestOrIsVisualTest(frames);
		}

		private static bool StartedFromMain(IEnumerable<StackFrame> frames)
		{
			return frames.Any(frame => frame.GetMethod().Name == "Main");
		}

		private static bool HasNoTestOrIsVisualTest(StackFrame[] frames)
		{
			return !frames.Any(IsTestMethod) || frames.Any(IsVisualTestMethod);
		}

		private static bool IsTestMethod(StackFrame frame)
		{
			return frame.HasAttribute("NUnit.Framework.TestAttribute");
		}

		private static bool IsVisualTestMethod(StackFrame frame)
		{
			return frame.HasAttribute("NUnit.Framework.IgnoreAttribute");
		}

		public static bool HasAttribute(this StackFrame frame, string name)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			return attributes.Any(attribute => attribute.GetType().ToString() == name);
		}

		public static bool ContainsUnitTest()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			return frames != null && frames.Any(IsTest);
		}

		private static bool IsTest(this StackFrame frame)
		{
			return frame.HasAttribute("NUnit.Framework.TestAttribute") &&
				!frame.HasAttribute("NUnit.Framework.CategoryAttribute") &&
				!frame.HasAttribute("NUnit.Framework.IgnoreAttribute");
		}

		/// <summary>
		/// Get entry name from stack frame, which is either the namespace name where the main method
		/// is located or if we are started from a test, the name of the test method.
		/// </summary>
		public static string GetEntryName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			Debug.Assert(frames != null);

			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);

			return GetTestMethodName(frames);
		}

		public static string GetTestMethodName(this IEnumerable<StackFrame> frames)
		{
			const string TestAttribute = "NUnit.Framework.TestAttribute";
			foreach (StackFrame frame in frames.Where(frame => frame.HasAttribute(TestAttribute)))
				return frame.GetMethod().Name;
			return "";
		}

		private static string GetNamespaceName(StackFrame frame)
		{
			var classType = frame.GetMethod().DeclaringType;
			return classType != null ? classType.Namespace : "";
		}
	}
}