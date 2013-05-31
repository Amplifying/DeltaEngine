using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A Sprite which can also display text
	/// </summary>
	public class Label : Sprite
	{
		public Label(Theme theme, Rectangle drawArea, string text = "")
			: this(theme.Label, theme.Font, drawArea)
		{
			Text = text;
		}

		public string Text
		{
			get { return Get<FontText>().Text; }
			set { Get<FontText>().Text = value; }
		}

		internal Label(Theme.Appearance appearance, Font font, Rectangle drawArea)
			: this(appearance.Image, appearance.Color, font)
		{
			DrawArea = drawArea;
		}

		internal Label(Image image, Color color, Font font)
			: base(image, color)
		{
			Add(new FontText(font, "", Point.Zero));
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

		public class UpdateText : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				var keypress = message as InteractWithKeyboard.KeyPress;
				var label = entity as Label;
				if (keypress != null && HasFocus(label))
					ProcessKeyPress(label, keypress.Key);
			}

			private static bool HasFocus(Entity entity)
			{
				return entity != null && entity.Contains<Interact.State>() &&
					entity.Get<Interact.State>().HasFocus;
			}

			private static void ProcessKeyPress(Label label, Key key)
			{
				if (key >= Key.D0 && key <= Key.D9)
					label.Text += key.ToString()[1].ToString(CultureInfo.InvariantCulture);

				if (key >= Key.A && key <= Key.Z)
					label.Text += key;

				if (key == Key.Space)
					label.Text += " ";

				if (key == Key.BackSpace && label.Text.Length > 0)
					label.Text = label.Text.Substring(0, label.Text.Length - 1);
			}
		}
	}
}