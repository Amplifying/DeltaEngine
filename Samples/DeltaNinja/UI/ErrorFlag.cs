using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
	class ErrorFlag : Sprite
	{
		public ErrorFlag(float left, float width, float bottom)
			: base(ContentLoader.Load<Image>("ErrorIcon"), new Rectangle(left, bottom - width, width, width))
		{
			Color = DefaultColors.Red;
			this.Time = DeltaEngine.Core.Time.Current.Milliseconds;
			this.RenderLayer = (int)GameRenderLayer.Points;			
		}

		public readonly long Time;

		public void Fade()
		{
			AlphaValue -= GameSettigs.FadeStep;
		}

		public void Reset()
		{
			IsActive = false;
		}
	}
}
