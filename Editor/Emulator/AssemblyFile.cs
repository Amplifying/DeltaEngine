using System.IO;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Stores the assembly file path and displays just the simplified assembly name for the combo box
	/// </summary>
	public class AssemblyFile
	{
		public AssemblyFile(string filePath)
		{
			FilePath = filePath;
		}

		public string FilePath { get; set; }

		public string Name
		{
			get
			{
				return
					Path.GetFileNameWithoutExtension(Path.GetFileName(FilePath).Replace("DeltaEngine.", ""));
			}
			set
			{
				FilePath = value;
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}