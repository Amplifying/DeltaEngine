using System;
using DeltaEngine.Core;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Entry point for Windows DirectX applications and tests, implementation details are hidden.
	/// </summary>
	public static class App
	{
		public static void Start<AppEntryRunner>()
			where AppEntryRunner : Runner
		{
			using (var resolver = new SharpDXResolver().Init<AppEntryRunner>())
				resolver.Run();
		}

		public static void Start<FirstClass>(Action<FirstClass> initCode, Action runCode = null)
		{
			using (var resolver = new SharpDXResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<FirstClass, SecondClass>(Action<FirstClass, SecondClass> initCode,
			Action runCode = null)
		{
			using (var resolver = new SharpDXResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<First, Second, Third>(Action<First, Second, Third> initCode,
			Action runCode = null)
		{
			using (var resolver = new SharpDXResolver().Init(initCode))
				resolver.Run(runCode);
		}
	}
}