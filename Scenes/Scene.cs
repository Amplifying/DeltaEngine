using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Consists of any number of UI controls (labels, buttons), saved to and restored from a Stream.
	/// </summary>
	public class Scene : IDisposable
	{
		public Scene()
		{
			Controls = new List<Control>();
		}

		internal List<Control> Controls { get; private set; }

		public void Show(EntitySystem setEntitySystem, ContentLoader setContent,
			InputCommands setInput)
		{
			if (isShown)
				return;

			isShown = true;
			input = setInput;
			content = setContent;
			entitySystem = setEntitySystem;
			RespondToInput();
			foreach (Control control in Controls)
				ShowControlIfSceneActive(control);
		}

		private bool isShown;
		private InputCommands input;
		private ContentLoader content;
		private EntitySystem entitySystem;

		private void RespondToInput()
		{
			AddMouseHandling();
			AddTouchHandling();
		}

		private void AddMouseHandling()
		{
			if (mouseMovement == null)
				mouseMovement = input.AddMouseMovement(MouseMovement);

			if (mouseHover == null)
				mouseHover = input.AddMouseHover(MouseHover);

			if (leftMouseButtonPress == null)
				leftMouseButtonPress = input.Add(MouseButton.Left, State.Pressing,
					mouse => PointerPressed(mouse.Position));

			if (leftMouseButtonRelease == null)
				leftMouseButtonRelease = input.Add(MouseButton.Left,
					mouse => PointerReleased(mouse.Position));
		}

		private Command mouseMovement;
		private Command mouseHover;
		private Command leftMouseButtonPress;
		private Command leftMouseButtonRelease;

		private void MouseMovement(Mouse mouse)
		{
			var interactiveControls = new List<InteractiveControl>(Controls.OfType<InteractiveControl>());
			foreach (var control in interactiveControls)
				ProcessMouseMovement(mouse, control);
		}

		private static void ProcessMouseMovement(Mouse mouse, InteractiveControl control)
		{
			if (control.IsHovering)
				control.StopHover();

			if (!control.IsInside && control.Contains(mouse.Position))
				control.Enter();
			else if (control.IsInside && !control.Contains(mouse.Position))
				control.Exit();
		}

		private void MouseHover(Mouse mouse)
		{
			var interactiveControls = new List<InteractiveControl>(Controls.OfType<InteractiveControl>());
			foreach (
				var control in interactiveControls.Where(control => control.Contains(mouse.Position)))
				control.Hover();
		}

		private void PointerPressed(Point position)
		{
			var interactiveControls = new List<InteractiveControl>(Controls.OfType<InteractiveControl>());
			foreach (var control in interactiveControls.Where(control => control.Contains(position)))
				control.Press();
		}

		private void PointerReleased(Point position)
		{
			var interactiveControls = new List<InteractiveControl>(Controls.OfType<InteractiveControl>());
			foreach (var control in interactiveControls.Where(control => control.IsPressed))
				if (control.Contains(position))
					control.Tap(position);
				else
					control.Release();
		}

		private void AddTouchHandling()
		{
			if (touchPress == null)
				touchPress = input.Add(State.Pressing, touch => PointerPressed(touch.GetPosition(0)));

			if (touchRelease == null)
				touchRelease = input.Add(State.Releasing, touch => PointerReleased(touch.GetPosition(0)));
		}

		private Command touchPress;
		private Command touchRelease;

		private void ShowControlIfSceneActive(Control control)
		{
			if (!isShown)
				return;

			control.Show();
		}

		public void Hide()
		{
			if (!isShown)
				return;

			isShown = false;
			StopRespondingToInput();
			foreach (Control control in Controls)
				control.Hide();
		}

		private void StopRespondingToInput()
		{
			input.Remove(mouseMovement);
			input.Remove(mouseHover);
			input.Remove(leftMouseButtonPress);
			input.Remove(leftMouseButtonRelease);
			input.Remove(touchPress);
			input.Remove(touchRelease);
		}

		public void Add(Control control)
		{
			if (!Controls.Contains(control))
				Controls.Add(control);

			//TODO: entitySystem.Add(control);
			//entitySystem.Add(Sprite);//TODO: should be done at the caller, not here
			ShowControlIfSceneActive(control);
		}

		public void Remove(Control control)
		{
			Controls.Remove(control);
			control.Dispose();
		}

		public Control Find(string controlName)
		{
			return Controls.FirstOrDefault(control => control.Name == controlName);
		}

		public void Clear()
		{
			if (isShown)
				foreach (var control in Controls)
					control.Hide();

			Controls.Clear();
		}

		public void Dispose()
		{
			foreach (var control in Controls)
				control.Dispose();
		}
	}
}