using DeltaEngine.Core;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Data container for SampleItems within the SampleBrowser.
	/// </summary>
	public class Sample
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string DifficultyLevel { get; set; }
		public string ImageFilePath { get; set; }
		public string ProjectFilePath { get; set; }
		public string ExecutableFilePath { get; set; }
		public string AssemblyNamespace { get; set; }
		public string EntryPointMethod { get; set; }
		public bool AllowLauncher { get; set; }

		public bool ContainsFilterText(string filterText)
		{
			return Title.ContainsCaseInsensitive(filterText) ||
				DifficultyLevel.ContainsCaseInsensitive(filterText) ||
				Description.ContainsCaseInsensitive(filterText) ||
				AssemblyNamespace.ContainsCaseInsensitive(filterText) ||
				EntryPointMethod.ContainsCaseInsensitive(filterText);
		}

		public override string ToString()
		{
			return "Sample: " + "Title=" + Title + ", Difficulty=" + DifficultyLevel + ", Description=" +
				Description;
		}
	}
}