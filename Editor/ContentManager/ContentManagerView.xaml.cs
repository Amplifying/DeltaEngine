using System.Windows;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ContentManagerView : EditorPluginView
	{
		public ContentManagerView(Service service)
			: this(new ContentManagerViewModel(service.Content)) {}

		public ContentManagerView(ContentManagerViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void OnImageViewDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			Messenger.Default.Send(dataObject, "AddImage");
		}

		private void DeleteSelectedImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteImage", "DeletingImage");
		}

		private void AddProject(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddProject", "AddProject");
		}

		public string ShortName
		{
			get { return "Content Manager"; }
		}
		public string Icon
		{
			get { return "Icons/Content.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Content; }
		}
		public int Priority
		{
			get { return 1; }
		}
	}
}