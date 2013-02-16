using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;

namespace GameOfDeath.Tests
{
	class UITests : TestStarter
	{
		[VisualTest]
		public void ShowBackgroundWithUI(Type resolver)
		{
			Start(resolver, (UI ui) => { });
		}

		[VisualTest]
		public void Resize(Type resolver)
		{
			Start(resolver, (UI ui, Window window) => window.TotalPixelSize = new Size(800, 600));
		}
	}
}