using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaNinja.Pages
{   
	class GameOverPage : BasePage
	{
		public GameOverPage(ScreenSpace screen, InputCommands input)
			: base(screen, input)
		{  
			SetTitle("GameOver", 0.25f, 4f, 0.05f);

			AddButton(MenuButton.Home, 0.2f, 4f);
			AddButton(MenuButton.Retry, 0.2f, 4f);
			AddButton(MenuButton.Exit, 0.2f, 4f);
		} 
	}
}
