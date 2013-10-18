using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.MaterialEditor;
using DeltaEngine.Editor.SpriteFontCreator;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	/// <summary>
	/// Interaction logic for UIEditorView.xaml
	/// </summary>
	public partial class UIEditorView : EditorPluginView
	{
		//ncrunch: no coverage start
		public UIEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			this.service = service;
			DataContext = new UIEditorViewModel(service);
		}

		private Service service;

		public void ProjectChanged() {}

		public string ShortName
		{
			get { return "UI Scenes"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/UI.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void SaveUI(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveUI", "SaveUI");
		}

		private void ChangeMaterial(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangeMaterial");
		}

		private void OpenMaterialEditorClick(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(MaterialEditorView));
		}

		private void OpenFontEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(FontCreatorView));
		}

		public void IncreaseRenderlayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(1, "ChangeRenderLayer");
		}

		public void DecreaseRenderLayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(-1, "ChangeRenderLayer");
		}

		private void DragImage(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingImage");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private bool isDragging;

		private void DragButton(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingButton");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingButton");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void DragLabel(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingLabel");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingLabel");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void SetMouseIcon(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				Messenger.Default.Send(false, "SetDraggingButton");
				Messenger.Default.Send(false, "SetDraggingLabel");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void ChangeGrid(object sender, SelectionChangedEventArgs e)
		{
			Messenger.Default.Send(e.AddedItems[0], "ChangeGrid");
		}

		private void DeleteSelectedItem(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteSelectedControl", "DeleteSelectedControl");
		}
	}
}