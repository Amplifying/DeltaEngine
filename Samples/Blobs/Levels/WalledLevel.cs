using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs.Levels
{
	public abstract class WalledLevel : Level
	{
		protected WalledLevel(ScreenSpace screen, InputCommands input)
			: base(screen, input) {}

		protected Color Color;

		public override void Reset()
		{
			base.Reset();
			float platformWidth = camera.Viewport.Width / 10;
			float viewportWidth = camera.Viewport.Width + platformWidth;
			float platformHeight = camera.Viewport.Height / 10;
			float viewportHeight = camera.Viewport.Height + platformHeight;
			AddPlatform(
				Rectangle.FromCenter(camera.Viewport.Center.X, camera.Top, viewportWidth, platformHeight),
				0, Color);
			AddPlatform(
				Rectangle.FromCenter(camera.Viewport.Center.X, camera.Bottom, viewportWidth, platformHeight),
				0, Color);
			AddPlatform(
				Rectangle.FromCenter(camera.Left, camera.Viewport.Center.Y, viewportHeight, platformWidth),
				90, Color);
			AddPlatform(
				Rectangle.FromCenter(camera.Right, camera.Viewport.Center.Y, viewportHeight, platformWidth),
				90, Color);
		}
	}
}