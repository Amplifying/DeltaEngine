using System;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Renders anything automatically until disposed. Used in Drawing and many Rendering classes.
	/// </summary>
	public abstract class Renderable : IDisposable
	{
		protected Renderable(Renderer renderer)
		{
			this.renderer = renderer;
			renderer.Add(this);
		}

		protected readonly Renderer renderer;

		public void Dispose()
		{
			renderer.Remove(this);
		}

		protected internal abstract void Render();
	}
}