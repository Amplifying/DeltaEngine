using System;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BackgroundTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (EntitySystem entitySystem, Background background, ContentLoader content) =>
			{
				//var b = new Background(content);
				//entitySystem.Add(b);
				Assert.IsTrue(background.Visibility == Visibility.Show);
			});
		}
	}
}