using System;
using System.Windows;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace DeltaEngine.Editor.Launcher.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class LauncherViewTests
	{
		private static Window CreateVerifiableWindowFromViewModel(string appName)
		{
			return
				CreateVerifiableWindowFromViewModel(
					LauncherViewModelTests.GetLauncherViewModelWithPreSelectedApp(appName));
		}

		private static Window CreateVerifiableWindowFromViewModel(LauncherViewModel viewModel)
		{
			var window = new Window { Content = new LauncherView(viewModel), Width = 640, Height = 480 };
			((MockLauncherService)viewModel.Service).PluginHostWindow = window;
			return window;
		}

		[Test, STAThread, Category("Slow")]
		public void ShowViewWithMockedModelButRealNetworking()
		{
			Window window = CreateVerifiableWindowFromViewModel("LogoApp.apk");
			window.ShowDialog();
		}
	}
}