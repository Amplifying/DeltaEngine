using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Layout;
using DeltaEngine.Editor.Helpers;

namespace DeltaEngine.Editor
{
	/// <summary>
	/// The editor main window manages the login and plugin selection (see EditorWindowModel).
	/// </summary>
	public partial class EditorWindow
	{
		public EditorWindow()
			: this(new EditorWindowModel()) {}

		public EditorWindow(EditorWindowModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			viewModel.AddAllPlugins();
			maximizer = new MaximizerForEmptyWindows(this);
			//TODO: save and load layout from last time http://avalondock.codeplex.com/wikipage?title=GettingStarted
		}

		private readonly MaximizerForEmptyWindows maximizer;

		private void OnMinimize(object sender, MouseButtonEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnMaximize(object sender, MouseButtonEventArgs e)
		{
			maximizer.ToggleMaximize();
		}

		private void OnExit(object sender, MouseButtonEventArgs e)
		{
			Application.Current.Shutdown();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (WindowState == WindowState.Maximized)
				maximizer.MaximizeWindow();
			base.OnRenderSizeChanged(sizeInfo);
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var mousePos = e.MouseDevice.GetPosition(this);
			if (e.ClickCount == 2 && mousePos.Y < 50)
				maximizer.ToggleMaximize();
			else if (e.ChangedButton == MouseButton.Left && !maximizer.isMaximized)
				DragMove();
		}

		private void OnEditorPluginSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count >= 1)
				StartEditorPlugin(e.AddedItems[0] as UserControl);
		}

		private void StartEditorPlugin(UserControl plugin)
		{
			EditorPluginSelection.SelectedItem = null;
			foreach (var existingPane in PluginGroup.Children)
				foreach (var existingDocument in existingPane.Children.OfType<LayoutDocument>())
					if (Equals(existingDocument.Content, plugin))
					{
						existingDocument.IsActive = true;
						return;
					}

			var document = new LayoutDocument
			{
				Content = plugin,
				CanClose = true,
				Title = plugin.GetType().Name.Replace("View", "")
			};
			LayoutDocumentPane pane;
			if (PluginGroup.Children.Count < 1)
				//2): currently disabled, all plugins go to the same group
			{
				pane = new LayoutDocumentPane();
				PluginGroup.Children.Add(pane);
			}
			else
				pane = PluginGroup.Children[0] as LayoutDocumentPane;
			pane.Children.Add(document);
			document.IsActive = true;
		}

		private void WebsiteClick(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://DeltaEngine.net/Account");
		}

		private void ProfileClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/Forum/yaf_cp_profile.aspx");
		}
	}
}