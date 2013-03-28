using System;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Integration tests always close the window automatically after 1 frame (OpenGL, SharpDX, XNA)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class IntegrationTestAttribute : TestCaseSourceAttribute
	{
		public IntegrationTestAttribute()
			: base(typeof(TestStarter), "Resolvers") {}
	}
}