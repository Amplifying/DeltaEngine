using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
   class Background : Sprite
   {
      public Background(ContentLoader content, ScreenSpace screen)
         : base(content.Load<Image>("Background"), screen.Viewport)
      {
         RenderLayer = (int) GameRenderLayer.Background;
         screen.ViewportSizeChanged += () => DrawArea = screen.Viewport;
      }
   }
}
