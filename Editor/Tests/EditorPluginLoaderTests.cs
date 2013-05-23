using System;
using DeltaEngine.Core;
using DeltaEngine.Editor.ProjectCreator;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class EditorPluginLoaderTests
	{
		[Test, Category("Slow")]
		public void LoadAllUserControlMainViews()
		{
			var plugins = new EditorPluginLoader("..\\..\\..");
			Console.WriteLine("Plugins: " + plugins.UserControlsType.ToText());
			Assert.Contains(typeof(ProjectCreatorView), plugins.UserControlsType);
		}
	}
}