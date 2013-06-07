using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaNinja
{   
   class GameOverPage : BasePage
   {
      public GameOverPage(ContentLoader content, ScreenSpace screen, InputCommands input)
         : base(content, screen, input)
      {  
         SetTitle("GameOver", 0.3f, 4f);

         AddButton(MenuButton.Home, 0.2f, 4f);
         AddButton(MenuButton.Retry, 0.2f, 4f);
         AddButton(MenuButton.Exit, 0.2f, 4f);
      } 
   }
}
