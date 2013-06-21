using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using System.Globalization;

namespace DeltaNinja
{
   class NumberFactory
   {
      public NumberFactory(ContentLoader content)
      {
         this.content = content;

         for (int digit = 0; digit < 10; digit++)
            images[digit] = content.Load<Image>(digit.ToString(CultureInfo.InvariantCulture));

         images[10] = content.Load<Image>("Empty");
      }

      private readonly ContentLoader content;
      private readonly Image[] images = new Image[11];

      public Number CreateNumber(float left, float top, float height, Alignment align, int digitCount, GameRenderLayer layer = GameRenderLayer.Score, Color? color = null)
      {
         return new Number(images, left, top, height, align, digitCount, layer, color);
      }
   }
}
