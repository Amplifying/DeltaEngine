using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace SideScrollerSample
{
	internal class ParallaxBackground : Entity
	{
		public ParallaxBackground(ContentLoader contentLoader, ScreenSpace screenSpace,
			float baseScrollSpeed = 1)
		{
			CreateLayers(contentLoader, screenSpace);
			Add<ParallaxScroller>();
			BaseScrollSpeed = baseScrollSpeed;
		}

		private void CreateLayers(ContentLoader contentLoader, ScreenSpace screenSpace)
		{
			//layerAlpha = new BackgroundLayer(contentLoader, "BgLowest", screenSpace, 0.2f);
			layerBeta = new BackgroundLayer(contentLoader, "BgMiddle", screenSpace, 0.4f);
			//layerGamma = new BackgroundLayer(contentLoader, "BgForemost", screenSpace,0.8f);
		}

		internal BackgroundLayer /*layerAlpha,*/ layerBeta /*, layerGamma*/;
		public float BaseScrollSpeed { get; set; }

		internal class BackgroundLayer
		{
			public BackgroundLayer(ContentLoader contentLoader, string layerImageName,
				ScreenSpace screenSpace, float factorToBaseSpeed)
			{
				FactorToBaseSpeed = factorToBaseSpeed;
				this.screenSpace = screenSpace;
				var layerImage = contentLoader.Load<Image>(layerImageName);
				var alphaDrawArea = new Rectangle(screenSpace.Viewport.TopLeft, screenSpace.Viewport.Size);
				var betaDrawArea = new Rectangle(screenSpace.Viewport.TopRight, screenSpace.Viewport.Size);
				SpriteAlpha = new Sprite(layerImage, alphaDrawArea);
				SpriteBeta = new Sprite(layerImage, betaDrawArea);
				SpriteAlpha.RenderLayer = (int)DefRenderLayer.Background + 1;
				SpriteBeta.RenderLayer = (int)DefRenderLayer.Background + 1;
			}

			internal Sprite SpriteAlpha, SpriteBeta;
			internal float FactorToBaseSpeed;
			internal ScreenSpace screenSpace;

			public void MoveLayer(float Offset)
			{
				var pointAlpha = GetFuturePointForLayer(SpriteAlpha.TopLeft, Offset);
				var pointBeta = GetFuturePointForLayer(SpriteBeta.TopLeft, Offset);

				if (pointAlpha.X < screenSpace.Viewport.Left)
					pointAlpha.X = pointBeta.X + SpriteBeta.Size.Width;
				else if (pointBeta.X < screenSpace.Viewport.Left)
					pointBeta.X = pointAlpha.X + SpriteAlpha.Size.Width;

				SpriteAlpha.TopLeft = pointAlpha;
				SpriteBeta.TopLeft = pointBeta;
			}

			private Point GetFuturePointForLayer(Point currentPoint, float offset)
			{
				return new Point(currentPoint.X - offset * FactorToBaseSpeed * Time.Current.Delta,
					currentPoint.Y);
			}
		}

		private class ParallaxScroller : EntityHandler
		{
			public ParallaxScroller()
			{
				Filter = entity => entity is ParallaxBackground;
			}

			public override void Handle(Entity entity)
			{
				var background = entity as ParallaxBackground;
				//background.layerAlpha.MoveLayer(background.BaseScrollSpeed);
				background.layerBeta.MoveLayer(background.BaseScrollSpeed);
				//background.layerGamma.MoveLayer(background.BaseScrollSpeed);
			}
		}
	}
}