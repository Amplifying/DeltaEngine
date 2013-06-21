using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;
using DeltaEngine.Graphics;

namespace SideScrollerSample.Tests
{
	class PlayerPlaneTests : TestWithAllFrameworks
	{
		private void InitPlayerPlane(Image playerTexture)
		{
			playerPlane = new PlayerPlane(playerTexture, Point.Half);
		}

		private PlayerPlane playerPlane;
		private const string PlaneTextureName = "testplane";

		[Test]
		public void FireShotEvent()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerPlane(contentLoader.Load<Image>(PlaneTextureName));
				bool shotfired = false;
				playerPlane.PlayerFiredShot += point =>
				{
					Assert.AreEqual(playerPlane.Get<Rectangle>().Center, point);
					shotfired = true;
				};
				playerPlane.IsFireing = true;
				mockResolver.AdvanceTimeAndExecuteRunners(0.2f);
				Assert.IsTrue(shotfired);
			});
		}

		[Test]
		public void MovePlaneVertically()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerPlane(contentLoader.Load<Image>(PlaneTextureName));
				checkMoveUp();
				checkMoveDown();
				checkStop();
			});
		}

		private void checkMoveUp()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(1);
			mockResolver.AdvanceTimeAndExecuteRunners();
			Assert.Greater(playerPlane.YPosition, originalYCoord);
		}

		private void checkMoveDown()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(-1);
			mockResolver.AdvanceTimeAndExecuteRunners();
			Assert.Less(playerPlane.YPosition, originalYCoord);
		}

		private void checkStop()
		{
			playerPlane.AccelerateVertically(3);
			var originalSpeed = playerPlane.Get<Velocity2D>().velocity.Y;
			playerPlane.StopVertically();
			mockResolver.AdvanceTimeAndExecuteRunners();
			Assert.Less(playerPlane.Get<Velocity2D>().velocity.Y, originalSpeed);
		}

		[Test]
		public void HittingTopBorder()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerPlane(contentLoader.Load<Image>(PlaneTextureName));

			});
		}
	}
}
