using NUnit.Framework;

namespace DeltaEngine.Platforms.All
{
	/// <summary>
	/// Integration tests always close the window automatically after 1 frame (OpenGL, SharpDX, XNA)
	/// </summary>
	public class IntegrationTestAttribute : TestCaseSourceAttribute
	{
		public IntegrationTestAttribute()
			: base(typeof(TestWithAllFrameworks), "AllResolvers") {}
	}
}