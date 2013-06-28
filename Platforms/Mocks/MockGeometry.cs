using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockGeometry : Geometry
	{
		public override void CreateFrom(GeometryData data) {}
		public override void Draw() {}
		protected override void DisposeData() {}
	}
}