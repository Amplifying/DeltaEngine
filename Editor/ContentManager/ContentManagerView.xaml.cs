using System;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ContentManagerView : EditorPluginView
	{
		//ncrunch: no coverage start
		public ContentManagerView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = contentManagerViewModel = new ContentManagerViewModel(service);
			service.ContentUpdated += (type, name) => RefreshContentList();
			service.ContentDeleted += name => RefreshContentList();
			service.ProjectChanged += RefreshContentList;
			service.ContentReady +=
				() => Dispatcher.Invoke(new Action(contentManagerViewModel.ShowStartContent));
			Messenger.Default.Send("ContentManager", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			Messenger.Default.Send("ContentManager", "SetSelectedEditorPlugin");
			contentManagerViewModel.Activate();
		}

		private ContentManagerViewModel contentManagerViewModel;

		private void RefreshContentList()
		{
			Dispatcher.Invoke(new Action(contentManagerViewModel.RefreshContentList));
		}

		private void DeleteSelectedItems(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteContent", "DeleteContent");
		}

		public string ShortName
		{
			get { return "Content Manager"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Content.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void ChangeSelectedItem(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;
			contentManagerViewModel.SelectedContent = e.AddedItems[0];
		}

		private void OpenFileExplorer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("OpenFileExplorerToAddNewContent", "OpenFileExplorerToAddNewContent");
		}
	}
}