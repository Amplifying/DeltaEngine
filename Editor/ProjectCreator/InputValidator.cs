using System.Text.RegularExpressions;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Validates user input to meet the coding standards and avoid exceptions.
	/// </summary>
	public static class InputValidator
	{
		public static bool IsValidFolderName(string validate)
		{
			return Regex.IsMatch(validate, "^[A-Z][a-zA-Z0-9]*$");
		}

		public static bool IsValidPath(string validate)
		{
			return Regex.IsMatch(validate, "^([a-zA-Z]:)?\\\\[[a-zA-Z0-9_-]+\\\\]*");
		}
	}
}