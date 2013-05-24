using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A UI Button but with text also
	/// </summary>
	public class TextButton : Button
	{
		public TextButton(Image background, ContentLoader content)
			: this(background, content, Color.White) {}

		public TextButton(Image background, ContentLoader content, Color color)
			: base(background, Rectangle.Zero, color)
		{
			Add(new FontText(new Font(content, "DefaultFont_12_16"), "", Point.Zero));
			Add<PositionText>();
		}

		public class PositionText : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (Entity2D entity in entities.OfType<Entity2D>().Where(e => e.Contains<FontText>()))
					PositionFontText(entity);
			}

			private static void PositionFontText(Entity2D entity)
			{
				Point center = entity.DrawArea.Center;
				var size = entity.Get<FontText>().Get<Size>();
				entity.Get<FontText>().DrawArea = Rectangle.FromCenter(center, size);
				entity.Get<FontText>().RenderLayer = entity.RenderLayer + 1;
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.High; }
			}
		}

		public string Text
		{
			get { return Get<FontText>().Text; }
			set { Get<FontText>().Text = value; }
		}
	}
}