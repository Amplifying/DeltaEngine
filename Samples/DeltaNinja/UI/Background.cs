using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja
{
   class Background : Sprite
   {
      public Background(ScreenSpace screen)
         : base(ContentLoader.Load<Image>("Background"), screen.Viewport)
      {
         RenderLayer = (int) GameRenderLayer.Background;
         screen.ViewportSizeChanged += () => DrawArea = screen.Viewport;
      }
   }
}
