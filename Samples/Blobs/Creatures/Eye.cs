using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;

namespace Blobs.Creatures
{
	/// <summary>
	/// Controls either eye of a Blob
	/// </summary>
	public class Eye : IDisposable
	{
		public Eye(ScreenSpace screen, InputCommands input, Mood mood)
		{
			camera = screen as Camera2DControlledQuadraticScreenSpace;
			this.mood = mood;
			CreateEyeElements();
			input.AddMouseMovement(StoreMousePosition);
		}

		private readonly Camera2DControlledQuadraticScreenSpace camera;
		private readonly Mood mood;

		private void CreateEyeElements()
		{
			eyeball = new Ellipse(Rectangle.Zero, Color.White) { RenderLayer = 12, Color = Color.White };
			eyeball.Add(new OutlineColor(Color.Black)).Start<Polygon.RenderOutline>();
			pupil = new Ellipse(Rectangle.Zero, Color.White) { RenderLayer = 13, Color = Color.Black };
			blink = new Line2D(Point.Zero, Point.Zero, Color.Black) { RenderLayer = 13 };
		}

		private Ellipse eyeball;
		private Ellipse pupil;
		private Line2D blink;

		private void StoreMousePosition(Mouse mouse)
		{
			mousePosition = mouse.Position;
			PositionElements();
		}

		private Point mousePosition;

		public Point Center
		{
			get { return center; }
			set
			{
				if (center == value)
					return;

				center = value;
				PositionElements();
			}
		}

		private Point center;

		private void PositionElements()
		{
			PositionPupil();
			PositionEyeball();
			PositionBlink();
		}

		private void PositionPupil()
		{
			SetPupilPosition();
			SetPupilRadius();
			pupil.Visibility = isBlinking ? Visibility.Hide : Visibility.Show;
		}

		public bool IsBlinking
		{
			get { return isBlinking; }
			set
			{
				if (isBlinking == value)
					return;

				isBlinking = value;
				PositionElements();
			}
		}

		private bool isBlinking;

		public float Rotation
		{
			get { return eyeball.Rotation; }
			set
			{
				if (eyeball.Rotation == value)
					return;

				eyeball.Rotation = value;
				PositionElements();
			}
		}

		private void SetPupilPosition()
		{
			if (camera == null)
				return;

			Point translatedCenter = camera.Transform(center);
			float distance = translatedCenter.DistanceTo(mousePosition);
			if (distance.IsNearlyEqual(0.0f))
			{
				pupil.Center = center;
				return;
			}

			var direction = translatedCenter.DirectionTo(mousePosition);
			direction /= distance;
			pupil.Center = center + 0.5f * direction * pupil.Radius;
		}

		private void SetPupilRadius()
		{
			pupil.Radius = 0.5f * RadiusY;
		}

		private void PositionEyeball()
		{
			eyeball.Center = center;
			eyeball.RadiusX = radiusX;
			eyeball.RadiusY = radiusY * (0.75f + 0.25f * mood.Fear);
			eyeball.Visibility = isBlinking ? Visibility.Hide : Visibility.Show;
		}

		private void PositionBlink()
		{
			blink.StartPoint = center - new Point(radiusX, 0.0f);
			blink.StartPoint = blink.StartPoint.RotateAround(Center, Rotation);
			blink.EndPoint = center + new Point(radiusX, 0.0f);
			blink.EndPoint = blink.EndPoint.RotateAround(Center, Rotation);
			blink.Visibility = isBlinking ? Visibility.Show : Visibility.Hide;
		}

		public float RadiusX
		{
			get { return radiusX; }
			set
			{
				if (radiusX == value)
					return;

				radiusX = value;
				PositionElements();
			}
		}

		private float radiusX;

		public float RadiusY
		{
			get { return radiusY; }
			set
			{
				if (radiusY == value)
					return;

				radiusY = value;
				PositionElements();
			}
		}

		private float radiusY;

		public void Dispose()
		{
			eyeball.IsActive = false;
			pupil.IsActive = false;
			blink.IsActive = false;
		}
	}
}