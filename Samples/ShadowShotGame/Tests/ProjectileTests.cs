using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using ShadowShotGame;

namespace ShadowShotGameTests
{
	public class ProjectileTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void CreateProjectileInScreen(Type resolver)
		{
			Start(resolver, (ScreenSpace screenSpace, ContentLoader contentLoader) =>
			{
				Initilize(screenSpace, contentLoader);
				Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			});
		}

		private void Initilize(ScreenSpace screenSpace, ContentLoader contentLoader)
		{
			screen = screenSpace;
			content = contentLoader;
			screen.Window.TotalPixelSize = new Size(500, 500);
			image = content.Load<Image>("projectile");
			projectile = new Projectile(image, Point.Half);
		}

		private ScreenSpace screen;
		private ContentLoader content;
		private Image image;
		private Projectile projectile;

		[Test]
		public void MoveProjectileUp()
		{
			Start(typeof(MockResolver), (ScreenSpace screenSpace, ContentLoader contentLoader) =>
			{
				Initilize(screenSpace, contentLoader);
				mockResolver.AdvanceTimeAndExecuteRunners();
				var newProjectileCenter = projectile.DrawArea.Center;
				Assert.AreNotEqual(Point.Half, newProjectileCenter);
			});
		}

		[Test]
		public void MoveProjectileOutsideBorder()
		{
			Start(typeof(MockResolver), (ScreenSpace screenSpace, ContentLoader contentLoader) =>
			{
				Initilize(screenSpace, contentLoader);
				mockResolver.AdvanceTimeAndExecuteRunners();
				var newProjectileCenter = projectile.DrawArea.Center;
				Assert.AreNotEqual(Point.Half, newProjectileCenter);
				mockResolver.AdvanceTimeAndExecuteRunners(5.0f);
				newProjectileCenter = projectile.DrawArea.Center;
				Assert.GreaterOrEqual(0.0f, newProjectileCenter.Y);
				Assert.IsFalse(projectile.IsActive);
			});
		}

		[Test]
		public void DisposeProjectile()
		{
			Start(typeof(MockResolver), (ScreenSpace screenSpace, ContentLoader contentLoader) =>
			{
				Initilize(screenSpace, contentLoader);
				projectile.Dispose();
				Assert.IsFalse(projectile.IsActive);
			});
		}
	}
}