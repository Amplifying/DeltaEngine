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
			return !frames.Any(IsJustTestMethod) || GetIsVisualTestCaseAndReset();
		}

		private static bool IsJustTestMethod(StackFrame frame)
		{
			bool isVisualTest = frame.HasAttribute(VisualTestAttribute);
			return frame.HasAttribute(TestAttribute) && !frame.HasAttribute(IgnoreAttribute) &&
				!isVisualTest || frame.HasAttribute(TestCaseAttribute) ||
				frame.HasAttribute(IntegrationTestAttribute) || StartedFromNCrunch && isVisualTest;
		}

		public static bool StartedFromNCrunch { get; set; }

		private const string TestAttribute = "NUnit.Framework.TestAttribute";
		private const string IgnoreAttribute = "NUnit.Framework.IgnoreAttribute";
		private const string TestCaseAttribute = "NUnit.Framework.TestCaseAttribute";
		private const string IntegrationTestAttribute =
			"DeltaEngine.Platforms.All.IntegrationTestAttribute";
		private const string VisualTestAttribute = "DeltaEngine.Platforms.All.VisualTestAttribute";
		private const string ApproveFirstFrameScreenshotAttribute =
			"DeltaEngine.Platforms.Tests.ApproveFirstFrameScreenshotAttribute";

		public static bool IsApprovalTest(this IEnumerable<StackFrame> frames)
		{
			return frames.Any(f => f.HasAttribute(ApproveFirstFrameScreenshotAttribute) &&
				(f.HasAttribute(IntegrationTestAttribute) || f.HasAttribute(VisualTestAttribute)));
		}

		private static bool GetIsVisualTestCaseAndReset()
		{
			if (IsVisualTestCase == false)
				return false;
			IsVisualTestCase = false;
			return true;
		}

		/// <summary>
		/// Since we cannot access NUnit.Framework.TestCaseAttribute here, inject it from TestStarter.
		/// </summary>
		public static bool IsVisualTestCase { private get; set; }

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

		private const string CategoryAttribute = "NUnit.Framework.CategoryAttribute";

		/// <summary>
		/// Get entry name from stack frame, which is either the namespace name where the main method
		/// is located or if we are started from a test, the name of the test method.
		/// </summary>
		public static string GetEntryName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			Debug.Assert(frames != null);

			var testName = GetTestMethodName(frames);
			if (!string.IsNullOrEmpty(testName))
				return testName;

			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);

			return "Delta Engine"; //ncrunch: no coverage
		}

		public static string GetTestMethodName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames)
				if (IsAnyTestAttribute(frame))
					return frame.GetMethod().Name;

			return string.Empty;
		}

		private static bool IsAnyTestAttribute(StackFrame frame)
		{
			return frame.HasAttribute(TestAttribute) || frame.HasAttribute(TestCaseAttribute) ||
				frame.HasAttribute(VisualTestAttribute) || frame.HasAttribute(IntegrationTestAttribute);
		}

		private static string GetNamespaceName(StackFrame frame)
		{
			var classType = frame.GetMethod().DeclaringType;
			return classType != null ? classType.Namespace : "";
		}

		public static string GetClassName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames.Where(frame => IsAnyTestAttribute(frame)))
				return frame.GetMethod().DeclaringType.Name;

			return string.Empty;
		}
	}
}