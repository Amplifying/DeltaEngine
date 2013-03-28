using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Consists of any number of UI controls (labels, buttons), saved to and restored from a Stream.
	/// </summary>
	public class Scene : BinaryData, IDisposable
	{
		public Scene()
		{
			Controls = new List<Control>();
		}

		internal List<Control> Controls { get; private set; }

		public void Show(Renderer setRenderer, Content setContent, InputCommands setInput)
		{
			if (isShown)
				return;

			isShown = true;
			input = setInput;
			content = setContent;
			renderer = setRenderer;
			RespondToInput();
			foreach (Control control in Controls)
				ShowControlIfSceneActive(control);
		}

		private bool isShown;
		private InputCommands input;
		private Content content;
		private Renderer renderer;

		private void RespondToInput()
		{
			if (mouseMovement == null)
				mouseMovement = input.AddMouseMovement(MouseMovement);

			if (mouseHover == null)
				mouseHover = input.AddMouseHover(MouseHover);

			if (leftMouseButtonPress == null)
				leftMouseButtonPress = input.Add(MouseButton.Left, State.Pressing, LeftMouseButtonPressed);

			if (leftMouseButtonRelease == null)
				leftMouseButtonRelease = input.Add(MouseButton.Left, LeftMouseButtonReleased);
		}

		private Command mouseMovement;
		private Command mouseHover;
		private Command leftMouseButtonPress;
		private Command leftMouseButtonRelease;

		private void LeftMouseButtonPressed(Mouse mouse)
		{
			foreach (InteractiveControl control in interactiveControls)
				if (control.DrawArea.Contains(mouse.Position))
					control.Press();
		}

		private readonly List<InteractiveControl> interactiveControls =
			new List<InteractiveControl>();

		private void LeftMouseButtonReleased(Mouse mouse)
		{
			foreach (InteractiveControl control in interactiveControls)
				if (control.IsPressed)
					if (control.DrawArea.Contains(mouse.Position))
						control.Tap();
					else
						control.Release();
		}

		private void MouseMovement(Mouse mouse)
		{
			foreach (InteractiveControl control in interactiveControls)
				ProcessMouseMovement(mouse, control);
		}

		private static void ProcessMouseMovement(Mouse mouse, InteractiveControl control)
		{
			if (control.IsHovering)
				control.StopHover();

			if (!control.IsInside && control.DrawArea.Contains(mouse.Position))
				control.Enter();
			else if (control.IsInside && !control.DrawArea.Contains(mouse.Position))
				control.Exit();
		}

		private void MouseHover(Mouse mouse)
		{
			foreach (InteractiveControl control in interactiveControls)
				if (control.DrawArea.Contains(mouse.Position))
					control.Hover();
		}

		private void ShowControlIfSceneActive(Control control)
		{
			if (!isShown)
				return;

			control.LoadContent(content);
			control.Show(renderer);
		}

		public void Hide()
		{
			if (!isShown)
				return;

			isShown = false;
			StopRespondingToInput();
			foreach (Control control in Controls)
				control.Hide(renderer);
		}

		private void StopRespondingToInput()
		{
			input.Remove(mouseMovement);
			input.Remove(mouseHover);
			input.Remove(leftMouseButtonPress);
			input.Remove(leftMouseButtonRelease);
		}

		public void Add(Control control)
		{
			if (!Controls.Contains(control))
				Controls.Add(control);

			var interactiveControl = control as InteractiveControl;
			if (interactiveControl != null && !interactiveControls.Contains(interactiveControl))
				interactiveControls.Add(interactiveControl);

			ShowControlIfSceneActive(control);
		}

		public void Remove(Control control)
		{
			Controls.Remove(control);
			control.Dispose();
			var interactiveControl = control as InteractiveControl;
			if (interactiveControl != null)
				interactiveControls.Remove(interactiveControl);
		}

		public Control Find(string controlName)
		{
			return Controls.FirstOrDefault(control => control.Name == controlName);
		}

		public void Clear()
		{
			if (isShown)
				foreach (var control in Controls)
					control.Hide(renderer);

			Controls.Clear();
			interactiveControls.Clear();
		}

		public void SaveData(BinaryWriter writer)
		{
			foreach (Control control in Controls)
				control.Save(writer);
		}

		public void LoadData(BinaryReader reader)
		{
			while (reader.BaseStream.Position < reader.BaseStream.Length)
				Add(reader.Create<Control>());
		}

		public void Dispose()
		{
			Hide();
		}
	}
}