using System;
using System.Linq;
using System.Windows;
using DeltaEngine.Editor.Builder;
using DeltaEngine.Editor.Builder.Tests;
using DeltaEngine.Editor.Common;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Launcher
{
	/// <summary>
	/// Interaction logic for LauncherView.xaml
	/// </summary>
	public partial class LauncherView : EditorPluginView
	{
		public LauncherView(Service service)
			: this(new LauncherViewModel(service)) {}

		public LauncherView(LauncherViewModel viewModel)
		{
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
			viewModel.ErrorOccurred += OnErrorOccurred;
		}

		private readonly LauncherViewModel viewModel;

		private void OnErrorOccurred(Exception exception)
		{
			string errorMessage = GetErrorDialogText(exception);
			// TODO: Fix providing the host window via the Editor
			//MessageBox.Show(viewModel.Service.PluginHostWindow, errorMessage);
			Window ownerWindow = viewModel.Service.PluginHostWindow;
			if (ownerWindow == null)
				ownerWindow = Application.Current.MainWindow;
			//if (ownerWindow != null)
			//	MessageBox.Show(ownerWindow, errorMessage);
			//else
			//	MessageBox.Show(errorMessage);
			MessageBox.Show(ownerWindow, errorMessage);
		}

		private static string GetErrorDialogText(Exception exception)
		{
			if (exception is WP7Device.ZuneNotLaunchedException)
				return "You need to launch Zune for your Windows Phone 7 first.";
			if (exception is WP7Device.ScreenLockedException)
				return "You need to unlock the of your Windows Phone 7 first.";

			return "Can't connect to the device because '" + exception + "'.";
		}

		public static Window GetActiveWindow()
		{
			return Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
		}

		public string ShortName
		{
			get { return "Launcher"; }
		}

		public string Icon
		{
			get { return "Icons/Launcher.png"; }
		}

		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Launcher; }
		}

		public int Priority
		{
			get { return 2; }
		}

		private void OnSelectAppToBuildButtonClicked(object sender, RoutedEventArgs e)
		{
			//var builderViewModel = new BuilderViewModel(viewModel.Service);
			var builderViewModel = new BuilderViewModel(new BuildServiceConnectionViaLAN());
			var window = new Window
			{
				Content = new BuilderView(builderViewModel),
				Width = 640,
				Height = 480
			};
			window.ShowDialog();
		}

		private void OnAppPackagePathButtonClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = CreateUserProjectPathBrowseDialog();
			if (dialog.ShowDialog().Equals(true))
				viewModel.SelectedPackageFilePath = dialog.FileName;
		}

		private OpenFileDialog CreateUserProjectPathBrowseDialog()
		{
			return new OpenFileDialog
			{
				DefaultExt = ".sln",
				Filter = "Package files (*.*)|*.*",
				InitialDirectory = GetInitialDirectoryForBrowseDialog(),
			};
		}

		protected virtual string GetInitialDirectoryForBrowseDialog()
		{
			return "";
		}

		private void OnRefreshDevicesButtonClicked(object sender, RoutedEventArgs e)
		{
			viewModel.RefreshAvailableDevices();
		}
	}
}