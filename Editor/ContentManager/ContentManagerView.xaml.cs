using System;
using System.Windows;
using DeltaEngine.Editor.Core;
using DeltaEngine.Networking.Messages;
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
			service.DataReceived += OnDataReceived;
			service.ContentUpdated += (type, name) => RefreshContentList();
			service.ContentDeleted += name => RefreshContentList();
		}

		public void ProjectChanged() {}

		private ContentManagerViewModel contentManagerViewModel;

		private void OnDataReceived(object message)
		{
			if (message is SetProject)
				RefreshContentList();
		}

		private void RefreshContentList()
		{
			Dispatcher.Invoke(new Action(contentManagerViewModel.RefreshContentList));
		}

		private void OnImageViewDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			Messenger.Default.Send(dataObject, "AddContent");
		}

		private void DeleteSelectedItems(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteContent", "DeleteContent");
		}

		private void DeleteSelectedImageAnimation(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(true, "DeleteContent");
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

		private void ChangeSelectedItem(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			contentManagerViewModel.SelectedContent = e.NewValue;
		}
	}
}