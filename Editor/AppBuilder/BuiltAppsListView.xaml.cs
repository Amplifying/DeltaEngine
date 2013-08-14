using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Shows all available apps provided by the BuiltAppsListViewModel.
	/// </summary>
	public partial class BuiltAppsListView
	{
		public BuiltAppsListView()
		{
			InitializeComponent();
		}

		public BuiltAppsListViewModel ViewModel
		{
			get { return viewModel; }
			set
			{
				viewModel = value;
				DataContext = value;
			}
		}
		private BuiltAppsListViewModel viewModel;

		private void OnRebuildAppClicked(object rebuildButton, RoutedEventArgs e)
		{
			AppInfo boundApp = GetBoundApp(rebuildButton);
			viewModel.RequestRebuild(boundApp);
		}

		private static AppInfo GetBoundApp(object clickedButton)
		{
			Panel controlOwner = GetControlOwner(clickedButton);
			return GetBoundApp(controlOwner);
		}

		private static Panel GetControlOwner(object clickedControl)
		{
			return (clickedControl as Control).Parent as Panel;
		}

		private static AppInfo GetBoundApp(FrameworkElement controlOwner)
		{
			return controlOwner.DataContext as AppInfo;
		}

		private void OnLaunchAppClicked(object launchButton, RoutedEventArgs e)
		{
			Panel controlOwner = GetControlOwner(launchButton);
			AppInfo boundApp = GetBoundApp(controlOwner);
			foreach (ComboBox child in controlOwner.Children.OfType<ComboBox>())
				if (child.Name.StartsWith("Device"))
					TryLaunchApp(boundApp, child.SelectedValue as Device);
		}

		private static void TryLaunchApp(AppInfo app, Device selectedDevice)
		{
			try
			{
				app.LaunchApp(selectedDevice);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}
	}
}
