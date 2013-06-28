using System.Globalization;
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
			: base(image, Rectangle.Zero)
		{
			Color = color;
			Add(new FontText(font, "", Point.Zero));
			Start<PositionText>();
		}

		private class PositionText : Behavior2D
		{
			public override void Handle(Entity2D label)
			{
				Point center = label.DrawArea.Center;
				var size = label.Get<FontText>().Get<Size>();
				label.Get<FontText>().DrawArea = Rectangle.FromCenter(center, size);
				label.Get<FontText>().RenderLayer = label.RenderLayer + 1;
			}

			public override Priority Priority
			{
				get { return Priority.High; }
			}
		}

		protected class UpdateText : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
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

				if (key == Key.Backspace && label.Text.Length > 0)
					label.Text = label.Text.Substring(0, label.Text.Length - 1);
			}
		}
	}
}