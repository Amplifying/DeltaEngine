using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Allows to render shapes, meshes or images on screen. Needs a graphic device.
	/// </summary>
	public class Renderer : Runner, IDisposable
	{
		public Renderer(Drawing draw)
		{
			this.draw = draw;
		}

		private readonly Drawing draw;

		public void Run()
		{
			foreach (Renderable renderObject in visibles)
				renderObject.Render();
		}

		private readonly List<Renderable> visibles = new List<Renderable>();

		public void Add(Renderable renderable)
		{
			visibles.Add(renderable);
		}

		public void Remove(Renderable renderable)
		{
			visibles.Remove(renderable);
		}
		
		public void DrawRectangle(Rectangle rect, Color color)
		{
			draw.SetColor(color);
			draw.DrawRectangle(rect);
		}

		public void Dispose()
		{
			var visiblesToDispose = new List<Renderable>(visibles);
			foreach (Renderable renderObject in visiblesToDispose)
				renderObject.Dispose();
		}
	}
}