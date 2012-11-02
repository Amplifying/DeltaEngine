using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.WindowsXna.Tests
{
	public class AppTests
	{
		[Test, Ignore]
		public void ShowColoredRectangle()
		{
			App.Start((Renderer r) => new ColoredRectangle(r, Point.Half, Size.Half, Color.Red));
		}

		[Test, Ignore]
		public void ShowWindowAndStartRunner()
		{
			App.Start<DummyRunner>();
			Assert.IsNotNull(new DummyRunner());
		}

		private class DummyRunner : Runner
		{
			public void Run() { }
		}
	}
}