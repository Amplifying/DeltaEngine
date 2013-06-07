using System;
using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.Launcher
{
	internal class Windows8StorePackageCreator
	{
		public bool IsPackageSigned(string packagePath)
		{
			CheckThatFileExist(packagePath);
			string signtoolArguments = "verify " + packagePath;
			var process = MakeProcess("signtool", signtoolArguments);
			return CheckSigntoolVerifyForSuccess(process);
		}

		private static bool CheckSigntoolVerifyForSuccess(Process process)
		{
			try
			{
				ExecuteProcessAndThrowExceptionOnFailure(process);
			}
			catch (ExternalProcessFailure exception)
			{
				if (exception.Message.Contains("SignTool Error: WinVerifyTrust returned error"))
					return false;
			}

			return true;
		}

		public void SignPackage(string packagePath, string signingKeyPath)
		{
			CheckPathesForPackageSigning(packagePath, signingKeyPath);
			string signtoolArguments = "sign /fd sha256 /a /f " + signingKeyPath + " " + packagePath;
			var process = MakeProcess("signtool", signtoolArguments);
			ExecuteProcessAndThrowExceptionOnFailure(process);
		}

		private static void CheckPathesForPackageSigning(string packagePath, string signingKeyPath)
		{
			CheckThatFileExist(packagePath);
			CheckThatFileExist(signingKeyPath);
		}

		private static void CheckThatFileExist(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);
		}

		public void CreatePackage(string binDirectory, string outputPath)
		{
			CheckFilesForPackagesCreation(binDirectory, outputPath);
			GenerateUnsignedPackage(binDirectory, outputPath);
		}

		private static void CheckFilesForPackagesCreation(string binDirectory, string outputPath)
		{
			CheckThatDirectoryExist(binDirectory);
			if (!File.Exists(Path.Combine(binDirectory, "AppxManifest.xml")))
				throw new AppxManifestNotFound(Path.Combine(binDirectory, "AppxManifest.xml"));

			CheckThatDirectoryExist(Path.GetDirectoryName(outputPath));
		}

		public class AppxManifestNotFound : Exception
		{
			public AppxManifestNotFound(string path)
				: base(path + " not found") { }
		}

		private static void CheckThatDirectoryExist(string directory)
		{
			if (!Directory.Exists(directory))
				throw new DirectoryNotFoundException(directory);
		}

		private static void GenerateUnsignedPackage(string binDirectory, string outputPath)
		{
			string makeappxArguments = "pack /o /d " + binDirectory + " /p " + outputPath;
			var process = MakeProcess("makeappx", makeappxArguments);
			ExecuteProcessAndThrowExceptionOnFailure(process);
		}

		private static Process MakeProcess(string filename, string arguments)
		{
			var process = new Process
			{
				StartInfo =
					new ProcessStartInfo
					{
						FileName = Path.Combine(WindowsKitsFolder, filename),
						Arguments = arguments,
						UseShellExecute = false,
						ErrorDialog = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						RedirectStandardError = true
					}
			};

			return process;
		}

		private const string WindowsKitsFolder = @"C:\Program Files (x86)\Windows Kits\8.0\bin\x86";

		private static void ExecuteProcessAndThrowExceptionOnFailure(Process process)
		{
			process.Start();
			StreamReader errorReader = process.StandardError;
			process.WaitForExit();
			if (process.ExitCode == 1)
				throw new ExternalProcessFailure(process.StartInfo.FileName + " "
					+ process.StartInfo.Arguments, errorReader.ReadToEnd());
		}

		private class ExternalProcessFailure : Exception
		{
			public ExternalProcessFailure(string process, string output)
				: base("Error while executing " + process + ":\r\n" + output) {}
		}
	}
}