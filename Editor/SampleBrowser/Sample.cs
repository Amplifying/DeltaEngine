using DeltaEngine.Core;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Data container for SampleItems within the SampleBrowser.
	/// </summary>
	public class Sample
	{
		protected Sample(string title, SampleCategory category, string projectFilePath,
			string assemblyFilePath, string entryClass, string entryMethod)
		{
			Title = title;
			Category = category;
			if (Category == SampleCategory.Game)
			{
				Description = "Sample Game";
				SetImageFilePath(Title);
			}
			else
			{
				Description = "VisualTest";
				ImageFilePath = "http://deltaengine.net/Content/Icons/StaticTest.png";
			}
			ProjectFilePath = projectFilePath;
			AssemblyFilePath = assemblyFilePath;
			EntryClass = entryClass;
			EntryMethod = entryMethod;
		}

		public string Title { get; private set; }
		public string Description { get; private set; }
		public SampleCategory Category { get; private set; }
		public string ImageFilePath { get; private set; }
		public string ProjectFilePath { get; private set; }
		public string AssemblyFilePath { get; private set; }
		public string EntryClass { get; private set; }
		public string EntryMethod { get; private set; }

		private void SetImageFilePath(string fileName)
		{
			ImageFilePath = GetIconWebPath() + fileName + ".png";
		}

		private static string GetIconWebPath()
		{
			return "http://DeltaEngine.net/Content/Icons/";
		}

		public static Sample CreateGame(string title, string projectFilePath,
			string executableFilePath)
		{
			return new Sample(title, SampleCategory.Game, projectFilePath, executableFilePath, "", "");
		}

		public static Sample CreateTest(string title, string projectFilePath, string assemblyFilePath,
			string entryClass, string entryMethod)
		{
			return new Sample(title, SampleCategory.Test, projectFilePath, assemblyFilePath, entryClass,
				entryMethod);
		}

		public bool ContainsFilterText(string filterText)
		{
			return Title.ContainsCaseInsensitive(filterText) ||
				Category.ToString().ContainsCaseInsensitive(filterText) ||
				Description.ContainsCaseInsensitive(filterText) ||
				AssemblyFilePath.ContainsCaseInsensitive(filterText) ||
				EntryMethod.ContainsCaseInsensitive(filterText);
		}

		public override string ToString()
		{
			return "Sample: " + "Title=" + Title + ", Category=" + Category + ", Description=" +
				Description;
		}
	}
}