using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
	class ErrorFlag : Sprite
	{
		public ErrorFlag(ContentLoader content, float left, float width, float bottom)
			: base(content.Load<Image>("ErrorIcon"), new Rectangle(left, bottom - width, width, width), DefaultColors.Red)
		{
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
