using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputTypeEditor
	{
		public InputTypeEditor(InputEditorViewModel inputEditorViewModel)
		{
			this.inputEditorViewModel = inputEditorViewModel;
		}

		private readonly InputEditorViewModel inputEditorViewModel;

		public void ChangeExistingTypeInList(string adding, string key)
		{
			for (int i = 0; i < GetTriggersOfCommand().Count; i++)
				CheckWichTriggerTypeToChange(adding, key, i);

			inputEditorViewModel.UpdateTriggerList(inputEditorViewModel.SelectedCommand);
		}

		private void CheckWichTriggerTypeToChange(string adding, string key, int i)
		{
			var trigger = GetTriggersOfCommand()[i];
			if (trigger.GetType() == typeof(KeyTrigger))
				ChangeTriggerTypeInCommandList(adding, key, trigger);
			else if (trigger.GetType() == typeof(MouseButtonTrigger))
				ChangeMouseTriggerTypeInCommandList(adding, key, trigger);
			else if (trigger.GetType() == typeof(GamePadButtonTrigger))
				ChangeGamePadTriggerTypeInCommandList(adding, key, trigger);
		}

		private List<Trigger> GetTriggersOfCommand()
		{
			return GetCommands().GetAllTriggers(inputEditorViewModel.SelectedCommand);
		}

		private CommandList GetCommands()
		{
			return inputEditorViewModel.availableCommands;
		}

		private void ChangeTriggerTypeInCommandList(string adding, string key, Trigger trigger)
		{
			var newKeyTrigger = (KeyTrigger)trigger;
			if (newKeyTrigger.Key.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		private void CheckWhatTriggerType(string adding, Trigger trigger)
		{
			int index = GetTriggersOfCommand().IndexOf(trigger);
			if (adding == "Keyboard")
				ChanceTriggerToKeyTrigger(index);
			else if (adding == "MouseButton")
				ChanceTriggerToMouseTrigger(index);
			else if (adding == "Gamepad")
				ChanceTriggerToGamePadTrigger(index);
			else if (adding == "Touchpad")
				ChanceKeyTriggerToTouchPadTrigger(index);
		}

		private void ChanceTriggerToKeyTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (Key key in Enum.GetValues(typeof(Key)))
				foundFreeKey = CheckWichKeyForKeyTriggerToUse(foundFreeKey, key, index);
		}

		private bool CheckWichKeyForKeyTriggerToUse(bool foundFreeKey, Key key, int index)
		{
			if (foundFreeKey)
				return true;

			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfKeyIsALreadyUsed(newTrigger, key, keyAlreadyUsed);

			if (keyAlreadyUsed)
				return false;

			var keyTrigger = new KeyTrigger(key, State.Pressed);
			GetTriggersOfCommand()[index] = keyTrigger;

			return true;
		}

		private static bool CheckIfKeyIsALreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(KeyTrigger))
				return keyAlreadyUsed;

			var keyTrigger = (KeyTrigger)newTrigger;
			if (keyTrigger.Key.ToString() == key.ToString())
				keyAlreadyUsed = true;

			return keyAlreadyUsed;
		}

		private void ChanceTriggerToMouseTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
				foundFreeKey = CheckWichButtonForMouseButtonTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWichButtonForMouseButtonTriggerToUse(bool foundFreeKey, MouseButton button,
			int index)
		{
			if (foundFreeKey)
				return true;

			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfMouseButtonIsALreadyUsed(newTrigger, button, keyAlreadyUsed);

			if (keyAlreadyUsed)
				return false;

			var mouseButtonTrigger = new MouseButtonTrigger(button, State.Pressed);
			GetTriggersOfCommand()[index] = mouseButtonTrigger;

			return true;
		}

		private static bool CheckIfMouseButtonIsALreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(MouseButtonTrigger))
				return keyAlreadyUsed;

			var mouseButtonTrigger = (MouseButtonTrigger)newTrigger;
			if (mouseButtonTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;

			return keyAlreadyUsed;
		}

		private void ChanceTriggerToGamePadTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (GamePadButton button in Enum.GetValues(typeof(GamePadButton)))
				foundFreeKey = CheckWichButtonForGamePadTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWichButtonForGamePadTriggerToUse(bool foundFreeKey, GamePadButton button,
			int index)
		{
			if (foundFreeKey)
				return true;

			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfGamePadButtonIsALreadyUsed(newTrigger, button, keyAlreadyUsed);

			if (keyAlreadyUsed)
				return false;

			var mouseButtonTrigger = new GamePadButtonTrigger(button, State.Pressed);
			GetTriggersOfCommand()[index] = mouseButtonTrigger;

			return true;
		}

		private static bool CheckIfGamePadButtonIsALreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(GamePadButtonTrigger))
				return keyAlreadyUsed;

			var mouseButtonTrigger = (GamePadButtonTrigger)newTrigger;
			if (mouseButtonTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;

			return keyAlreadyUsed;
		}

		private void ChanceKeyTriggerToTouchPadTrigger(int index)
		{
			var touchpadTrigger = new TouchPressTrigger(State.Pressed);
			inputEditorViewModel.availableCommands.GetAllTriggers(inputEditorViewModel.SelectedCommand)
				[index] = touchpadTrigger;
		}

		private void ChangeMouseTriggerTypeInCommandList(string adding, string key, Trigger trigger)
		{
			var newMouseTrigger = (MouseButtonTrigger)trigger;
			if (newMouseTrigger.Button.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeGamePadTriggerTypeInCommandList(string adding, string key,
			Trigger trigger)
		{
			var newMouseTrigger = (GamePadButtonTrigger)trigger;
			if (newMouseTrigger.Button.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		public void ChangeExistingTypeInList(string adding)
		{
			for (int i = 0; i < GetTriggersOfCommand().Count; i++)
				CheckWhatTriggerType(adding, GetTriggersOfCommand()[i]);
			inputEditorViewModel.UpdateTriggerList(inputEditorViewModel.SelectedCommand);
		}
	}
}