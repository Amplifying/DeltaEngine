using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// A simple menu system where all buttons are the same size and auto-arrange on screen.
	/// </summary>
	public class Menu : Scene
	{
		public Menu(Size buttonSize)
		{
			this.buttonSize = buttonSize;
		}

		private Size buttonSize;

		public Size ButtonSize
		{
			get { return buttonSize; }
			set
			{
				buttonSize = value;
				ArrangeButtons();
			}
		}

		private void ArrangeButtons()
		{
			var left = 0.5f - ButtonSize.Width / 2;
			for (int i = 0; i < buttons.Count; i++)
				buttons[i].DrawArea = new Rectangle(left, GetButtonTop(i), ButtonSize.Width,
					ButtonSize.Height);
		}

		private readonly List<ActiveButton> buttons = new List<ActiveButton>();

		internal List<ActiveButton> Buttons
		{
			get { return buttons; }
		}

		private float GetButtonTop(int button)
		{
			float gapHeight = ButtonSize.Height / 2;
			float totalHeight = buttons.Count * ButtonSize.Height + (buttons.Count - 1) * gapHeight;
			float top = 0.5f - totalHeight / 2;
			return top + button * (ButtonSize.Height + gapHeight);
		}

		public override void Clear()
		{
			base.Clear();
			buttons.Clear();
		}

		public void ClearMenuOptions()
		{
			foreach (ActiveButton button in buttons)
				Remove(button);

			buttons.Clear();
		}

		public void AddMenuOption(Theme theme, Action clicked, string text = "")
		{
			AddButton(theme, clicked, text);
			ArrangeButtons();
		}

		private void AddButton(Theme theme, Action clicked, string text)
		{
			var button = new ActiveButton(theme, new Rectangle(Point.Zero, ButtonSize), text);
			button.Clicked += clicked;
			buttons.Add(button);
			Add(button);
		}
	}
}