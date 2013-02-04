using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Integration tests always close the window automatically after 1 frame (OpenGL, SharpDX, XNA)
	/// </summary>
	public class IntegrationTestAttribute : TestCaseSourceAttribute
	{
		public IntegrationTestAttribute()
			: base(typeof(TestStarter), "Resolvers") {}
	}
}