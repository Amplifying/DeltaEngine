using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace DeltaNinja.Pages
{
	public class HomePage : BasePage
	{
		public HomePage(ScreenSpace screen, InputCommands input)
			: base(screen, input)
		{
			SetTitle("DeltaNinjaHome", 0.4f, 3f, 0f);
			AddLogoLink("DeltaEngineLink", "http://www.deltaengine.com/", 0.07f, -2);
			AddLogoLink("CodePlexLink", "http://deltaninja.codeplex.com/", 0.07f, 2);
			AddButton(MenuButton.NewGame, 0.2f, 4f);
			AddButton(MenuButton.About, 0.2f, 4f);
			AddButton(MenuButton.Exit, 0.2f, 4f);
			input.Add(MouseButton.Left, m => { CheckAboutBox(m.Position); });
			aboutBox = new Sprite(ContentLoader.Load<Image>("AboutBox"),
				Rectangle.FromCenter(Point.Half, new Size(0.38f, 0.38f * 0.6070f)));
			aboutBox.IsActive = false;
		}

		private readonly Sprite aboutBox;

		protected override void OnButtonClicked(MenuButton code)
		{
			if (code == MenuButton.About)
			{
				aboutBox.IsActive = true;
				return;
			}
			base.OnButtonClicked(code);
		}

		private void CheckAboutBox(Point position)
		{
			if (aboutBox.IsActive)
				if (aboutBox.DrawArea.Contains(position))
					aboutBox.IsActive = false;
		}
	}
}