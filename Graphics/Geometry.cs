using System;

namespace DeltaEngine.Graphics
{
	public abstract class Geometry : IDisposable
	{
		public abstract void CreateFrom(GeometryData data);
		public abstract void Draw();

		protected abstract void DisposeData();

		public void Dispose()
		{
			DisposeData();
		}
	}
}