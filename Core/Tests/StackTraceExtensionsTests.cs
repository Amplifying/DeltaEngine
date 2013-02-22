using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	[CLSCompliant(false)]
	public class StackTraceExtensionsTests
	{
		[Test]
		public void ContainsNoTestOrIsVisualTest()
		{
			Assert.IsFalse(StackTraceExtensions.ContainsNoTestOrIsVisualTest());
			StackTraceExtensions.InVisualTestCase = true;
			Assert.IsTrue(StackTraceExtensions.ContainsNoTestOrIsVisualTest());
		}
		
		[Test]
		public void HasAttribute()
		{
			var stackFrame = new StackFrame();
			Assert.IsTrue(stackFrame.HasAttribute("NUnit.Framework.TestAttribute"));
			Assert.IsFalse(stackFrame.HasAttribute("Foo.Baar"));
		}

		[Test]
		public void GetEntryName()
		{
			Assert.AreEqual("GetEntryName", StackTraceExtensions.GetEntryName());
		}

		[DerivedTestToFakeMain]
		public void Main()
		{
			// Because this method is named "Main" we will get the namespace name instead!
			Assert.AreEqual("DeltaEngine.Core.Tests", StackTraceExtensions.GetEntryName());
		}

		private class DerivedTestToFakeMainAttribute : TestAttribute {}

		[Test]
		public void GetTestMethodName()
		{
			Assert.AreEqual("GetTestMethodName", new StackTrace().GetFrames().GetTestMethodName());
			Assert.AreEqual("", new StackFrame[0].GetTestMethodName());
		}

		[Test]
		public void ContainsUnitTest()
		{
			Assert.IsTrue(StackTraceExtensions.ContainsUnitTest());
		}

		[TestCase]
		public void GetTestCaseName()
		{
			Assert.AreEqual("GetTestCaseName", new StackTrace().GetFrames().GetTestMethodName());
			Assert.IsFalse(IsTestCaseWithIgnore(new StackFrame()));
		}

		private static bool IsTestCaseWithIgnore(StackFrame frame)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			return
				attributes.Any(
					attribute => attribute is TestCaseAttribute && (attribute as TestCaseAttribute).Ignore);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void HasSlowCategoryAttribute()
		{
			var stackFrame = new StackFrame();
			Assert.IsTrue(stackFrame.HasAttribute("NUnit.Framework.CategoryAttribute"));
		}

		[TestCase(Ignore = true)]
		public void TestCaseHasIgnoreAttribute()
		{
			Assert.IsTrue(IsTestCaseWithIgnore(new StackFrame()));
			Assert.IsFalse(IsTestCaseWithSlowCategory(new StackFrame()));
		}

		[TestCase(Category = "Slow")]
		public void TestCaseHasSlowCategoryAttribute()
		{
			Assert.IsFalse(IsTestCaseWithIgnore(new StackFrame()));
			Assert.IsTrue(IsTestCaseWithSlowCategory(new StackFrame()));
		}

		private static bool IsTestCaseWithSlowCategory(StackFrame frame)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			foreach (object attribute in attributes)
				if (attribute is TestCaseAttribute && (attribute as TestCaseAttribute).Category == "Slow")
					return true;
			return false;
		}

		[Test, Ignore]
		public void HasIgnoreAttribute()
		{
			var stackFrame = new StackFrame();
			Assert.IsTrue(stackFrame.HasAttribute("NUnit.Framework.IgnoreAttribute"));
		}
	}
}