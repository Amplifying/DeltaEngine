using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using System;
using System.Collections.Generic;

namespace DeltaNinja
{
   // TODO: must be a Menu but Scenes.Button throws Autofac.Core.Registration.ComponentNotRegisteredException.
   // NOTE: https://deltaengine.net/Forum/default.aspx?g=posts&t=1481
   abstract class BasePage
   {
      public event EventHandler ButtonClicked;

      public BasePage(ContentLoader content, ScreenSpace screen, InputCommands input)
      {
         this.content = content;
         this.screen = screen;
         this.input = input;

         screen.ViewportSizeChanged += () => Update();
      }

      private readonly ContentLoader content;
      private readonly ScreenSpace screen;
      private readonly InputCommands input;

      private Sprite title;
      private List<Button> buttons = new List<Button>();
      private bool isShown;

      public void SetTitle(string imageName, float width, float ratio)
      {
         title = new Sprite(content.Load<Image>(imageName), new Rectangle(0, 0, width, width / ratio));
         title.RenderLayer = (int)GameRenderLayer.Menu;
      }

      public void AddButton(MenuButton code, float width, float ratio)
      {
         Add(new Button(code, content.Load<Image>(code.ToString() + "Button"), new Rectangle(0, 0, width, width / ratio), input, OnButtonClicked));
         Update();
      }

      public void Add(Button button)
      {
         if (!buttons.Contains(button))
            buttons.Add(button);

         button.RenderLayer = (int)GameRenderLayer.Menu;
         button.IsActive = isShown;
      }

      public void Show()
      {
         if (title != null)
            title.IsActive = true;

         foreach (Button button in buttons)
            button.IsActive = true;

         isShown = true;
         Update();
      }

      public void Hide()
      {
         if (title != null)
            title.IsActive = false;

         foreach (Button button in buttons)
            button.IsActive = false;

         isShown = false;
      }

      private void Update()
      {
         if (!isShown) return;

         var view = screen.Viewport;

         float center = view.Width / 2f;
         float middle = view.Top + view.Height / 2f;
         float offset = view.Top;

         if (title != null)
            title.Center = new Point(center, offset += title.DrawArea.Height);

         foreach (Button button in buttons)
            button.Center = new Point(center, offset += (button.DrawArea.Height * 2));
      }

      protected void OnButtonClicked(object sender, EventArgs e)
      {
         var handler = ButtonClicked;

         if (handler != null)
         {
            handler(sender, e);
         }
      }
   }
}
