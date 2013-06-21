using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;
using System;

namespace DeltaNinja
{
   class Button : Sprite
   {
      public event EventHandler ButtonClicked;

      public Button(MenuButton code, Image image, Rectangle area, InputCommands input, EventHandler handler)
         : base(image, area)
      {
         this.Code = code;

         if (handler != null)
            ButtonClicked += handler;

         input.AddMouseMovement(mouse => OnMouseMove(mouse));
         input.Add(MouseButton.Left, State.Pressing, mouse => OnMouseClick(mouse));
      }

      public MenuButton Code { get; private set; }

      private void OnMouseClick(Mouse mouse)
      {
         if (!IsActive) return;
         if (Visibility != DeltaEngine.Rendering.Visibility.Show) return;

         if (this.DrawArea.Contains(mouse.Position))
         {
            var handler = ButtonClicked;

            if (handler != null)
            {
               handler(this, new EventArgs());
            }
         }
      }

      private void OnMouseMove(Mouse mouse)
      {
         if (!IsActive) return;
         if (Visibility != DeltaEngine.Rendering.Visibility.Show) return;

         if (this.DrawArea.Contains(mouse.Position))
            this.Color = DefaultColors.HoverButton;
         else
				this.Color = DefaultColors.NormalButton;
      }
   }
}
