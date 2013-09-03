using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Helpers
{
	public class EditorPluginLoader
	{
		public EditorPluginLoader()
			: this(Path.Combine("..", "..")) {}

		public EditorPluginLoader(string pluginBaseDirectory)
		{
			this.pluginBaseDirectory = pluginBaseDirectory;
			UserControlsType = new List<Type>();
		}

		public List<Type> UserControlsType { get; private set; }
		private readonly string pluginBaseDirectory;

		public void FindAndLoadAllPlugins()
		{
			CopyAllEditorPlugins(pluginBaseDirectory);
			FindAllEditorPluginViews();
		}

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
				Logger.Error(new FailedToCopyFiles(
					"Failed to copy " + sourceFile + " to editor directory", ex));
			}
		}

		private class FailedToCopyFiles : Exception
		{
			public FailedToCopyFiles(string message, Exception inner)
				: base(message, inner) {}
		}

		private void FindAllEditorPluginViews()
		{
			var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			var dllFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
			foreach (var file in dllFiles)
			{
				var fileName = Path.GetFileNameWithoutExtension(file);
				if (fileName.StartsWith("DeltaEngine.") &&
					(DateTime.Now - File.GetLastWriteTime(file)).TotalDays > 7)
					Logger.Warning(file + " looks outdated, it was last updated " +
						File.GetLastWriteTime(file) + ". Check if this file is still being used!");
				if (fileName.StartsWith("DeltaEngine.Editor.") &&
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
						typeof(UserControl).IsAssignableFrom(type) && IsNotExcluded(type))
						UserControlsType.Add(type);
			}
			catch (ReflectionTypeLoadException ex)
			{
				Logger.Warning("Failed to get EditorPluginViews from " + assembly + ": " +
					ex.LoaderExceptions.ToText());
			}
		}

		private bool IsNotExcluded(Type type)
		{
			return excludedEditorPlugins.All(name => type.FullName != name);
		}

		private readonly string[] excludedEditorPlugins = new[]
		{ "DeltaEngine.Editor.EmptyEditorPlugin.EmptyEditorPluginView" };
	}
}