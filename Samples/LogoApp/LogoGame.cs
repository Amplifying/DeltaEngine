using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;

namespace LogoApp
{
	/// <summary>
	/// Displays a colored rectangle in a simple 800x600 window. Will be changed soon to use images!
	/// </summary>
	public class LogoGame : Runner
	{
		public LogoGame(Renderer render, Time time, Window window)
		{
			box = new ColoredRectangle(render, Point.Half, Size.Half, Color.Red);
			this.time = time;
			this.window = window;
			window.BackgroundColor = Color.CornflowerBlue;
		}

		private readonly ColoredRectangle box;
		private readonly Time time;
		private readonly Window window;

		public void Run()
		{
			box.Rect.Center += moveDirection * time.CurrentDelta;
			if (box.Rect.Left <= 0.0f)
				SetMoveDirectionAndChangeColor(Point.UnitX);
			else if (box.Rect.Left > 0.5f)
				SetMoveDirectionAndChangeColor(-Point.UnitX);
		}

		private Point moveDirection = Point.UnitX;

		private void SetMoveDirectionAndChangeColor(Point direction)
		{
			window.Title = "LogoGame Fps: " + time.Fps;
			moveDirection = direction;
			box.Color = Color.GetRandomBrightColor();
		}
	}
}