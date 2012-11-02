using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Allows to draw shapes or images on screen. Needs a graphic device.
	/// </summary>
	public abstract class Drawing : IDisposable
	{
		protected Drawing(Device device)
		{
			GraphicsDevice = device;
		}

		protected Device GraphicsDevice { get; private set; }

		public abstract void SetColor(Color color);
		public abstract void DrawRectangle(Rectangle area);

		public virtual void Dispose() {}
	}
}