using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using DeltaEngine.Commands;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.InputEditor
{
	/// <summary>
	/// Edites the data for the ImputEditorView
	/// </summary>
	public class InputEditorViewModel : ViewModelBase
	{
		public InputEditorViewModel(Service service)
		{
			CommandList = new ObservableCollection<string>();
			inputNewTriggerEditor = new InputNewTriggerEditor();
			availableCommands = new CommandList();
			InputTypeEditor = new InputTypeEditor(this);
			inputKeyAndButtonEditor = new InputKeyAndButtonEditor(this);
			inputStateEditor = new InputStateEditor(this);
			TriggerList = new ObservableCollection<TriggerLayoutView>();
			inputSaverAndLoader = new InputSaverAndLoader();
			this.service = service;
			CheckForLoad();
			SetICommands();
		}

		public ObservableCollection<string> CommandList { get; private set; }
		public Service service;

		private void SetICommands()
		{
			AddCommand = new RelayCommand(AddNewCommand);
			AddTrigger = new RelayCommand(AddNewTrigger);
			RemoveCommand = new RelayCommand(RemoveSelectedCommand);
			EditCommand = new RelayCommand(EditSelectedCommand);
			RemoveTrigger = new RelayCommand(RemoveSelectedTrigger);
			Messenger.Default.Register<TriggerLayoutData>(this, "KeyChanged", ChangeKey);
			Messenger.Default.Register<TriggerLayoutData>(this, "StateChanged", ChangeState);
			Messenger.Default.Register<TriggerLayoutData>(this, "TypeChanged", ChangeType);
			Messenger.Default.Register<string>(this, "SaveCommands", SaveAsXml);
		}

		public ICommand AddCommand { get; private set; }
		public ICommand AddTrigger { get; private set; }
		public ICommand RemoveCommand { get; private set; }
		public ICommand EditCommand { get; private set; }
		public ICommand RemoveTrigger { get; private set; }
		private readonly InputNewTriggerEditor inputNewTriggerEditor;
		public readonly CommandList availableCommands;
		private readonly InputKeyAndButtonEditor inputKeyAndButtonEditor;
		private readonly InputStateEditor inputStateEditor;
		public ObservableCollection<TriggerLayoutView> TriggerList { get; private set; }
		private readonly InputSaverAndLoader inputSaverAndLoader;

		public void AddNewCommand()
		{
			if (string.IsNullOrEmpty(NewCommand))
				return;

			availableCommands.AddCommand(NewCommand);
			EditListInView();
			RaisePropertyChanged("CommandList");
		}

		public string NewCommand { get; set; }

		private void EditListInView()
		{
			CommandList.Clear();
			foreach (var command in availableCommands.GetCommands())
				CommandList.Add(command);
		}

		public void AddNewTrigger()
		{
			if (string.IsNullOrEmpty(SelectedCommand))
				return;

			var triggerLayoutView = new TriggerLayoutView();
			inputNewTriggerEditor.CreateNewTriggerBox(availableCommands, SelectedCommand,
				triggerLayoutView);
			TriggerList.Add(triggerLayoutView);
			RaisePropertyChanged("TriggerList");
		}

		private void RemoveSelectedCommand()
		{
			if (string.IsNullOrEmpty(SelectedCommand))
				return;

			availableCommands.RemoveCommand(SelectedCommand);
			CommandList.Remove(selectedCommand);
			triggerList.Clear();
		}

		private void EditSelectedCommand()
		{
			if (string.IsNullOrEmpty(NewCommand))
				return;

			availableCommands.EditCommand(SelectedCommand, NewCommand);
			int index = CommandList.IndexOf(SelectedCommand);
			CommandList[index] = NewCommand;
		}

		private void RemoveSelectedTrigger()
		{
			if (SelectedTrigger == null || string.IsNullOrEmpty(SelectedCommand))
				return;

			var indexToRemove = TriggerList.IndexOf(SelectedTrigger);
			availableCommands.GetAllTriggers(selectedCommand).RemoveAt(indexToRemove);
			TriggerList.RemoveAt(indexToRemove);
		}

		public TriggerLayoutView SelectedTrigger { get; set; }

		public void UpdateTriggerList(string value)
		{
			selectedCommand = value;
			TriggerList.Clear();
			List<Trigger> triggersForSelectedCommand = availableCommands.GetAllTriggers(selectedCommand);
			foreach (var trigger in triggersForSelectedCommand)
				CheckWichTriggerToAdd(trigger);
		}

		private void CheckWichTriggerToAdd(Trigger trigger)
		{
			if (trigger.GetType() == typeof(KeyTrigger))
				AddKeyTriggerToList(trigger);
			if (trigger.GetType() == typeof(MouseButtonTrigger))
				AddMouseTriggerToList(trigger);
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
				AddGamePadTriggerToList(trigger);
			if (trigger.GetType() == typeof(TouchPressTrigger))
				AddTouchPadTriggerToList(trigger);
		}

		public string SelectedCommand
		{
			get { return selectedCommand; }
			set { UpdateTriggerList(value); }
		}

		private string selectedCommand;

		private void AddKeyTriggerToList(Trigger trigger)
		{
			var newKeyTrigger = (KeyTrigger)trigger;
			var triggerLayoutView = new TriggerLayoutView();
			inputNewTriggerEditor.SetKeyTrigger(newKeyTrigger.Key, newKeyTrigger.State,
				triggerLayoutView);
			TriggerList.Add(triggerLayoutView);
		}

		public InputTypeEditor InputTypeEditor { get; set; }
		public InputKeyAndButtonEditor InputKeyAndButtonEditor
		{
			get { return inputKeyAndButtonEditor; }
		}
		public InputStateEditor InputStateEditor
		{
			get { return inputStateEditor; }
		}

		private readonly ObservableCollection<TriggerLayoutView> triggerList =
			new ObservableCollection<TriggerLayoutView>();

		private void AddMouseTriggerToList(Trigger trigger)
		{
			var newMouseTrigger = (MouseButtonTrigger)trigger;
			var triggerLayoutView = new TriggerLayoutView();
			inputNewTriggerEditor.SetMouseButtonTrigger(newMouseTrigger.Button, newMouseTrigger.State,
				triggerLayoutView);
			TriggerList.Add(triggerLayoutView);
		}

		private void AddGamePadTriggerToList(Trigger trigger)
		{
			var newGamepadTrigger = (GamePadButtonTrigger)trigger;
			var triggerLayoutView = new TriggerLayoutView();
			inputNewTriggerEditor.SetGamePadTrigger(newGamepadTrigger.Button, newGamepadTrigger.State,
				triggerLayoutView);
			TriggerList.Add(triggerLayoutView);
		}

		private void AddTouchPadTriggerToList(Trigger trigger)
		{
			var newTouchpadTrigger = (TouchPressTrigger)trigger;
			var triggerLayoutView = new TriggerLayoutView();
			inputNewTriggerEditor.SetTouchPadTrigger(newTouchpadTrigger.State, triggerLayoutView);
			TriggerList.Add(triggerLayoutView);
		}

		public void ChangeKey(TriggerLayoutData e)
		{
			if (e.NumberOfAddingItems == 0 || e.NumberOfRemovingItems == 0)
				return;

			InputKeyAndButtonEditor.ChangeExistingKeyInList(e.AddingItem, e.RemovingItem);
		}

		public void ChangeState(TriggerLayoutData e)
		{
			if (e.NumberOfAddingItems == 0 || e.NumberOfRemovingItems == 0)
				return;

			if (e.TypeBox.SelectedItem.ToString() != "Touchpad")
				InputStateEditor.ChangeExistingStateInList(e.AddingItem, e.KeyBox.SelectedItem.ToString());
			else
				InputStateEditor.ChangeExistingStateInList(e.AddingItem);
		}

		public void ChangeType(TriggerLayoutData e)
		{
			if (e.NumberOfAddingItems == 0 || e.NumberOfRemovingItems == 0)
				return;
			if (e.RemovingItem != "Touchpad")
				InputTypeEditor.ChangeExistingTypeInList(e.AddingItem, e.KeyBox.SelectedItem.ToString());
			else
				InputTypeEditor.ChangeExistingTypeInList(e.AddingItem);
		}

		public void SaveAsXml(string obj)
		{
			inputSaverAndLoader.SaveInput(availableCommands, service);
		}

		public void CheckForLoad()
		{
			if(File.Exists("Content/" + service.ProjectName + "/InputCommands.xml"))
				inputSaverAndLoader.LoadInput(this);
		}
	}
}