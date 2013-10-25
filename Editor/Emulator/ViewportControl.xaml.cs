using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Forms.Panel;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Engine Viewport for all of the Editor plugins
	/// </summary>
	public partial class ViewportControl : EditorPluginView
	{
		//ncrunch: no coverage start
		public ViewportControl()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = new ViewportControlViewModel();
			SetMessengers();
		}

		private void SetMessengers()
		{
			Messenger.Default.Register<string>(this, "ShowToolboxPane", ShowToolboxPane);
			Messenger.Default.Register<string>(this, "HideToolboxPane", HideToolboxPane);


		}

		public void ShowToolboxPane(string obj)
		{
			ToolPane.Visibility = Visibility.Visible;
			ViewportHost.Margin = new Thickness(0, 0, 0, 0);
			Graph.Margin = new Thickness(140, 0, 0, 0);
		}

		public void HideToolboxPane(string obj)
		{
			ToolPane.Visibility = Visibility.Hidden;
			ViewportHost.Margin = new Thickness(-140, 0, 0, 0);
			Graph.Margin = new Thickness(0, 0, 0, 0);
		}

		public void ProjectChanged() {}

		public void ApplyEmulator()
		{
			SetupScreen();
			SetupHost();
		}

		private void SetupScreen()
		{
			Screen = new Panel();
			Screen.Dock = DockStyle.None;
		}

		public Panel Screen { get; private set; }

		private void SetupHost()
		{
			ViewportHost.Child = Screen;
			Screen.ResumeLayout(false);
		}

		private void CreateAndDragNewSceneControl(object sender, MouseEventArgs e)
		{
			if (UIToolbox.SelectedItem == null)
				return;
			var item = UIToolbox.SelectedItem as ToolboxEntry;
			if(item.ShortName == "Image")
				DragImage(e);
			if (item.ShortName == "Button")
				DragButton(e);
			if (item.ShortName== "Label")
				DragLabel(e);
			if (item.ShortName == "Slider")
				DragSlider(e);
			isClicking = false;
		}

		private void DragImage(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingImage");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private bool isDragging;

		private void DragButton(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingButton");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingButton");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private void DragLabel(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingLabel");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingLabel");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private void DragSlider(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingSlider");
				isDragging = true;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingSlider");
				isDragging = false;
				Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		private void ClickingOnButton(object sender, MouseButtonEventArgs e)
		{
			isClicking = true;
		}

		private bool isClicking;

		private void CreateNewSceneControl(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			if (UIToolbox.SelectedItem == null || isClicking == false)
				return;
			var item = UIToolbox.SelectedItem as ToolboxEntry;
			PlaceCenteredControl(item.ShortName);
			UIToolbox.SelectedItem = null;
			isClicking = false;
		}

		private void PlaceCenteredControl(string newControl)
		{
			Messenger.Default.Send(newControl, "SetCenteredControl");
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
		}

		private void GridOnGotFocus(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("ShowToolboxPane", "ShowToolboxPane");
		}

		private void GridOnLostFocus(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("HideToolboxPane", "HideToolboxPane");
		}

		public string ShortName
		{
			get { return "Viewport"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Viewport.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		private void DeleteSelectedItem(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("", "DeleteSelectedContent");
		}
	}
}