using System;
using DeltaEngine.Core;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Renders anything automatically when added to renderer until it is removed again via Dispose.
	/// </summary>
	public abstract class Renderable : IDisposable
	{
		protected Renderable()
		{
			IsVisible = true;
			RenderLayer = 0;
		}

		public bool IsVisible { get; set; }

		public int RenderLayer
		{
			get { return renderLayer; }
			set
			{
				renderLayer = value;
				HasRenderLayerChanged = true;
			}
		}

		private int renderLayer;
		internal bool HasRenderLayerChanged { get; set; }
		public const int MinRenderLayer = -99;
		public const int MaxRenderLayer = 100;

		protected abstract void Render(Renderer renderer, Time time);

		internal void InternalRender(Renderer renderer, Time time)
		{
			Render(renderer, time);
		}

		public virtual void Dispose()
		{
			IsVisible = false;
			markForDisposal = true;
		}

		internal bool markForDisposal;
	}
}