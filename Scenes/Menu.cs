using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// A simple menu system. Moving the mouse over a menu option will change its size and color.
	/// Just set the size of a menu button and it will auto-arrange all button positions.
	/// </summary>
	public class Menu : Scene
	{
		public Menu(ContentLoader content, Size buttonSize)
			: this(content)
		{
			this.buttonSize = buttonSize;
		}

		private Size buttonSize;

		public Menu(ContentLoader content)
		{
			this.content = content;
		}

		private readonly ContentLoader content;

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

		private readonly List<TextButton> buttons = new List<TextButton>();

		internal List<TextButton> Buttons
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
			foreach (TextButton button in buttons)
				Remove(button);

			buttons.Clear();
		}

		public void AddMenuOption(Image image, Action clicked, string text = "")
		{
			AddButton(image, clicked, text);
			ArrangeButtons();
		}

		private void AddButton(Image image, Action clicked, string text)
		{
			var button = new TextButton(image, content)
			{
				NormalColor = Color.LightGray,
				MouseoverColor = Color.White,
				Text = text
			};
			button.Clicked += clicked;
			button.Add<ChangeSizeDynamically>();
			buttons.Add(button);
			Add(button);
		}

		public class ChangeSizeDynamically : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				var control = entity as Entity2D;
				var state = control.Get<Interact.State>();
				Rectangle drawArea = control.DrawArea;
				if (IsControlEnteredForTheFirstTime(message, state) ||
					IsClickCompletedInsideControl(message, state))
					MakeControlBigger(control, drawArea);
				else if (IsClickBegunOnControl(message) || IsControlLeftWithoutBeingClicked(message, state))
					MakeControlSmaller(control, drawArea);
			}

			private static bool IsControlEnteredForTheFirstTime(object message, Interact.State state)
			{
				return (!state.IsPressed && message is Interact.ControlEntered);
			}

			private static bool IsClickCompletedInsideControl(object message, Interact.State state)
			{
				return (state.IsInside &&
					(message is Interact.ControlReleased || message is Interact.ControlClicked));
			}

			private static void MakeControlBigger(Entity2D control, Rectangle drawArea)
			{
				control.DrawArea = Rectangle.FromCenter(drawArea.Center, drawArea.Size * Growth);
			}

			private const float Growth = 1.05f;

			private static bool IsControlLeftWithoutBeingClicked(object message, Interact.State state)
			{
				return (!state.IsPressed && message is Interact.ControlExited);
			}

			private static bool IsClickBegunOnControl(object message)
			{
				return message is Interact.ControlPressed;
			}

			private static void MakeControlSmaller(Entity2D control, Rectangle drawArea)
			{
				control.DrawArea = Rectangle.FromCenter(drawArea.Center, drawArea.Size / Growth);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Low; }
			}
		}
	}
}