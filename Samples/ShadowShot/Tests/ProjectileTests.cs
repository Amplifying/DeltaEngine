using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace ShadowShot.Tests
{
	public class ProjectileTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateProjectileInScreen(Type resolver)
		{
			Initilize();
			//projectile.Size = new Size(0.05f, 0.1f);
			projectile.Get<SimplePhysics.Data>().Velocity = Point.Zero;
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
		}

		private void Initilize()
		{
			Resolve<ScreenSpace>().Window.ViewportPixelSize = new Size(500, 500);
			image = ContentLoader.Load<Image>("projectile");
			projectile = new Projectile(image, Point.Half);
		}

		private Image image;
		private Projectile projectile;

		[Test]
		public void MoveProjectileUp()
		{
			Initilize();
			resolver.AdvanceTimeAndExecuteRunners();
			var newProjectileCenter = projectile.DrawArea.Center;
			Assert.AreNotEqual(Point.Half, newProjectileCenter);
		}

		[Test]
		public void MoveProjectileOutsideBorder()
		{
			Initilize();
			resolver.AdvanceTimeAndExecuteRunners();
			var newProjectileCenter = projectile.DrawArea.Center;
			Assert.AreNotEqual(Point.Half, newProjectileCenter);
			resolver.AdvanceTimeAndExecuteRunners(5.0f);
			newProjectileCenter = projectile.DrawArea.Center;
			Assert.GreaterOrEqual(0.0f, newProjectileCenter.Y);
			Assert.IsFalse(projectile.IsActive);
		}

		[Test]
		public void DisposeProjectile()
		{
			Initilize();
			projectile.Dispose();
			Assert.IsFalse(projectile.IsActive);
		}
	}
}