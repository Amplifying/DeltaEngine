using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Content;
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
			service.ContentUpdated += (type, name) =>
			{
				RefreshContentList();
				ContentLoader.RemoveResource(name);
				if (isContentReadyForUse)
					Dispatcher.Invoke(
						new Action(
							() =>
								contentManagerViewModel.SelectedContent =
									new ContentIconAndName(contentManagerViewModel.GetContentTypeIcon(type), name)));
			};
			service.ContentDeleted += name =>
			{
				RefreshContentList();
				ContentLoader.RemoveResource(name);
			};
			service.ProjectChanged += () =>
			{
				isContentReadyForUse = false;
				RefreshContentList();
			};
			service.ContentReady += () =>
			{
				Dispatcher.Invoke(new Action(contentManagerViewModel.ShowStartContent));
				isContentReadyForUse = true;
			};
			Messenger.Default.Send("ContentManager", "SetSelectedEditorPlugin");
		}

		private bool isContentReadyForUse;

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

		private void DeleteContent(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteContent", "DeleteContent");
		}

		private void OnHelp(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}
	}
}