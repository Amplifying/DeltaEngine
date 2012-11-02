using System.Diagnostics;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class StackTraceExtensionsTests
	{
		[Test]
		public void ContainsNoTestOrIsVisualTest()
		{
			var isThisAVisualTest = StackTraceExtensions.ContainsNoTestOrIsVisualTest();
			Assert.IsFalse(isThisAVisualTest);
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

		[Test]
		public void Main()
		{
			// Note: Because this method is named "Main" we will get the namespace name instead!
			Assert.AreEqual("DeltaEngine.Core.Tests", StackTraceExtensions.GetEntryName());
		}

		[Test]
		public void GetTestMethodName()
		{
			Assert.AreEqual("GetTestMethodName", new StackTrace().GetFrames().GetTestMethodName());
			Assert.AreEqual("", new StackFrame[0].GetTestMethodName());
		}
	}
}