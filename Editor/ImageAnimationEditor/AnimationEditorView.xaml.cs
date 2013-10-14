using System.Windows;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	/// <summary>
	/// Interaction logic for AnimationEditorView.xaml
	/// </summary>
	public partial class AnimationEditorView : EditorPluginView
	{
		public AnimationEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new AnimationEditorViewModel(service);
		}

		private AnimationEditorViewModel viewModel;

		public void ProjectChanged()
		{
			viewModel.RefreshData();
		}

		public string ShortName
		{
			get { return "Image Animation"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ImageAnimation.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void DeleteSelectedImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(ImageViewList.SelectedItem.ToString(), "DeletingImage");
		}

		private void MoveImageUp(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.SelectedItem == null)
				return;
			Messenger.Default.Send(ImageViewList.SelectedIndex, "MoveImageUp");
		}

		private void MoveImageDown(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.SelectedItem == null)
				return;
			Messenger.Default.Send(ImageViewList.SelectedIndex, "MoveImageDown");
		}

		private void SaveAnimation(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.Items.Count == 0)
				return;
			Messenger.Default.Send("SaveAnimation", "SaveAnimation");
		}
	}
}