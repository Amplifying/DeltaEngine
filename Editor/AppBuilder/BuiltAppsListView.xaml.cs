using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using Microsoft.SmartDevice.Connectivity;

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
			AppInfo selectedApp = GetBoundApp(controlOwner);
			(launchButton as Control).IsEnabled = false;
			Action enableLaunchButtonAgain =
				() => Dispatcher.BeginInvoke(new Action(() => (launchButton as Control).IsEnabled = true));
			ThreadExtensions.Start(() => TryLaunchApp(selectedApp, enableLaunchButtonAgain));
		}

		private static void TryLaunchApp(AppInfo selectedApp, Action enableLaunchButtonAgain)
		{
			try
			{
				Device device;
				if (selectedApp.Platform == PlatformName.Android)
				{
					var androidDevices = AndroidDeviceFinder.GetAvailableDevices();
					if (androidDevices.Length == 0)
						throw new DeviceNotConnectedException(
							"No Android device found. Make sure you have the Android USB driver installed");
					device = androidDevices[0];
				}
				else
					device = new WebDevice();
				selectedApp.LaunchApp(device);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			enableLaunchButtonAgain();
		}
	}
}
