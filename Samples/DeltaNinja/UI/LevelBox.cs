using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
	class LevelBox
	{
		public LevelBox(ContentLoader content, NumberFactory numberFactory, float left, float top)
		{
			caption = new Sprite(content.Load<Image>("LevelCaption"), Rectangle.FromCenter(left, top + 0.02f, 0.04f, 0.02f), DefaultColors.Yellow);
			caption.RenderLayer = (int) GameRenderLayer.Score;
			number = numberFactory.CreateNumber(left, top + caption.DrawArea.Height, 0.019f, Alignment.Center, 2);	
		}

		private Sprite caption;
		private Number number;

		public void Show()
		{
			caption.Visibility = DeltaEngine.Rendering.Visibility.Show;
			number.Show(1);
		}

		public void Hide()
		{
			caption.Visibility = DeltaEngine.Rendering.Visibility.Hide;
			number.Hide();
		}

		public int Value
		{
			set
			{
				number.SetValue(value);
			}
		}

		public float Top
		{
			set
			{
				caption.TopLeft = new Point(caption.TopLeft.X, value);
				number.Top = value + caption.DrawArea.Height;
			}
		}
	}
}
