using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaNinja.Pages
{
	class PausePage : BasePage
	{
		public PausePage(ScreenSpace screen, InputCommands input)
			: base(screen, input)
		{
			SetBackground(ContentLoader.Load<Image>("PauseBackground"));

			SetTitle("Pause", 0.25f, 4f, 0.05f);

			AddButton(MenuButton.Resume, 0.2f, 4f);
			AddButton(MenuButton.NewGame, 0.2f, 4f);
			AddButton(MenuButton.Abort, 0.2f, 4f);						
		} 
	}
}
