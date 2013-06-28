using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShot
{
	public class Game
	{
		public Game(ScreenSpace screen, InputCommands input)
		{
			this.screen = screen;
			this.input = input;
			InitializeGame();
		}

		private readonly ScreenSpace screen;
		private readonly InputCommands input;

		private void InitializeGame()
		{
			SetupPlayArea();
			SetupShip();
			SetupController();
			new GameInputControls(input, Ship);
		}

		private void SetupPlayArea()
		{
			screen.Window.ViewportPixelSize = new Size(1600, 900);
			screen.Window.Title = "ShadowShot Game";
			AddBackground();
		}

		private void AddBackground()
		{
			Background = new Sprite(ContentLoader.Load<Image>("starfield"), Rectangle.One);
			Background.RenderLayer = (int)Constants.RenderLayer.Background;
		}

		public Sprite Background { get; private set; }

		private void SetupShip()
		{
			var image = ContentLoader.Load<Image>("player");
			ComputeSizeAndDrawArea(image);
			var drawArea = Rectangle.FromCenter(drawAreaMidPoint, objectSize);
			Ship = new PlayerShip(image, drawArea);
		}

		private Point drawAreaMidPoint;
		private Size objectSize;
		public PlayerShip Ship { get; private set; }

		private void ComputeSizeAndDrawArea(Image image)
		{
			var offset = screen.Viewport.BottomRight.Y / 20;
			drawAreaMidPoint = new Point(screen.Viewport.BottomRight.X / 2,
				screen.Viewport.BottomRight.Y - offset);
			objectSize = new Size(image.PixelSize.Width / screen.Window.TotalPixelSize.Width,
				image.PixelSize.Height / (screen.Window.TotalPixelSize.Height * 2));
		}

		private void SetupController()
		{
			var image = ContentLoader.Load<Image>("asteroid");
			ComputeSizeAndDrawArea(image);
			Controller = new GameController(Ship, image, objectSize);
			Controller.ShipCollidedWithAsteroid += RestartGame;
		}

		public GameController Controller { get; private set; }

		public void RestartGame()
		{
			Controller.Dispose();
			InitializeGame();
		}
	}
}