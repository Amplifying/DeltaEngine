using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using NUnit.Framework;
using DeltaEngine.Graphics;

namespace SideScroller.Tests
{
	class PlaneTests : TestWithMocksOrVisually
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

			InitPlayerPlane(ContentLoader.Load<Image>(PlaneTextureName));
			bool shotfired = false;
			playerPlane.PlayerFiredShot += point =>
			{
				Assert.AreEqual(playerPlane.Get<Rectangle>().Center, point);
				shotfired = true;
			};
			playerPlane.IsFireing = true;
			resolver.AdvanceTimeAndExecuteRunners(0.2f);
			Assert.IsTrue(shotfired);
		}

		[Test]
		public void MovePlaneVertically()
		{
			InitPlayerPlane(ContentLoader.Load<Image>(PlaneTextureName));
			CheckMoveUp();
			CheckMoveDown();
			CheckStop();
		}

		private void CheckMoveUp()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(1);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.Greater(playerPlane.YPosition, originalYCoord);
		}

		private void CheckMoveDown()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(-1);
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.Less(playerPlane.YPosition, originalYCoord);
		}

		private void CheckStop()
		{
			playerPlane.AccelerateVertically(3);
			var originalSpeed = playerPlane.Get<Velocity2D>().velocity.Y;
			playerPlane.StopVertically();
			resolver.AdvanceTimeAndExecuteRunners();
			Assert.Less(playerPlane.Get<Velocity2D>().velocity.Y, originalSpeed);
		}

		[Test]
		public void HittingTopBorder()
		{
			InitPlayerPlane(ContentLoader.Load<Image>(PlaneTextureName));
		}

		[Test]
		public void CreatePlayerPlane(Type resolver)
		{
			InitPlayerPlane(ContentLoader.Load<Image>(PlaneTextureName));
		}

		[Test]
		public void CreateEnemyPlane(Type resolver)
		{
			InitEnemyPlane(ContentLoader.Load<Image>(EnemyTextureName));
		}

		private void InitEnemyPlane(Image foeTexture)
		{
			new EnemyPlane(foeTexture, new Point(1.2f, 0.5f));
		}

		private const string EnemyTextureName = "testplaneFlip";
	}
}
