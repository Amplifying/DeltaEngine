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
			id = maxId++;
			RenderLayer = DefaultRenderLayer;
		}

		private readonly int id;
		private static int maxId;
		public bool IsVisible { get; set; }
		public byte RenderLayer { get; set; }
		public const byte BackgroundRenderLayer = 16;
		public const byte DefaultRenderLayer = 64;
		public const byte UIRenderLayer = 128;
		public int SortKey
		{
			get { return (RenderLayer << 20) | id; }
		}

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