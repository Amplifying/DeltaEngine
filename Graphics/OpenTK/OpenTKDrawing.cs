using DeltaEngine.Datatypes;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Graphics.OpenTK
{
	/// <summary>
	/// Allows to draw shapes on screen. Needs a graphic device.
	/// </summary>
	public class OpenTKDrawing : Drawing
	{
		public OpenTKDrawing(OpenTKDevice device)
			: base(device) {}

		public override void SetColor(Color color)
		{
			GL.Color3(color.R, color.G, color.B);
		}

		public override void DrawRectangle(Rectangle area)
		{
			GL.Begin(BeginMode.Quads);
			var points = new[]
			{
				GraphicsDevice.Screen.ToPixelSpace(area.TopLeft),
				GraphicsDevice.Screen.ToPixelSpace(area.TopRight),
				GraphicsDevice.Screen.ToPixelSpace(area.BottomRight),
				GraphicsDevice.Screen.ToPixelSpace(area.BottomLeft)
			};
			foreach (var point in points)
				GL.Vertex2(point.X, point.Y);
			GL.End();
		}
	}
}