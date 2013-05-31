using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class Entity2DTests
	{
		[Test]
		public void CreateEntity2D()
		{
			var entity = new Entity2D(DoubleSizedRectangle, Color.Green, 15);
			Assert.AreEqual(DoubleSizedRectangle, entity.DrawArea);
			Assert.AreEqual(Color.Green, entity.Color);
			Assert.AreEqual(15, entity.Rotation);
			Assert.AreEqual(Entity2D.DefaultRenderLayer, entity.RenderLayer);
			Assert.AreEqual(Point.One, entity.Center);
			Assert.AreEqual(new Size(2, 2), entity.Size);
		}

		private static readonly Rectangle DoubleSizedRectangle = new Rectangle(0, 0, 2, 2);

		[Test]
		public void AddEmptyComponent()
		{
			var entity = new Entity2D(Rectangle.Zero);
			Assert.AreEqual(Rectangle.Zero, entity.DrawArea);
			Assert.AreEqual(Color.White, entity.Color);
			Assert.AreEqual(3, entity.NumberOfComponents);
			entity.Add(Size.Zero);
			Assert.AreEqual(4, entity.NumberOfComponents);
		}

		[Test]
		public void SetDrawAreaProperties()
		{
			var entity = new Entity2D(Rectangle.One, Color.Blue)
			{
				Center = Point.One,
				Size = new Size(2)
			};
			Assert.AreEqual(DoubleSizedRectangle, entity.DrawArea);
			entity.DrawArea = new Rectangle(-1, -1, 2, 2);
			Assert.AreEqual(Point.Zero, entity.Center);
			entity.TopLeft = Point.Zero;
			Assert.AreEqual(Point.Zero, entity.TopLeft);
			Assert.AreEqual(DoubleSizedRectangle, entity.DrawArea);
		}

		[Test]
		public void SetColorRotationAndRenderLayerProperties()
		{
			var entity = new Entity2D(Rectangle.One, Color.Blue);
			entity.Color = Color.Teal;
			Assert.AreEqual(Color.Teal, entity.Color);
			entity.AlphaValue = 0.5f;
			Assert.AreEqual(0.5f, entity.AlphaValue, 0.05f);
			entity.Rotation = MathExtensions.Pi;
			Assert.AreEqual(MathExtensions.Pi, entity.Rotation);
			entity.RenderLayer = 10;
			Assert.AreEqual(10, entity.RenderLayer);
			entity.RenderLayer = 1;
			Assert.AreEqual(1, entity.RenderLayer);
		}

		[Test]
		public void SetAndGetEntity2DComponentsDirectly()
		{
			var entity = new Entity2D(DoubleSizedRectangle, Color.Red);
			entity.Set(Color.Green);
			Assert.AreEqual(Color.Green, entity.Get<Color>());
			entity.Set(Rectangle.One);
			Assert.AreEqual(Rectangle.One, entity.Get<Rectangle>());
			entity.Set(5.0f);
			Assert.AreEqual(5.0f, entity.Get<float>());
			entity.RenderLayer = -10;
			Assert.AreEqual(-10, entity.RenderLayer);
		}

		[Test]
		public void SaveAndLoadFromMemoryStream()
		{
			var entity = new Entity2D(Rectangle.Zero);
			var data = entity.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(82, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity2D;
			Assert.AreEqual(entity.Tag, loadedEntity.Tag);
			Assert.AreEqual(3, loadedEntity.NumberOfComponents);
			Assert.IsTrue(loadedEntity.IsActive);
		}
	}
}