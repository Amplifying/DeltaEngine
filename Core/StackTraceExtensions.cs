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

		private static bool HasNoTestOrIsVisualTest(IEnumerable<StackFrame> frames)
		{
			return !frames.Any(IsTestMethod) || InVisualTestCase;
		}

		private static bool IsTestMethod(StackFrame frame)
		{
			return frame.HasAttribute(TestAttribute) && !frame.HasAttribute(IgnoreAttribute) ||
				frame.HasAttribute(TestCaseAttribute) || frame.HasAttribute(IntegrationTestAttribute) ||
				StartedFromNCrunch && frame.HasAttribute(VisualTestAttribute);
		}

		public static bool StartedFromNCrunch { get; set; }
		public static bool StartedFromNunitConsole { get; set; }

		const string TestAttribute = "NUnit.Framework.TestAttribute";
		const string IgnoreAttribute = "NUnit.Framework.IgnoreAttribute";
		const string TestCaseAttribute = "NUnit.Framework.TestCaseAttribute";
		const string IntegrationTestAttribute = "DeltaEngine.Platforms.Tests.IntegrationTestAttribute";
		const string VisualTestAttribute = "DeltaEngine.Platforms.Tests.VisualTestAttribute";

		/// <summary>
		/// Since we cannot access NUnit.Framework.TestCaseAttribute here, inject it from TestStarter.
		/// </summary>
		public static bool InVisualTestCase
		{
			private get
			{
				if (inTestCaseWithIgnore == false)
					return false;
				inTestCaseWithIgnore = false;
				return true;
			}
			set { inTestCaseWithIgnore = value; }
		}

		private static bool inTestCaseWithIgnore;

		public static bool HasAttribute(this StackFrame frame, string name)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			return attributes.Any(attribute => attribute.GetType().ToString() == name);
		}

		public static bool ContainsUnitTest()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			return frames != null && frames.Any(IsTestWithoutCategory);
		}

		private static bool IsTestWithoutCategory(this StackFrame frame)
		{
			return frame.HasAttribute(TestAttribute) && !frame.HasAttribute(CategoryAttribute);
		}

		const string CategoryAttribute = "NUnit.Framework.CategoryAttribute";
		
		/// <summary>
		/// Get entry name from stack frame, which is either the namespace name where the main method
		/// is located or if we are started from a test, the name of the test method.
		/// </summary>
		public static string GetEntryName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			Debug.Assert(frames != null);

			var testName = GetTestMethodName(frames);
			if (testName != "")
				return testName;

			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);

			return "Delta Engine"; //ncrunch: no coverage
		}

		public static string GetTestMethodName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames)
				if (frame.HasAttribute(TestAttribute) || frame.HasAttribute(TestCaseAttribute) ||
					frame.HasAttribute(VisualTestAttribute) || frame.HasAttribute(IntegrationTestAttribute))
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