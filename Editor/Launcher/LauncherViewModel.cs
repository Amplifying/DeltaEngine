using System;
using System.IO;
using System.Windows.Input;
using DeltaEngine.Editor.Builder;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.Launcher
{
	public class LauncherViewModel : ViewModelBase
	{
		public LauncherViewModel(Service service)
		{
			Service = service;
			availableDevices = new Device[0];
			LaunchPressed = new RelayCommand(OnLaunchExecuted, CanLaunchExecuted);
		}

		public Service Service { get; private set; }
		private Device[] availableDevices;
		public ICommand LaunchPressed { get; private set; }

		protected virtual void OnLaunchExecuted()
		{
			try
			{
				AppPackage selectedPackage = GetSelectedPackage();
				if (SelectedDevice.IsAppInstalled(selectedPackage))
					SelectedDevice.Uninstall(selectedPackage);

				SelectedDevice.Install(selectedPackage);
				SelectedDevice.Launch(selectedPackage);
			}
			catch (Exception ex)
			{
				RaiseErrorOccurredEvent(ex);
			}
		}

		private void RaiseErrorOccurredEvent(Exception exception)
		{
			if (ErrorOccurred != null)
				ErrorOccurred(exception);
			else
				throw new LauncherException(exception);
		}

		public class LauncherException : Exception
		{
			public LauncherException(Exception ex) : base("LauncherViewModel error: ", ex) {}
		}

		public event Action<Exception> ErrorOccurred;

		private AppPackage GetSelectedPackage()
		{
			string appName = Path.GetFileNameWithoutExtension(SelectedPackageFilePath);
			return new AppPackage
			{
				AppName = appName,
				PackageFileData = File.ReadAllBytes(SelectedPackageFilePath),
				PackageFileName = Path.GetFileName(SelectedPackageFilePath),
				PackageGuid = GetAssemblyGuid(appName),
			};
		}

		private static Guid GetAssemblyGuid(string appName)
		{
			switch (appName.ToLower())
			{
				case "logoapp":
					return LogoAppAssemblyGuid;
				case "breakout":
					return BreakoutAssemblyGuid;
				case "blocks":
					return BreakoutAssemblyGuid;

				default:
					throw new AssemblyGuidForAppNotFound(appName);
			}
		}

		public class AssemblyGuidForAppNotFound : Exception
		{
			 public AssemblyGuidForAppNotFound(string appName) : base(appName) {}
		}

		public static readonly Guid LogoAppAssemblyGuid =
			new Guid("4d33a50e-3aa2-4e7e-bc0c-4ef7b3d5e985");
		public static readonly Guid BreakoutAssemblyGuid =
			new Guid("2e78abad-fe79-455a-8393-48e08de57064");
		public static readonly Guid BlocksAssemblyGuid =
			new Guid("8d02900e-a9a6-4510-acd1-f8df74602ed0");

		protected virtual bool CanLaunchExecuted()
		{
			return IsUserPackagePathValid() && IsUserDeviceSelected();
		}

		private bool IsUserPackagePathValid()
		{
			return File.Exists(SelectedPackageFilePath);
		}

		private bool IsUserDeviceSelected()
		{
			return SelectedDevice != null;
		}

		public Device SelectedDevice
		{
			get { return selectedDevice; }
			set
			{
				selectedDevice = value;
				RaisePropertyChanged("SelectedDevice");
			}
		}

		private Device selectedDevice;

		public Device[] AvailableDevices
		{
			get { return availableDevices; }
			private set
			{
				availableDevices = value;
				RaisePropertyChanged("AvailableDevices");
			}
		}

		public string SelectedPackageFilePath
		{
			get { return selectedPackageFilePath; }
			set
			{
				selectedPackageFilePath = value;
				RaisePropertyChanged("SelectedPackageFilePath");
				RefreshAvailableDevices();
			}
		}

		private string selectedPackageFilePath;

		private Device[] GetAvailableDevicesByPackageExtension()
		{
			string packageExtension = GetFileExtensionOfSelectedPackage();
			switch (packageExtension)
			{
				case ".xap":
					return GetAvailableWindowsPhone7Devices();
				case ".apk":
					return GetAvailableAndroidDevices();
				default:
					return new Device[0];
			}
		}

		private string GetFileExtensionOfSelectedPackage()
		{
			return String.IsNullOrEmpty(SelectedPackageFilePath) ?
				"" :
				Path.GetExtension(SelectedPackageFilePath).ToLower();
		}

		private static WP7Device[] GetAvailableWindowsPhone7Devices()
		{
			return new WP7DeviceFinder().GetAvailableDevices();
		}

		private static AndroidDevice[] GetAvailableAndroidDevices()
		{
			return new AndroidDeviceFinder().GetAvailableDevices();
		}

		public void RefreshAvailableDevices()
		{
			AvailableDevices = GetAvailableDevicesByPackageExtension();
			if (AvailableDevices.Length > 0)
				SelectedDevice = AvailableDevices[0];
			else
				SelectedDevice = null;
		}
	}
}