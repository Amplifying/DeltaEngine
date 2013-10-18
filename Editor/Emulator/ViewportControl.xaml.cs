using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

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
			//ShowToolPane();
		}

		private void ShowToolPane()
		{
			ToolPaneGrid.Visibility = Visibility.Visible;
			ViewportHost.Margin = new Thickness(0, 0, 0, 0);
			Graph.Margin = new Thickness(140, 0, 0, 0);
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

		private void DragImage(object sender, System.Windows.Input.MouseEventArgs e)
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

		private void Drag(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				var toolboxEntry = sender as ToolboxEntry;
				Console.WriteLine(toolboxEntry.Icon);
			}
		}

		private bool isDragging;

		private void DragButton(object sender, System.Windows.Input.MouseEventArgs e)
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

		private void DragLabel(object sender, System.Windows.Input.MouseEventArgs e)
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
	}
}