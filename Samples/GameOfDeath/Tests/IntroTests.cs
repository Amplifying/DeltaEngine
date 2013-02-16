using System;
using DeltaEngine.Platforms.Tests;

namespace GameOfDeath.Tests
{
	class IntroTests : TestStarter
	{
		[VisualTest]
		public void Show(Type resolver)
		{
			Start(resolver, (Intro intro) => { });
		}
	}
}