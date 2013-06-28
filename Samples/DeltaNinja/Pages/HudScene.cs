using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;

namespace DeltaNinja.Pages
{
	internal class HudScene : Scene
	{
		public HudScene(ScreenSpace screen, NumberFactory numberFactory)
		{
			var view = screen.Viewport;
			var center = view.Width / 2f;
			var left = view.Left;
			var top = view.Top;
			var right = view.Right;
			pointsNumber = numberFactory.CreateNumber(this, left, top, 0.05f, Alignment.Left, 0);
			levelCaption = new Sprite(ContentLoader.Load<Image>("LevelCaption"),
				Rectangle.FromCenter(center, top + 0.02f, 0.07f, 0.03f));
			levelCaption.Color = DefaultColors.Yellow;
			Add(levelCaption);
			levelNumber = numberFactory.CreateNumber(this, center, levelCaption.DrawArea.Bottom, 0.022f,
				Alignment.Center, 2);
			var errImage = ContentLoader.Load<Image>("ErrorIcon");
			var offsets = new[,] { { 0.108f, 0.025f }, { 0.083f, 0.033f }, { 0.05f, 0.05f } };
			errorIcons = new Sprite[3];
			for (int i = 0; i < errorIcons.Length; i++)
				Add(
					errorIcons[i] =
						new Sprite(errImage,
							new Rectangle(right - offsets[i, 0], top, offsets[i, 1], offsets[i, 1])));

			// Layer
			foreach (var control in Controls.FindAll(x => x is Sprite))
				((Sprite)control).RenderLayer = (int)GameRenderLayer.Hud;
		}

		private readonly Number pointsNumber;
		private readonly Sprite levelCaption;
		private readonly Number levelNumber;
		private readonly Sprite[] errorIcons;

		public void SetError(int count)
		{
			errorIcons[count - 1].Color = DefaultColors.Red;
		}

		public void SetPoints(int count)
		{
			pointsNumber.SetValue(count);
		}

		public void SetLevel(int level)
		{
			levelNumber.SetValue(level);
		}

		public void Reset()
		{
			foreach (var errIcon in errorIcons)
				errIcon.Color = DefaultColors.Gray;

			SetPoints(0);
			SetLevel(1);
		}

		public void ArrageControls(float top)
		{
			pointsNumber.Top = top;

			levelCaption.TopLeft = new Point(levelCaption.TopLeft.X, top);
			levelNumber.Top = levelCaption.DrawArea.Bottom;

			foreach (var errIcon in errorIcons)
				errIcon.TopLeft = new Point(errIcon.TopLeft.X, top);
		}
	}
}