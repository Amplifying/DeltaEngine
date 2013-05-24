using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// ShowMessageBox shows a combination of Okay, Cancel and Ignore and returns the clicked result.
	/// </summary>
	[Flags]
	public enum MessageBoxButton
	{
		Okay = 1,
		Cancel = 2,
		Ignore = 4,
	}
}