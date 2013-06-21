using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class CreepTests : TestWithAllFrameworks
	{
		[SetUp]
		public void Init()
		{
			creepName = "SandCreep";
			moveSpeed = 0.2f;
			startPointCreep = new Point(0.0f, 0.45f);
			size = new Size(0.1f);
		}

		private string creepName;
		private float moveSpeed;
		private Point startPointCreep;

		private Size size;

		[VisualTest]
		public void DisplaySandCreep(Type resolver)
		{
			Start(resolver,
				(ContentLoader content, ScreenSpace screen) => { CreateCreep(content, screen); });
		}

		private void CreateCreep(ContentLoader content, ScreenSpace screen)
		{
			var creepImage = content.Load<Image>(creepName);
			var drawArea = new Rectangle(startPointCreep, size);
			creep = new Creep(screen, creepImage, drawArea);
		}

		private Creep creep;

		[Test]
		public void MoveSandCreepRight()
		{
			Start(typeof(MockResolver), (ContentLoader content, ScreenSpace screen) =>
			{
				CreateCreep(content, screen);
				Assert.AreEqual(startPointCreep, creep.DrawArea.TopLeft);
				creep.Get<MovementData>().Speed = moveSpeed;
				mockResolver.AdvanceTimeAndExecuteRunners(10);
				Assert.GreaterOrEqual(0.95f, creep.DrawArea.Left);
				Assert.AreEqual(startPointCreep.Y, creep.DrawArea.Top);
			});
		}

		[VisualTest]
		public void DisplaySandCreepWalking(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				CreateCreep(content, screen);
				creep.Get<MovementData>().Speed = moveSpeed;
			});
		}
	}
}