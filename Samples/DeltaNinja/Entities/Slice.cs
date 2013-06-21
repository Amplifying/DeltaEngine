using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeltaNinja
{
	class Slice
	{
		long startTime;
		bool isActive = false;
		Point lastMousePosition;
		List<Line2D> segments = new List<Line2D>();

		public bool IsActive { get { return isActive; } }

		public bool IsOver
		{
			get
			{
				return isActive && Time.Current.Milliseconds - startTime >= 300;
			}
		}

		public void Reset()
		{
			foreach (var segment in segments)
				segment.IsActive = false;

			segments.Clear();
			isActive = false;
		}

		public void Switch(Point position)
		{
			if (isActive)
				Reset();
			else
			{
				lastMousePosition = position;
				isActive = true;
				startTime = Time.Current.Milliseconds;
			}
		}

		public void Check(Point position, IEnumerable<Logo> logoSet)
		{
			if (isActive)
			{
				foreach (var logo in logoSet)
					logo.CheckForSlicing(lastMousePosition, position);

				var segment = new Line2D(lastMousePosition, position, Color.Cyan);
				segment.RenderLayer = (int)GameRenderLayer.Segments;

				segments.Add(segment);
				segment.IsActive = true;

				lastMousePosition = position;
			}
		}
	}
}
