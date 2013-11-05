using System;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	/// <summary>
	/// Interaction logic for AnimationEditorView.xaml
	/// </summary>
	public partial class AnimationEditorView : EditorPluginView
	{		
		//ncrunch: no coverage start
		public AnimationEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			viewModel = new AnimationEditorViewModel(service);
			DataContext = viewModel;
			service.ProjectChanged +=
				() => Dispatcher.Invoke(new Action(viewModel.ResetOnProjectChange));
			service.ContentUpdated += (t, s) => Dispatcher.Invoke(
				new Action(() => viewModel.RefreshOnContentChange()));
			service.ContentDeleted += s => Dispatcher.Invoke(new Action(viewModel.RefreshOnContentChange));
			Messenger.Default.Send("AnimationEditor", "SetSelectedEditorPlugin");
		}

		private AnimationEditorViewModel viewModel;

		public void Activate()
		{
			viewModel.ActivateAnimation();
			Messenger.Default.Send("AnimationEditor", "SetSelectedEditorPlugin");
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

		private void AddImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}
	}
}