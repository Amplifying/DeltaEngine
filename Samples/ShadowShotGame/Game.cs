using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShotGame
{
	public class Game
	{
		public Game(ScreenSpace screen, InputCommands input, ContentLoader content)
		{
			this.screen = screen;
			this.input = input;
			this.content = content;
			SetupPlayArea();
			SetupShip();
			SetupController();
			new GameInputControls(input, Ship);
		}

		private readonly ScreenSpace screen;
		private readonly InputCommands input;
		private readonly ContentLoader content;

		private void SetupPlayArea()
		{
			screen.Window.TotalPixelSize = new Size(1600, 900);
			screen.Window.Title = "ShadowShot Game";
			AddBackground();
		}

		private void AddBackground()
		{
			Background = new Sprite(content.Load<Image>("starfield"), Rectangle.One);
			Background.RenderLayer = (int)Constants.RenderLayer.Background;
		}

		public Sprite Background { get; private set; }

		private void SetupShip()
		{
			var image = content.Load<Image>("player");
			ComputeSizeAndDrawArea(image);
			var drawArea = Rectangle.FromCenter(drawAreaMidPoint, objectSize);
			Ship = new PlayerShip(image, drawArea, content);
		}

		private Point drawAreaMidPoint;
		private Size objectSize;
		public PlayerShip Ship { get; private set; }
		public GameController Controller { get; private set; }

		private void SetupController()
		{
			var image = content.Load<Image>("asteroid");
			ComputeSizeAndDrawArea(image);
			Controller = new GameController(Ship, image, objectSize);
		}

		private void ComputeSizeAndDrawArea(Image image)
		{
			var offset = screen.Viewport.BottomRight.Y / 20;
			drawAreaMidPoint = new Point(screen.Viewport.BottomRight.X / 2,
				screen.Viewport.BottomRight.Y - offset);
			objectSize = new Size(image.PixelSize.Width / screen.Window.TotalPixelSize.Width,
				image.PixelSize.Height / (screen.Window.TotalPixelSize.Height * 2));
		}

		
	}
}