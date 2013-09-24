using System;
using System.Linq;
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
			Messenger.Default.Send(ImageList.SelectedItems.Cast<ContentIconAndName>().ToArray(),
				"DeleteContent");
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
	}
}