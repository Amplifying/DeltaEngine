using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces;

namespace CreepyTowers
{
	public class MainMenu : Scene
	{
		public MainMenu(ContentLoader content, ScreenSpace screenSpace)
		{
			contentLoader = content;
			Screen = screenSpace;
			SetupPlayArea();
			AddMenuBackground();
			AddCreepyTowersLogo();
			AddPlayButtonn();
			//AddOptionButton();
			//AddHighScoreButton();
			//AddCreditsButton();
			//AddExitButton();
		}

		private readonly ContentLoader contentLoader;
		public ScreenSpace Screen { get; private set; }

		private void SetupPlayArea()
		{
			Screen.Window.TotalPixelSize = new Size(800, 600);
			Screen.Window.Title = "CreepyTowers";
		}

		private void AddMenuBackground()
		{
			var menuBackground = contentLoader.Load<Image>("MainMenu");
			Background = new Sprite(menuBackground, Rectangle.One);
			Background.RenderLayer = (int)RenderLayer.Background;
		}

		public Sprite Background { get; private set; }

		private void AddCreepyTowersLogo()
		{
			var creepyTowersLogoImage = contentLoader.Load<Image>("CreepyTowersLogo");
			var imageSize = CalculateImageSizePreservingAspectRatio(creepyTowersLogoImage, 0.35f);
			var logoDrawArea = CalculateNewLogoDrawArea(Screen.Viewport, imageSize);
			CreepyTowersLogo = new Sprite(creepyTowersLogoImage, logoDrawArea);
			Screen.ViewportSizeChanged += () => AdjustLogoDrawArea(imageSize);
		}

		private static Size CalculateImageSizePreservingAspectRatio(Image image, float width)
		{
			var imageAspect = image.PixelSize.Width / image.PixelSize.Height;
			return new Size(width, width / imageAspect);
		}

		private static Rectangle CalculateNewLogoDrawArea(Rectangle referenceDrawArea, Size imageSize)
		{
			var centerPoint = new Point(referenceDrawArea.Center.X,
				referenceDrawArea.Top + imageSize.Height / 2);
			return Rectangle.FromCenter(centerPoint, imageSize);
		}

		public Sprite CreepyTowersLogo { get; private set; }

		private void AdjustLogoDrawArea(Size imageSize)
		{
			var centerPoint = new Point(Screen.Viewport.Center.X,
				Screen.Viewport.Top + imageSize.Height / 2);
			CreepyTowersLogo.DrawArea = Rectangle.FromCenter(centerPoint, imageSize);
		}

		private void AddPlayButtonn()
		{
			var playButtonImage = contentLoader.Load<Image>("ButtonPlay");
			var imageSize = CalculateImageSizePreservingAspectRatio(playButtonImage, 0.20f);
			var buttonDrawArea = CalculateNewButtonDrawArea(CreepyTowersLogo.DrawArea, imageSize);
			PlayButton = new Button(CreateTheme(playButtonImage), buttonDrawArea);
			Screen.ViewportSizeChanged +=
				() => AdjustButtonDrawArea(PlayButton, imageSize, buttonDrawArea);
		}

		private static Rectangle CalculateNewButtonDrawArea(Rectangle referenceDrawArea, Size imageSize)
		{
			var centerPoint = new Point(referenceDrawArea.Center.X,
				referenceDrawArea.Center.Y + imageSize.Height / 2 + referenceDrawArea.Size.Height / 2);
			return Rectangle.FromCenter(centerPoint, imageSize);
		}

		public Button PlayButton { get; private set; }

		private static void AdjustButtonDrawArea(Button imageSprite, Size imageSize,
			Rectangle previousImageDrawArea)
		{
			var centerPoint = new Point(previousImageDrawArea.Center.X,
				previousImageDrawArea.Center.Y + previousImageDrawArea.Size.Height / 2 +
					imageSize.Height / 2);
			imageSprite.DrawArea = Rectangle.FromCenter(centerPoint, imageSize);
		}

		private Theme CreateTheme(Image buttonImage)
		{
			var appearance = new Theme.Appearance(buttonImage);
			return new Theme
			{
				Button = appearance,
				ButtonMouseover = appearance,
				ButtonPressed = appearance,
				Font = new Font(contentLoader, "Verdana12")
			};
		}

		//private void AddOptionButton()
		//{
		//	var optionsButtonImage = contentLoader.Load<Image>("ButtonOptions");
		//	AddMenuOption(CreateTheme(optionsButtonImage), () => { });
		//}

		//private void AddHighScoreButton()
		//{
		//	var highScoresButtonImage = contentLoader.Load<Image>("ButtonHighscore");
		//	AddMenuOption(CreateTheme(highScoresButtonImage), () => { });
		//}

		//private void AddCreditsButton()
		//{
		//	var creditsButtonImage = contentLoader.Load<Image>("ButtonCredits");
		//	AddMenuOption(CreateTheme(creditsButtonImage), () => { });
		//}

		//private void AddExitButton()
		//{
		//	var exitButtonImage = contentLoader.Load<Image>("ButtonExit");
		//	AddMenuOption(CreateTheme(exitButtonImage), () =>
		//	{
		//		Clear();
		//		ExitGame();
		//	});
		//}

		//public event Action ExitGame;

		public void Dispose()
		{
			PlayButton.IsActive = false;
			Background.IsActive = false;
			CreepyTowersLogo.IsActive = false;
			Controls.Clear();
		}
	}
}