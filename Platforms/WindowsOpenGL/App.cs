using System;
using DeltaEngine.Core;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Provides the Start function for OpenTK applications and visual tests.
	/// </summary>
	public static class App
	{
		public static void Start<AppEntryRunner>()
			where AppEntryRunner : Runner
		{
			using (var resolver = new OpenTKResolver().Init<AppEntryRunner>())
				resolver.Run();
		}

		public static void Start<FirstClass>(Action<FirstClass> initCode, Action runCode = null)
		{
			using (var resolver = new OpenTKResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<First, Second>(Action<First, Second> initCode, Action runCode = null)
		{
			using (var resolver = new OpenTKResolver().Init(initCode))
				resolver.Run(runCode);
		}

		public static void Start<First, Second, Third>(Action<First, Second, Third> initCode,
			Action runCode = null)
		{
			using (var resolver = new OpenTKResolver().Init(initCode))
				resolver.Run(runCode);
		}
	}
}