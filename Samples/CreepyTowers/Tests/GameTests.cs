using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Scenes;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class GameTests : TestWithAllFrameworks
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
		public void ShowMenuWithTwoButtons(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content, Game game) => { });
		}

		[VisualTest]
		public void DisplayMultipleTowersAndCreepWalking(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				CreateAndMoveCreep(content, screen);
				CreateMultipleTowers(content, screen);
			});
		}

		private void CreateMultipleTowers(ContentLoader content, ScreenSpace screen)
		{
			CreateTower(content, screen, "WaterTower", new Point(0.3f, 0.3f));
			CreateTower(content, screen, "FireTower", new Point(0.5f, 0.3f));
			CreateTower(content, screen, "SlicingTower", new Point(0.7f, 0.3f));
			CreateTower(content, screen, "IceTowerPlaceholder", new Point(0.3f, 0.6f));
			CreateTower(content, screen, "AcidTowerPlaceholder", new Point(0.5f, 0.6f));
			CreateTower(content, screen, "WaterTower", new Point(0.7f, 0.6f));
		}

		private Creep creep;

		private void CreateAndMoveCreep(ContentLoader content, ScreenSpace screen)
		{
			var creepImage = content.Load<Image>(creepName);
			var drawArea = new Rectangle(startPointCreep, size);
			creep = new Creep(screen, creepImage, drawArea);
			creep.Get<MovementData>().Speed = moveSpeed;
		}

		private void CreateTower(ContentLoader content, ScreenSpace screen, String towerName,
			Point towerStartPoint)
		{
			var towerImage = content.Load<Image>(towerName);
			var drawArea = new Rectangle(towerStartPoint, size);
			tower = new Tower(screen, towerImage, drawArea);
		}

		private Tower tower;
	}
}