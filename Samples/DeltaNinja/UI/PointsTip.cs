using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
	class PointsTip
	{
		public PointsTip(NumberFactory numberFactory, Point center, int value)
		{
			Time = DeltaEngine.Core.Time.Current.Milliseconds;
			
			number = numberFactory.CreateNumber(null, center.X, center.Y, 0.02f, Alignment.Center, 0, GameRenderLayer.Points, DefaultColors.Yellow);
			number.Show(value);

			plus = new Sprite(ContentLoader.Load<Image>("Plus"), Rectangle.FromCenter(center.X - 0.01f - number.Width / 2f, center.Y + number.Height / 2f, 0.01f, 0.01f));
			plus.RenderLayer = (int)GameRenderLayer.Points;
			plus.Color = DefaultColors.Yellow;
		}

		public readonly long Time;
		private Sprite plus;
		private Number number;
		
		public void Fade()
		{
			plus.AlphaValue -= GameSettigs.FadeStep;
			number.Fade();
		}

		public void Reset()
		{
			plus.IsActive = false;
			number.Hide();
		}
	}
}
