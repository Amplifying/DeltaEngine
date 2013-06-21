using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaNinja
{
   class HomePage : BasePage
   {
      public HomePage(ContentLoader content, ScreenSpace screen, InputCommands input) : base(content, screen, input)
      {
         SetTitle("DeltaNinjaHome", 0.4f, 3f);

         AddButton(MenuButton.NewGame, 0.2f, 4f);
         AddButton(MenuButton.Exit, 0.2f, 4f);
      }
   }
}
