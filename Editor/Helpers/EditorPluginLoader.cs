using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DeltaEngine.Core;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Helpers
{
	public class EditorPluginLoader
	{
		public EditorPluginLoader()
			: this(Path.Combine("..", "..")) {}

		public EditorPluginLoader(string pluginBaseDirectory)
		{
			UserControlsType = new List<Type>();
			CopyAllEditorPlugins(pluginBaseDirectory);
			FindAllEditorPluginViews();
		}

		public List<Type> UserControlsType { get; private set; }

		private static void CopyAllEditorPlugins(string pluginBaseDirectory)
		{
			var pluginDirectories = Directory.GetDirectories(pluginBaseDirectory);
			foreach (var directory in pluginDirectories)
			{
				var pluginOutputDirectory = Path.Combine(directory, "bin",
					ExceptionExtensions.IsDebugMode ? "Debug" : "Release");
				if (Directory.Exists(pluginOutputDirectory) && !directory.EndsWith("Tests"))
					CopyAllDllsAndPdbsToCurrentDirectory(pluginOutputDirectory);
			}
		}

		private static void CopyAllDllsAndPdbsToCurrentDirectory(string directory)
		{
			var files = Directory.GetFiles(directory);
			foreach (var file in files)
				if (Path.GetExtension(file) == ".dll" || Path.GetExtension(file) == ".pdb")
					CopyIfFileIsNewer(file,
						Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(file)));
		}

		private static void CopyIfFileIsNewer(string sourceFile, string targetFile)
		{
			if (!File.Exists(targetFile) ||
				File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
				TryCopyFile(sourceFile, targetFile);
		}

		private static void TryCopyFile(string sourceFile, string targetFile)
		{
			try
			{
				File.Copy(sourceFile, targetFile, true);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to copy " + sourceFile + " to editor directory: " + ex);
			}
		}

		private void FindAllEditorPluginViews()
		{
			var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			var dllFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
			foreach (var file in dllFiles)
			{
				var fileName = Path.GetFileNameWithoutExtension(file);
				if ((!fileName.StartsWith("DeltaEngine.") || fileName.StartsWith("DeltaEngine.Editor.")) &&
					!assemblies.Any(assembly => assembly.FullName.Contains(fileName)))
					assemblies.Add(Assembly.LoadFile(file));
			}
			foreach (var assembly in assemblies)
				if (assembly.IsAllowed())
					TryAddEditorPlugins(assembly);
		}

		private void TryAddEditorPlugins(Assembly assembly)
		{
			try
			{
				foreach (var type in assembly.GetTypes())
					if (typeof(EditorPluginView).IsAssignableFrom(type) &&
						typeof(UserControl).IsAssignableFrom(type))
						UserControlsType.Add(type);
			}
			catch (ReflectionTypeLoadException ex)
			{
				Console.WriteLine("Failed to get EditorPluginViews from " + assembly + ": " +
					ex.LoaderExceptions.ToText());
			}
		}
	}
}