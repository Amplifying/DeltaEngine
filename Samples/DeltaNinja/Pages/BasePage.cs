using System.Diagnostics;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes;
using System;
using System.Collections.Generic;

namespace DeltaNinja.Pages
{
	public abstract class BasePage : Menu
	{
		internal const float TITLE_TopMargin = 0.02f;
		// internal const float LINKS_BottomMargin = 0.03f;

		public event Action<MenuButton> ButtonClicked;

		public BasePage(ScreenSpace screen, InputCommands input)
			: base(new Size(0.2f, 0.2f / 4f))
		{
			this.screen = screen;
			links = new List<LogoLink>();

			input.AddMouseMovement(m => OnMouseMovement(m));
			input.Add(MouseButton.Left, m => OnMouseClick(m));
		}

		private readonly ScreenSpace screen;
		private readonly List<LogoLink> links;
		
		public void SetTitle(string imageName, float width, float ratio, float topOffset)
		{
			var view = screen.Viewport;

			float center = view.Width / 2f;
			float offset = view.Top + topOffset;

			var title = new Sprite(ContentLoader.Load<Image>(imageName), new Rectangle(0,0, width, width / ratio));
			title.Center = new Point(center, offset + TITLE_TopMargin + title.DrawArea.Height / 2f);
			title.RenderLayer = (int)GameRenderLayer.Menu;

			Add(title);			
		}

		public void AddLogoLink(string imageName, string link, float size, int offset)
		{
			var view = screen.Viewport;

			float center = (view.Width / 2f) + (offset * size);

			var logo = new LogoLink(ContentLoader.Load<Image>(imageName), link, size);
			logo.Center = new Point(center, view.Bottom - size);
			logo.RenderLayer = (int)GameRenderLayer.Menu;

			links.Add(logo);			
			Add(logo);			
		}		

		public void AddButton(MenuButton code, float width, float ratio)
		{
			var image = ContentLoader.Load<Image>(code.ToString() + "Button");

			var theme = new Theme
			{
				Button = new Theme.Appearance(image),
				ButtonMouseover = new Theme.Appearance(image, DefaultColors.HoverButton),
				ButtonPressed = new Theme.Appearance(image),
				Font = new Font("Verdana12")
			};

			AddMenuOption(theme, () => { OnButtonClicked(code); });
		}

		protected virtual void OnButtonClicked(MenuButton code)
		{
			var handler = ButtonClicked;

			if (handler != null)
			{				
				handler(code);
			}
		}

		private void OnMouseClick(Mouse m)
		{
			foreach (var link in links)
			{
				if (link.IsHover(m.Position))
				{
					try
					{
						Process.Start(link.Url);
					}
					catch (Exception ex)
					{
						screen.Window.ShowMessageBox("Open link error", ex.ToString(), DeltaEngine.Platforms.MessageBoxButton.Okay);
					}
				}
			}
		}

		private void OnMouseMovement(Mouse m)
		{
			foreach (var link in links)
			{
				if (link.IsHover(m.Position))
					link.Color = DefaultColors.Yellow;
				else
					link.Color = Color.White;
			}
		}
	}
}
