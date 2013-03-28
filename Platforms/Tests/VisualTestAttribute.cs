using System;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Simplifies visual tests, which just use the TestStarter.Resolvers (Test, OpenGL, SharpDX, XNA)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class VisualTestAttribute : IntegrationTestAttribute {}
}