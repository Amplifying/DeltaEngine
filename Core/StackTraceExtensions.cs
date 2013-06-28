using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides additional check methods on stack traces to find out where we are (e.g. in tests)
	/// </summary>
	public static class StackTraceExtensions
	{
		public static bool StartedFromNCrunch
		{
			get
			{
				if (alreadyCheckedIfStartedFromNCrunch)
					return wasStartedFromNCrunch;

				alreadyCheckedIfStartedFromNCrunch = true;
				wasStartedFromNCrunch = IsStartedFromNCrunch();
				return wasStartedFromNCrunch;
			}
		}

		private static bool alreadyCheckedIfStartedFromNCrunch;
		private static bool wasStartedFromNCrunch;

		private static bool IsStartedFromNCrunch()
		{
			var stackFrames = new StackTrace().GetFrames();
			foreach (var frame in stackFrames)
				if (frame.GetMethod().ReflectedType.FullName.StartsWith("nCrunch.TestExecution."))
					return true;

			return false; //ncrunch: no coverage
		}

		public static bool IsStartedFromNunitConsole()
		{
			string currentDomainName = AppDomain.CurrentDomain.FriendlyName;
			return currentDomainName == "NUnit Domain" || currentDomainName.StartsWith("test-domain-");
		}

		public static string GetApprovalTestName()
		{
			var frames = new StackTrace().GetFrames();
			foreach (var frame in frames)
			{
				if (IsTestAttribute(frame) && frame.HasAttribute(ApproveFirstFrameScreenshotAttribute))
					return frames.GetClassName() + "." + frames.GetTestMethodName();
				if (IsInTestSetUpButRunningTestHasApprovalAttribute(frame))
					return unitTestClassName + "." + unitTestMethodName;
			}
			return "";
		}

		private static bool IsInTestSetUpButRunningTestHasApprovalAttribute(StackFrame frame)
		{
			if (String.IsNullOrEmpty(unitTestMethodName) || !IsInTestSetUp(frame))
				return false;

			var testClassType = GetClassTypeFromRunningAssemblies(unitTestClassFullName);
			if (testClassType == null)
				return false;

			var method = testClassType.GetMethod(unitTestMethodName);
			if (method == null)
				return false;

			object[] attributes = method.GetCustomAttributes(false);
			return attributes.Any(a => a.GetType().ToString() == ApproveFirstFrameScreenshotAttribute);
		}

		private static Type GetClassTypeFromRunningAssemblies(string classFullName)
		{
			var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in runningAssemblies)
				if (classFullName.StartsWith(assembly.GetName().Name))
					return assembly.GetType(classFullName);
			return null;
		}

		/// <summary>
		/// Since we do not initialize or run the resolver in a test, we need to set the current unit
		/// test name up beforehand so we can find out if the test uses ApproveFirstFrameScreenshot.
		/// </summary>
		public static void SetUnitTestName(string fullName)
		{
			var nameParts = fullName.Split(new[] { '.' });
			unitTestMethodName = nameParts[nameParts.Length - 1];
			unitTestClassName = nameParts[nameParts.Length - 2];
			unitTestClassFullName = nameParts[0];
			for (int num = 1; num < nameParts.Length - 1; num++)
				unitTestClassFullName += "." + nameParts[num];
		}

		private static string unitTestClassName;
		private static string unitTestMethodName;
		private static string unitTestClassFullName;

		/// <summary>
		/// When we don't know the attribute type we cannot use Attribute.IsAttribute. Use this instead.
		/// </summary>
		public static bool HasAttribute(this StackFrame frame, string name)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			return attributes.Any(attribute => attribute.GetType().ToString() == name);
		}

		private const string TestAttribute = "NUnit.Framework.TestAttribute";
		private const string ApproveFirstFrameScreenshotAttribute =
			"DeltaEngine.Platforms.ApproveFirstFrameScreenshotAttribute";

		public static bool IsUnitTest()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			return frames.Any(frame => IsTestAttribute(frame) || IsInTestSetUp(frame));
		}

		/// <summary>
		/// Get entry name from stack frame, which is either the namespace name where the main method
		/// is located or if we are started from a test, the name of the test method.
		/// </summary>
		public static string GetEntryName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
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
			{
				if (IsTestAttribute(frame))
					return frame.GetMethod().Name;
				if (!String.IsNullOrEmpty(unitTestMethodName) && IsInTestSetUp(frame))
					return unitTestMethodName;
			}

			return string.Empty;
		}

		public static string GetExecutingAssemblyName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			foreach (StackFrame frame in frames)
				if (IsTestAttribute(frame))
					return frame.GetMethod().DeclaringType.Assembly.GetName().Name;

			if (!String.IsNullOrEmpty(unitTestClassFullName))
				return GetNamespaceNameFromClassName(unitTestClassFullName);

			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);

			throw new ExecutingAssemblyOrNamespaceNotFound();
		}

		private static string GetNamespaceNameFromClassName(string fullClassName)
		{
			var result = System.IO.Path.GetFileNameWithoutExtension(fullClassName);
			if (result.Contains(".Tests."))
				result = System.IO.Path.GetFileNameWithoutExtension(result);
			return result;
		}

		public class ExecutingAssemblyOrNamespaceNotFound : Exception {}

		private static bool IsTestAttribute(StackFrame frame)
		{
			return frame.HasAttribute(TestAttribute);
		}

		private static bool IsInTestSetUp(StackFrame frame)
		{
			return frame.HasAttribute(SetUpAttribute);
		}

		private const string SetUpAttribute = "NUnit.Framework.SetUpAttribute";

		private static string GetNamespaceName(StackFrame frame)
		{
			var classType = frame.GetMethod().DeclaringType;
			return classType != null ? classType.Namespace : "";
		}

		public static string GetClassName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames.Where(frame => IsTestAttribute(frame)))
				return frame.GetMethod().DeclaringType.Name;

			return string.Empty;
		}
	}
}