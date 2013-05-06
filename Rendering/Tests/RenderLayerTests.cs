using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class RenderLayerTests
	{
		[Test]
		public void NewRenderLayer()
		{
			var renderLayer = new RenderLayer();
			Assert.AreEqual(Entity2D.DefaultRenderLayer, renderLayer.Value);
		}

		[Test]
		public void ChangeRenderLayer()
		{
			var renderLayer = new RenderLayer(-10);
			Assert.AreEqual(-10, renderLayer.Value);
			renderLayer.Value = 10;
			Assert.AreEqual(10, renderLayer.Value);
		}
	}
}