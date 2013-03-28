using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class AppStarterTests : TestStarter
	{
		[Test, Category("Slow")]
		public void StartAppWithOneClassToRegister()
		{
			var app = new OpenTKResolver();
			app.Start<MockEntryRunner, FirstClass>();
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
		}

		[Test, Category("Slow")]
		public void StartAppWithTwoClassToRegister()
		{
			var app = new OpenTKResolver();
			app.Start<MockEntryRunner, FirstClass, SecondClass>();
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
			var secondClass = app.Resolve<SecondClass>();
			Assert.IsNotNull(secondClass);
		}

		[Test, Category("Slow")]
		public void StartAppWithListOfTypesToRegister()
		{
			var app = new OpenTKResolver();
			var typeList = new List<Type> { typeof(FirstClass), typeof(SecondClass) };
			app.Start<MockEntryRunner>(typeList);
			var firstClass = app.Resolve<FirstClass>();
			Assert.IsNotNull(firstClass);
			var secondClass = app.Resolve<SecondClass>();
			Assert.IsNotNull(secondClass);
			Assert.IsNotNull(new MockEntryRunner());
		}

		private class MockEntryRunner : Runner
		{
			public void Run() {}
		}

		private class FirstClass {}
		private class SecondClass {}
	}
}