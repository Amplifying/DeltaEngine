using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Scenes;
using System.Globalization;

namespace DeltaNinja
{
   class NumberFactory
   {
      public NumberFactory()
      {
         for (int digit = 0; digit < 10; digit++)
            images[digit] = ContentLoader.Load<Image>(digit.ToString(CultureInfo.InvariantCulture));

         images[10] = ContentLoader.Load<Image>("Empty");
      }

      private readonly Image[] images = new Image[11];

      public Number CreateNumber(Scene scene, float left, float top, float height, Alignment align, int digitCount, GameRenderLayer layer = GameRenderLayer.Hud, Color? color = null)
      {
         return new Number(scene, images, left, top, height, align, digitCount, layer, color);
      }
   }
}
