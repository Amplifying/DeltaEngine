using System;
using DeltaEngine.Platforms.All;

namespace GameOfDeath.Tests
{
	internal class IntroTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Show(Type resolver)
		{
			Start(resolver, (Intro intro) => {});
		}
	}
}