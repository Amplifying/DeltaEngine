using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Emulator;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using MouseButton = DeltaEngine.Input.MouseButton;
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
			InitializeComponent();
			viewModel.TextLogger.NewLogMessage +=
				() => Dispatcher.BeginInvoke(new Action(LogOutput.ScrollToEnd));
			Loaded += SetWindowedOrFullscreen;
			Closing += SaveWindowedOrFullscreen;
			DataContext = this.viewModel = viewModel;
			queuedContent = new List<string>();
			maximizer = new MaximizerForEmptyWindows(this);
			SetupEditorPlugins();
			SetProjectAndTestFromCommandLineArguments();
			try
			{
				StartOpenTKViewportAndBlock();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (StackTraceExtensions.IsStartedFromNunitConsole())
					throw;
				window.CopyTextToClipboard(ex.ToString());
				MessageBox.Show("Failed to initialize: " + ex + Resolver.ErrorWasCopiedToClipboardMessage,
					"Delta Engine Editor - Fatal Error");
			}
		}

		private void SetWindowedOrFullscreen(object sender, RoutedEventArgs e)
		{
			if (viewModel.StartEditorMaximized)
				maximizer.MaximizeWindow();
		}

		private void SaveWindowedOrFullscreen(object sender, CancelEventArgs e)
		{
			viewModel.StartEditorMaximized = maximizer.isMaximized;
		}

		private readonly EditorViewModel viewModel;
		private readonly List<string> queuedContent;
		private readonly MaximizerForEmptyWindows maximizer;

		private void SetupEditorPlugins()
		{
			viewModel.AddAllPlugins();
			viewModel.Service.StartEditorPlugin += StartInitialPlugin;
			StartInitialPlugin(typeof(ViewportControl));
		}

		private void StartInitialPlugin(Type type)
		{
			StartEditorPlugin(GetPluginByType(type));
		}

		private UserControl GetPluginByType(Type type)
		{
			foreach (var plugin in viewModel.EditorPlugins.Where(plugin => plugin.GetType() == type))
				return plugin as UserControl;
			return null;
		}

		private void StartEditorPlugin(UserControl plugin)
		{
			EditorPluginSelection.SelectedItem = null;
			if (CheckIfPluginIsAlreadyRunningAndFocus(plugin))
				return;
			if (viewport != null)
			{
				viewport.DestroyRenderedEntities();
				viewport.ResetViewportArea();
			}
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

		private LayoutDocument CreateDocumentForPlugin(UserControl plugin)
		{
			var layoutDocument = new LayoutDocument
			{
				Content = plugin,
				CanClose = true,
				Title = ((EditorPluginView)plugin).ShortName
			};
			layoutDocument.IsActiveChanged += ChangeActivePlugin;
			layoutDocument.Closed -= ChangeActivePlugin;
			return layoutDocument;
		}

		private void ChangeActivePlugin(object sender, EventArgs e)
		{
			var userControl = ((UserControl)((LayoutDocument)sender).Content);
			if (userControl.GetType() == typeof(ViewportControl))
				return;
			// ReSharper disable once PossibleUnintendedReferenceComparison
			if (userControl == activePlugin)
				return;
			activePlugin = userControl;
			if (viewModel.Service.Viewport != null)
				viewModel.Service.Viewport.DestroyRenderedEntities();
			((EditorPluginView)userControl).Activate();
		}

		private UserControl activePlugin;

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
			pane.DockMinWidth = widthValue < 0.5 ? SmallPaneMinWidth : LargePaneMinWidth;
			PluginGroup.Children.Add(pane);
			return pane;
		}

		private const int SmallPaneMinWidth = 300;
		private const int LargePaneMinWidth = 540;

		private static void FocusDocumentToSeePlugin(LayoutDocument document)
		{
			document.IsActive = true;
		}

		private void SetProjectAndTestFromCommandLineArguments()
		{
			var arguments = Environment.GetCommandLineArgs();
			if (arguments.Length > 1)
			{
				viewModel.SetProjectAndTest(App.StartupPath, arguments[1],
					arguments.Length > 2 ? arguments[2] : "");
				HandleStartupArguments(App.StartupPath, arguments[1], arguments[2]);
			}
			App.SetProjectAndTest += viewModel.SetProjectAndTest;
			App.SetProjectAndTest += HandleStartupArguments;
		}

		private void HandleStartupArguments(string arg1, string arg2, string arg3)
		{
			SetDeltaEnginePathForCurrentProcess(arg2, arg3);
			ShowPlugin(arg2, arg3);
		}

		private static void SetDeltaEnginePathForCurrentProcess(string arg1, string arg2)
		{
			if (arg1 == PathExtensions.EnginePathEnvironmentVariableName)
				Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, arg2);
		}

		private void ShowPlugin(string arg1, string arg2)
		{
			if (arg1 != "ShowPlugin")
				return;
			Logger.Info(Directory.GetCurrentDirectory());
			var editorPlugin = viewModel.EditorPlugins.FirstOrDefault(p => p.ShortName == arg2);
			StartEditorPlugin(editorPlugin as UserControl);
			maximizer.BringWindowToForeground();
		}

		private void StartOpenTKViewportAndBlock()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			window = TryGetViewportWindow(viewModel.EditorPlugins);
			ElementHost.EnableModelessKeyboardInterop(this);
			StartViewportAndWaitUntilWindowIsClosed(window);
		}

		private WpfHostedFormsWindow window;

		private WpfHostedFormsWindow TryGetViewportWindow(IEnumerable<EditorPluginView> plugins)
		{
			foreach (var plugin in plugins.Where(plugin => plugin.GetType() == typeof(ViewportControl)))
				return new WpfHostedFormsWindow(plugin as ViewportControl, this);
			throw new EngineViewportCouldNotBeCreated();
		}

		private class EngineViewportCouldNotBeCreated : Exception {}

		private void StartViewportAndWaitUntilWindowIsClosed(FormsWindow window)
		{
			Closing += (sender, args) => window.Dispose();
			window.ViewportSizeChanged += size => InvalidateVisual();
			app = new BlockingViewportApp(window);
			RegisterViewportControlCommands();
			viewport = new EditorOpenTkViewport(window);
			viewModel.Service.Viewport = viewport;
			Show();
			GetPluginByType(typeof(ViewportControl)).Visibility = Visibility.Visible;
			StartInitialPlugin(typeof(ContentManagerView));
			app.RunAndBlock();
		}

		private void RegisterViewportControlCommands()
		{
			var dragTrigger = new MousePositionTrigger(MouseButton.Middle, State.Pressed);
			dragTrigger.AddTag("ViewControl");
			Command.Register("ViewportPanning", dragTrigger);
			var dragStartTrigger = new MousePositionTrigger(MouseButton.Middle);
			dragStartTrigger.AddTag("ViewControl");
			Command.Register("ViewportPanningStart", dragStartTrigger);
			var zoomTrigger = new MouseZoomTrigger();
			zoomTrigger.AddTag("ViewControl");
			Command.Register("ViewportZooming", zoomTrigger);
		}

		private EditorOpenTkViewport viewport;

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
			else if (e.ChangedButton == System.Windows.Input.MouseButton.Left && !maximizer.isMaximized)
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

		private void FirstStepsClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/firststeps");
		}

		private void TutorialsClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/tutorials");
		}

		private void AboutTheEditorClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}

		private void StartingWithCSClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/startingwithcsharp");
		}

		private void StartingWithCPPClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/startingwithcpp");
		}

		private void TroubleshootingClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/troubleshootingchecklist");
		}

		private void AppBuilderClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/appbuilder");
		}

		private void DocumentationClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://help.deltaengine.net/");
		}

		private void OnContentDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			if (!IsFile(dataObject))
				return;
			var newFiles = (string[])dataObject.GetData(DataFormats.FileDrop);
			if (isUploadingContent)
			{
				foreach (var file in newFiles)
					queuedContent.Add(file);
				return;
			}
			files = newFiles;
			viewModel.UploadToOnlineService(files[0]);
			isUploadingContent = true;
			contentUploadIndex++;
			viewModel.Service.ContentUpdated += UploadNextFile;
		}

		private string[] files;
		private int contentUploadIndex;

		private void UploadNextFile(ContentType arg1, string arg2)
		{
			if (files.Length < contentUploadIndex + 1)
			{
				contentUploadIndex = 0;
				if (queuedContent.Count == 0)
				{
					viewModel.Service.ContentUpdated -= UploadNextFile;
					isUploadingContent = false;
				}
				else
				{
					files = queuedContent.ToArray();
					queuedContent.Clear();
				}
			}
			else
			{
				viewModel.UploadToOnlineService(files[contentUploadIndex]);
				contentUploadIndex++;
			}
		}

		private bool isUploadingContent;

		private static bool IsFile(IDataObject dropObject)
		{
			return dropObject.GetDataPresent(DataFormats.FileDrop);
		}

		private void OnHelp(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}
	}
}