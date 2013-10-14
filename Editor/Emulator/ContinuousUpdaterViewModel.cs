using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using EnvDTE80;
using GalaSoft.MvvmLight;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Emulator
{
	public class ContinuousUpdaterViewModel : ViewModelBase, IDisposable
	{
		public ContinuousUpdaterViewModel(Service service)
		{
			Assemblies = new List<AssemblyFile>();
			if (File.Exists(PathExtensions.GetEngineSolutionFilePath()))
				ParseAllEngineFolders();
			else
				FindInstalledAssemblies();
			AppendUserFolder();
		}

		private void ParseAllEngineFolders()
		{
			var solutionPath = Path.GetDirectoryName(PathExtensions.GetEngineSolutionFilePath());
			var mainFolders = Directory.GetDirectories(solutionPath);
			foreach (var mainFolder in mainFolders)
				AddAssemblyFolderIfUseful(Path.GetFileName(mainFolder), mainFolder);
		}

		private void AddAssemblyFolderIfUseful(string folderName, string mainFolder)
		{
			if (folderName == "bin" || folderName == "obj" || folderName == ".hg" ||
				folderName == ".nuget" || folderName == "Editor" || folderName == "Content" ||
				folderName == "GameLogic" || folderName == "Logging" || folderName == "Networking" ||
				folderName == "Platforms")
				return;
			var subFolders = Directory.GetDirectories(mainFolder);
			if (folderName == "Samples" || folderName == "Tutorials")
				foreach (var folder in subFolders)
					AddAssemblyFromFolder(Path.Combine(folder, "bin", "Debug"));
			else
				ParseSubFolders(subFolders, mainFolder);
		}

		private void ParseSubFolders(string[] subFolders, string mainFolder)
		{
			foreach (var folder in subFolders)
				if (folder.EndsWith(@"\Tests"))
					AddAssemblyFromFolder(Path.Combine(folder, "bin", "Debug"));
				else
					foreach (var nestedFolder in Directory.GetDirectories(mainFolder))
						if (nestedFolder.EndsWith(@"\Tests"))
							AddAssemblyFromFolder(Path.Combine(nestedFolder, "bin", "Debug"));
		}

		private void AddAssemblyFromFolder(string folder)
		{
			var assemblyName = GetAssemblyNameFromFolder(folder);
			AddAssembly(Path.Combine(folder, assemblyName + ".dll"));
			AddAssembly(Path.Combine(folder, assemblyName + ".exe"));
		}

		internal static string GetAssemblyNameFromFolder(string folder)
		{
			var parts = folder.Split('\\');
			string assemblyName = "";
			for (int num = parts.Length - 3; num >= 0; num--)
			{
				if (parts[num] == "Samples")
					break;
				assemblyName = assemblyName == "" ? parts[num] : parts[num] + "." + assemblyName;
				if (parts[num] == "DeltaEngine")
					break;
			}
			return assemblyName;
		}

		private void AddAssembly(string assemblyFilePath)
		{
			if (!File.Exists(assemblyFilePath))
				return;
			var newAssembly = new AssemblyFile(assemblyFilePath);
			foreach (var assembly in Assemblies)
				if (assembly.Name == newAssembly.Name)
					return;
			Assemblies.Add(newAssembly);
		}

		private void FindInstalledAssemblies()
		{
			string basePath = PathExtensions.GetDeltaEngineInstalledDirectory();
			if (Directory.Exists(Path.Combine(basePath, "OpenTK")))
				basePath = Path.Combine(basePath, "OpenTK");
			else
			{
				Logger.Warning("Unable to use Continuous Updater: " +
					"Could not find Delta Engine solution or Editor OpenTK installation path");
				return;
			}
			var dllFiles = Directory.GetFiles(basePath, "*.dll");
			foreach (var file in dllFiles)
				AddAssemblyIfSampleOrTest(file);
			var exeFiles = Directory.GetFiles(basePath, "*.exe");
			foreach (var file in exeFiles)
				AddAssemblyIfSampleOrTest(file);
		}

		private void AddAssemblyIfSampleOrTest(string file)
		{
			if (file.EndsWith(".dll") && !file.Contains(".Tests."))
				return;
			string name = Path.GetFileNameWithoutExtension(file);
			if (!new AssemblyName(name).IsAllowed())
				return;
			if (!AssemblyExtensions.IsManagedAssembly(file))
				return;
			if (AssemblyExtensions.IsPlatformAssembly(name.Replace(".Tests", "")))
				return;
			if (!name.Contains(".") || name.EndsWith(".Tests"))
				Assemblies.Add(new AssemblyFile(file));
		}

		public AssemblyFile AssemblyFilePath { get; set; }
		public List<AssemblyFile> Assemblies { get; set; }
		public string SelectedTest { get; set; }
		public List<string> Tests { get; set; }
		public string SourceCode { get; set; }
		public string LastTimeUpdated { get; set; }
		public bool IsUpdating { get; set; }

		private void AppendUserFolder()
		{
			var lastProjectPath = LoadDataFromRegistry("LastProjectPath");
			if (lastProjectPath != null)
				AddAssemblyFromFolder(Path.Combine(lastProjectPath, "bin", "Debug"));
		}

		private static string LoadDataFromRegistry(string name)
		{
			using (var key = Registry.CurrentUser.OpenSubKey(RegistryPathForEditorValues, false))
				if (key != null)
					return (string)key.GetValue(name);
			return null;
		}

		private const string RegistryPathForEditorValues = @"Software\DeltaEngine\Editor";

		public void Dispose()
		{
			StopUpdating();
		}

		public void StopUpdating()
		{
			if (dynamicAssembly != null)
				dynamicAssembly.Dispose();
			dynamicAssembly = null;
		}

		public void Restart()
		{
			Select(AssemblyFilePath);
		}

		public void Select(AssemblyFile assembly)
		{
			StopUpdating();
			dynamicAssembly = new AssemblyStarter(assembly.FilePath);
			Tests = new List<string>(dynamicAssembly.GetTestNames());
			SelectTest(Tests[0]);
		}

		private AssemblyStarter dynamicAssembly;

		public void SelectTest(string testClassAndMethodName)
		{
			SelectedTest = testClassAndMethodName;
			//TODO: do the actual execution of the test
		}

		//ncrunch: no coverage start
		public void OpenCurrentTestInVisualStudio()
		{
			//TODO: selected project, file and line
			const string Filename = @"C:\code\DeltaEngine\Rendering2D\Shapes\Tests\Line2DTests.cs";
			const int FileLine = 100;
			var dte = GetLatestVisualStudioObject();
			if (dte != null && dte.MainWindow != null)
			{
				dte.MainWindow.Activate();
				const string VsViewKindTextView = "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}";
				dte.ItemOperations.OpenFile(Filename, VsViewKindTextView);
				((EnvDTE.TextSelection)dte.ActiveDocument.Selection).GotoLine(FileLine, true);
			}
			else
				Process.Start(Path.Combine(GetLatestVisualStudioBinPath(), "devenv.exe"),
					@"C:\code\DeltaEngine\DeltaEngine.sln " +
						@"/Edit C:\code\DeltaEngine\Rendering2D\Shapes\Tests\Line2DTests.cs " +
						@"/Command ""Edit.GoTo 125""");
		}

		internal DTE2 GetLatestVisualStudioObject()
		{
			for (int versionNumber = 12; versionNumber >= 8; versionNumber--)
				try
				{
					var versionId = "VisualStudio.DTE." + versionNumber + ".0";
					return (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject(versionId);
				}
				// ReSharper disable once EmptyGeneralCatchClause
				catch {}
			return null;
		}

		internal string GetLatestVisualStudioBinPath()
		{
			for (int versionNumber = 12; versionNumber >= 8; versionNumber--)
			{
				var key = Environment.Is64BitOperatingSystem
					? "Wow6432Node\\Microsoft\\VisualStudio\\" + versionNumber + ".0\\"
					: "Microsoft\\VisualStudio\\" + versionNumber + ".0\\";
				var installationPath =
					(string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\" + key, "InstallDir", null);
				if (!String.IsNullOrEmpty(installationPath))
					return installationPath;
			}
			throw new FoundNoVisualStudio();
		}

		public class FoundNoVisualStudio : Exception {}
	}
}