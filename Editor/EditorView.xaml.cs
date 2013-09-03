using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using AvalonDock.Layout;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Emulator;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Platforms.Windows;
using OpenTKApp = DeltaEngine.Platforms.App;
using Window = DeltaEngine.Core.Window;

namespace DeltaEngine.Editor
{
	/// <summary>
	/// The editor main window manages the login and plugin selection (see EditorWindowModel).
	/// </summary>
	public partial class EditorView
	{
		public EditorView()
			: this(new EditorViewModel()) {}

		public EditorView(EditorViewModel viewModel)
		{
			viewModel.TextLogger.NewLogMessage +=
				() => Dispatcher.BeginInvoke(new Action(LogOutput.ScrollToEnd));
			Loaded += SetWindowedOrFullscreen;
			Closing += SaveWindowedOrFullscreen;
			this.viewModel = viewModel;
			DataContext = viewModel;
			InitializeComponent();
			viewModel.AddAllPlugins();
			maximizer = new MaximizerForEmptyWindows(this);
			StartInitialPlugin(typeof(EmulatorControl));
			StartInitialPlugin(typeof(ContentManagerView));
			try
			{
				StartOpenTKViewportAndBlock();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				MessageBox.Show("Failed to initialize: " + ex, "Delta Engine Editor - Fatal Error");
			}
		}

		private readonly EditorViewModel viewModel;

		private readonly MaximizerForEmptyWindows maximizer;

		private void SetWindowedOrFullscreen(object sender, RoutedEventArgs e)
		{
			if (viewModel.StartEditorMaximized)
				maximizer.MaximizeWindow();
		}

		private void SaveWindowedOrFullscreen(object sender, CancelEventArgs e)
		{
			viewModel.StartEditorMaximized = maximizer.isMaximized;
		}

		private void StartInitialPlugin(Type type)
		{
			foreach (var plugin in viewModel.EditorPlugins.Where(plugin => plugin.GetType() == type))
				StartEditorPlugin(plugin as UserControl);
		}

		private void StartEditorPlugin(UserControl plugin)
		{
			EditorPluginSelection.SelectedItem = null;
			if (CheckIfPluginIsAlreadyRunningAndFocus(plugin))
				return;
			if (!TryInitializePlugin(plugin))
				return;
			var document = CreateDocumentForPlugin(plugin);
			var pane = CreatePaneForPlugins(plugin);
			pane.Children.Add(document);
			FocusDocumentToSeePlugin(document);
		}

		private bool CheckIfPluginIsAlreadyRunningAndFocus(UserControl plugin)
		{
			foreach (var existingPane in PluginGroup.Children)
				foreach (var existingDocument in existingPane.Children.OfType<LayoutDocument>())
					if (Equals(existingDocument.Content, plugin))
					{
						existingDocument.IsActive = true;
						return true;
					}
			return false;
		}

		private bool TryInitializePlugin(UserControl plugin)
		{
			try
			{
				((EditorPluginView)plugin).Init(viewModel.Service);
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				return false;
			}
		}

		private static LayoutDocument CreateDocumentForPlugin(UserControl plugin)
		{
			return new LayoutDocument
			{
				Content = plugin,
				CanClose = true,
				Title = ((EditorPluginView)plugin).ShortName
			};
		}

		private LayoutDocumentPane CreatePaneForPlugins(UserControl plugin)
		{
			LayoutDocumentPane pane;
			if (((EditorPluginView)plugin).RequiresLargePane)
				pane = PluginGroup.Children.Count < 1
					? CreateAndAddLayoutDocumentPane(0.9) : PluginGroup.Children[0] as LayoutDocumentPane;
			else
				pane = PluginGroup.Children.Count < 2
					? CreateAndAddLayoutDocumentPane(0.1) : PluginGroup.Children[1] as LayoutDocumentPane;
			return pane;
		}

		private LayoutDocumentPane CreateAndAddLayoutDocumentPane(double widthValue)
		{
			var pane = new LayoutDocumentPane();
			pane.DockWidth = new GridLength(widthValue, GridUnitType.Star);
			pane.DockMinWidth = 300;
			PluginGroup.Children.Add(pane);
			return pane;
		}

		private static void FocusDocumentToSeePlugin(LayoutDocument document)
		{
			document.IsActive = true;
		}

		private void StartOpenTKViewportAndBlock()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			var window = TryGetViewport(viewModel.EditorPlugins);
			ElementHost.EnableModelessKeyboardInterop(this);
			StartViewportAndWaitUntilWindowIsClosed(window);
		}

		private WpfHostedFormsWindow TryGetViewport(IEnumerable<EditorPluginView> plugins)
		{
			foreach (var plugin in plugins.Where(plugin => plugin.GetType() == typeof(EmulatorControl)))
				return new WpfHostedFormsWindow(plugin as EmulatorControl, this);
			throw new EngineViewportNotLoaded();
		}

		private class EngineViewportNotLoaded : Exception {}

		private void StartViewportAndWaitUntilWindowIsClosed(FormsWindow window)
		{
			Closing += (sender, args) => window.Dispose();
			window.ViewportSizeChanged += size => InvalidateVisual();
			app = new BlockingViewportApp(window);
			Show();
			app.RunAndBlock();
		}

		private BlockingViewportApp app;

		private class BlockingViewportApp : OpenTKApp
		{
			public BlockingViewportApp(Window windowToRegister)
				: base(windowToRegister) {}

			public void RunAndBlock()
			{
				Run();
			}
		}

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

		private void WebsiteClick(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://DeltaEngine.net/Account");
		}

		private void ProfileClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/Forum/yaf_cp_profile.aspx");
		}

		private void ErrorClick(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://deltaengine.net/Account/ApiKey");
		}

		private void OnContentDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			if (!IsFile(dataObject))
				return;
			var files = (string[])dataObject.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				viewModel.UploadToOnlineService(file);
		}

		private static bool IsFile(IDataObject dropObject)
		{
			return dropObject.GetDataPresent(DataFormats.FileDrop);
		}
	}
}