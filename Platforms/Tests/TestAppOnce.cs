using System;
using DeltaEngine.Core;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Provides start functions for test apps and unit tests. Nothing is shown on screen and all
	/// test apps will quit after one frame, but all of the app logic is run to make sure it works.
	/// </summary>
	public static class TestAppOnce
	{
		public static void Start<AppEntryRunner>()
			where AppEntryRunner : Runner
		{
			using (var resolver = new TestResolver().Init<AppEntryRunner>())
				resolver.Run();
		}

		public static void Start<FirstClass>(Action<FirstClass> initCode, Action runCode = null)
		{
			using (var resolver = new TestResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<First, Second>(Action<First, Second> initCode, Action runCode = null)
		{
			using (var resolver = new TestResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<First, Second, Third>(Action<First, Second, Third> initCode,
			Action runCode = null)
		{
			using (var resolver = new TestResolver().Init(initCode))
				resolver.Run(runCode);
		}
	}
}