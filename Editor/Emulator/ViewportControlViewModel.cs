using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.Emulator
{
	public class ViewportControlViewModel : ViewModelBase
	{
		public ViewportControlViewModel()
		{
			Tools = new List<ToolboxEntry>();
			UIImagesInList = new ObservableCollection<string>();
			Messenger.Default.Register<string>(this, "AddToHierachyList", AddToHierachyList);
			Messenger.Default.Register<string>(this, "SetSelectedName", SetSelectedName);
			Messenger.Default.Register<int>(this, "SetSelectedIndex", SetSelectedIndex);
			Messenger.Default.Register<string>(this, "ChangeSelectedControlName",
				ChangeSelectedControlName);
			Messenger.Default.Register<string>(this, "DeleteSelectedContent", DeleteSelectedContent);
			Messenger.Default.Register<string>(this, "ClearScene", ClearScene);
			SetupToolbox();
		}

		private void AddToHierachyList(string newControl)
		{
			UIImagesInList.Add(newControl);
		}

		private void SetSelectedName(string selectedName)
		{
			SelectedNameInList = selectedName;
		}

		private void SetSelectedIndex(int selectedIndex)
		{
			IndexOfSelectedNameInList = selectedIndex;
		}

		private void ChangeSelectedControlName(string newName)
		{
			UIImagesInList[IndexOfSelectedNameInList] = newName;
			SelectedNameInList= newName;
		}

		private void DeleteSelectedContent(string obj)
		{
			Messenger.Default.Send(selectedNameInList, "DeleteSelectedContentFromWpf");
			UIImagesInList.Remove(SelectedNameInList);
			RaisePropertyChanged("SelectedNameInList");
		}

		private void ClearScene(string obj)
		{
			UIImagesInList.Clear();
			RaisePropertyChanged("SelectedNameInList");
		}

		public string SelectedNameInList
		{
			get { return selectedNameInList; }
			set
			{
				selectedNameInList = value;
				Messenger.Default.Send(selectedNameInList, "SetSelectedNameFromHierachy");
				RaisePropertyChanged("SelectedNameInList");
			}
		}

		private string selectedNameInList;

		public int IndexOfSelectedNameInList
		{
			get { return indexOfSelectedNameInList; }
			set
			{
				indexOfSelectedNameInList = value;
				Messenger.Default.Send(indexOfSelectedNameInList, "SetSelectedIndexFromHierachy");
				RaisePropertyChanged("IndexOfSelectedNameInList");
			}
		}

		private int indexOfSelectedNameInList;

		public List<ToolboxEntry> Tools { get; private set; }

		public ObservableCollection<string> UIImagesInList
		{
			get { return uiImagesInList; }
			set
			{
				uiImagesInList = value;
				RaisePropertyChanged("UIImagesInList");
			}
		}

		private ObservableCollection<string> uiImagesInList { get; set; }

		private void SetupToolbox()
		{
			var namesAndPaths = new UIToolNamesAndPaths();
			foreach (string tool in namesAndPaths.GetNames())
				if (controls.Any(c => c == tool))
					Tools.Add(new ToolboxEntry(tool, namesAndPaths.GetImagePath(tool)));
		}

		private readonly string[] controls = new[] { "Button", "Image", "Label", "Slider" };
	}
}