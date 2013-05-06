using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;

namespace Blobs.Tests
{
	public class PlatformTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void DrawRotatingPlatform(Type resolver)
		{
			Platform platform = null;

			Start(resolver, (EntitySystem entitySystem) =>
			{
				platform = new Platform(entitySystem, new Rectangle(0.3f, 0.3f, 0.3f, 0.2f))
				{
					Color = Color.Gold
				};
			}, () =>
			{
				//platform.Rotation += 45 * Time.Current.Delta;
			});
		}
	}
}