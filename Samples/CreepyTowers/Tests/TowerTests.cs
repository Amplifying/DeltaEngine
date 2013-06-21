using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class TowerTests : TestWithAllFrameworks
	{
		[SetUp]
		public void Init()
		{
			towerName = "WaterTower";
			startPoint = new Point(0.0f, 0.45f);
			size = new Size(0.1f);
		}

		private string towerName;
		private Point startPoint;
		private Size size;

		[VisualTest]
		public void DisplayTower(Type resolver)
		{
			Start(resolver,
				(ContentLoader content, ScreenSpace screen) => { CreateTower(content, screen); });
		}

		private void CreateTower(ContentLoader content, ScreenSpace screen)
		{
			var towerImage = content.Load<Image>(towerName);
			var drawArea = new Rectangle(startPoint, size);
			tower = new Tower(screen, towerImage, drawArea);
		}

		private Tower tower;

		[VisualTest]
		public void FireTower(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				CreateTower(content, screen);
				tower.FireWhenEnemyInRange();
			});
		}
	}
}