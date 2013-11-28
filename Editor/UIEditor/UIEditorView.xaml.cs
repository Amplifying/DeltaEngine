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

		public void Init(Service setService)
		{
			service = setService;
			DataContext = uiEditorViewModel = new UIEditorViewModel(setService);
			setService.ProjectChanged += ChangeProject;
			setService.ProjectChanged +=
				() => Dispatcher.Invoke(new Action(uiEditorViewModel.RefreshOnContentChange));
			setService.ContentUpdated +=
				(type, s) => Dispatcher.Invoke(new Action(uiEditorViewModel.RefreshOnContentChange));
			setService.ContentDeleted +=
				s => Dispatcher.Invoke(new Action(uiEditorViewModel.RefreshOnContentChange));
			Messenger.Default.Register<string>(this, "SetMaterial", SetMaterial);
			Messenger.Default.Register<string>(this, "SetHoveredMaterial", SetHoveredMaterial);
			Messenger.Default.Register<string>(this, "SetPressedMaterial", SetPressedMaterial);
			Messenger.Default.Register<string>(this, "SetDisabledMaterial", SetDisabledMaterial);
			Messenger.Default.Register<string>(this, "SetHorizontalAllignment", SetHorizontalAllignment);
			Messenger.Default.Register<string>(this, "SetVerticalAllignment",
				SetVerticalAllignmentToNull);
			Messenger.Default.Register<string>(this, "EnabledHoveredButton", EnabledHoveredButton);
			Messenger.Default.Register<string>(this, "EnabledPressedButton", EnabledPressedButton);
			Messenger.Default.Register<string>(this, "EnabledDisableButton", EnabledDisableButton);
			Messenger.Default.Register<string>(this, "EnableButtonChanger", EnableButtonChanger);
		}

		private void ChangeProject()
		{
			Dispatcher.Invoke(new Action(uiEditorViewModel.ResetOnProjectChange));
		}

		private UIEditorViewModel uiEditorViewModel;

		public void Activate()
		{
			service.ShowToolbox(true);
			uiEditorViewModel.ActivateHiddenScene();
		}

		public void Deactivate()
		{
			service.ProjectChanged -= ChangeProject;
			service.ShowToolbox(false);
		}

		private void SetMaterial(string obj)
		{
			MaterialComboBox.Text = obj;
		}

		private void SetHoveredMaterial(string obj)
		{
			MaterialHoveredComboBox.Text = obj;
		}

		private void EnabledHoveredButton(string obj)
		{
			if (string.IsNullOrEmpty(obj))
			{
				MaterialHoveredComboBox.IsEnabled = false;
				return;
			}
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialHoveredComboBox.IsEnabled = false;
			else
				MaterialHoveredComboBox.IsEnabled = true;
		}

		private void SetPressedMaterial(string obj)
		{
			MaterialPressedComboBox.Text = obj;
		}

		private void EnabledPressedButton(string obj)
		{
			if (string.IsNullOrEmpty(obj))
			{
				MaterialPressedComboBox.IsEnabled = false;
				return;
			}
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialPressedComboBox.IsEnabled = false;
			else
				MaterialPressedComboBox.IsEnabled = true;
		}

		private void SetDisabledMaterial(string obj)
		{
			MaterialDisabledComboBox.Text = obj;
		}

		private void EnabledDisableButton(string obj)
		{
			if (string.IsNullOrEmpty(obj))
			{
				MaterialDisabledComboBox.IsEnabled = false;
				return;
			}
			if (obj.Contains("Picture") || obj.Contains("Label"))
				MaterialDisabledComboBox.IsEnabled = false;
			else
				MaterialDisabledComboBox.IsEnabled = true;
		}

		private void EnableButtonChanger(string obj)
		{
			if (string.IsNullOrEmpty(obj))
			{
				InteractiveButtonCheckBox.IsEnabled = false;
				return;
			}
			if (obj.Contains("Picture") || obj.Contains("Label") || obj.Contains("Slider"))
				InteractiveButtonCheckBox.IsEnabled = false;
			else
				InteractiveButtonCheckBox.IsEnabled = true;
		}

		private void SetHorizontalAllignment(string obj)
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

		private void WidthChanged(object sender, RoutedEventArgs routedEventArgs)
		{
			if (UIWidth.Text == "")
				UIWidth.Text = "0";
		}

		private void HeightChanged(object sender, RoutedEventArgs routedEventArgs)
		{
			if (UIHeight.Text == "")
				UIHeight.Text = "0";
		}

		private void LoadScene(object sender, RoutedEventArgs e)
		{
			uiEditorViewModel.LoadScene();
		}
	}
}