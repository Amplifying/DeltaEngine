using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Core;
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
			contentManagerViewModel = new ContentManagerViewModel(service);
			DataContext = contentManagerViewModel;
			service.ContentChanged += RefreshContentAfterUpdateOrDelete;
		}

		private ContentManagerViewModel contentManagerViewModel;

		private void RefreshContentAfterUpdateOrDelete()
		{
			Dispatcher.Invoke(new Action(contentManagerViewModel.RefreshContentList));
		}

		private void OnImageViewDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			Messenger.Default.Send(dataObject, "AddContent");
		}

		private void DeleteSelectedImage(object sender, RoutedEventArgs e)
		{
			IList<ContentView> contentList = new List<ContentView>();
			foreach (var content in ImageList.SelectedItems)
				contentList.Add((ContentView)content);
			Messenger.Default.Send(contentList, "DeleteContent");
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