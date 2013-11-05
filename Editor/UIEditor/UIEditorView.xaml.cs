using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.MaterialEditor;
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
			DataContext = uiEditorViewModel = new UIEditorViewModel(service);
			var viewModel = (UIEditorViewModel)DataContext;
			service.ProjectChanged += () => Dispatcher.Invoke(new Action(viewModel.ResetOnProjectChange));
			service.ContentUpdated +=
				(type, s) => Dispatcher.Invoke(new Action(viewModel.RefreshOnContentChange));
			service.ContentDeleted += s => Dispatcher.Invoke(new Action(viewModel.RefreshOnContentChange));
			Messenger.Default.Register<string>(this, "SetMaterialToNull", SetMaterialToNull);
			Messenger.Default.Register<string>(this, "SetHoveredMaterialToNull",
				SetHoveredMaterialToNull);
			Messenger.Default.Register<string>(this, "SetPressedMaterialToNull",
				SetPressedMaterialToNull);
			Messenger.Default.Register<string>(this, "SetDisabledMaterialToNull",
				SetDisabledMaterialToNull);
			Messenger.Default.Register<string>(this, "SetHorizontalAllignmentToNull",
				SetHorizontalAllignmentToNull);
			Messenger.Default.Register<string>(this, "SetVerticalAllignmentToNull",
				SetVerticalAllignmentToNull);
		}

		private UIEditorViewModel uiEditorViewModel;

		public void Activate()
		{
			uiEditorViewModel.ActivateHidenScene();
		}

		private void SetMaterialToNull(string obj)
		{
			MaterialComboBox.Text = "";
		}

		private void SetHoveredMaterialToNull(string obj)
		{
			MaterialHoveredComboBox.Text = "";
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialHoveredComboBox.IsEnabled = false;
			else
				MaterialHoveredComboBox.IsEnabled = true;
		}

		private void SetPressedMaterialToNull(string obj)
		{
			MaterialPressedComboBox.Text = "";
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialPressedComboBox.IsEnabled = false;
			else
				MaterialPressedComboBox.IsEnabled = true;
		}

		private void SetDisabledMaterialToNull(string obj)
		{
			MaterialDisabledComboBox.Text = "";
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialDisabledComboBox.IsEnabled = false;
			else
				MaterialDisabledComboBox.IsEnabled = true;
		}

		private void SetHorizontalAllignmentToNull(string obj)
		{
			HorizontalAllignment.Text = "";
		}

		private void SetVerticalAllignmentToNull(string obj)
		{
			VerticalAllignment.Text = "";
		}

		private Service service;

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
			if (e.AddedItems.Count == 0)
				return;
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangeMaterial");
		}

		private void ChangeHoveredMaterial(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangeHoveredMaterial");
		}

		private void ChangePressedMaterial(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangePressedMaterial");
		}

		private void ChangeDisabledMaterial(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangeDisabledMaterial");
		}

		private void OpenMaterialEditorClick(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(MaterialEditorView));
		}

		/*private void OpenFontEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(FontCreatorView));
		}*/

		public void IncreaseRenderlayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(1, "ChangeRenderLayer");
		}

		public void DecreaseRenderLayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(-1, "ChangeRenderLayer");
		}

		private void SetMouseIcon(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				Messenger.Default.Send(false, "SetDraggingButton");
				Messenger.Default.Send(false, "SetDraggingLabel");
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void GridOnGotFocus(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("ShowToolboxPane", "ShowToolboxPane");
		}

		private void GridOnLostFocus(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("HideToolboxPane", "HideToolboxPane");
		}

		private void AddNewResolutionToList(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddNewResolution", "AddNewResolution");
		}

		private void ClearScene(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("ClearScene", "ClearScene");
		}
	}
}