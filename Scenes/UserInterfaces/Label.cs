using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// The simplest UI control which simply draws a Sprite on screen.
	/// </summary>
	public class Label : Control
	{
		public Label(Image image, Rectangle initialDrawArea)
		{
			Sprite = new Sprite(image, initialDrawArea);
		}

		public Sprite Sprite { get; private set; }

		private Label() {}


		internal override void Show()
		{
			Sprite.Visibility = Visibility.Show;
		}

		internal override void Hide()
		{
			Sprite.Visibility = Visibility.Hide;
		}

		public override void Dispose()
		{
			Sprite.IsActive = false;
		}
	}
}