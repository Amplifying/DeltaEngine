using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Base class for all content classes, which will be loaded and cached by Content.Load.
	/// </summary>
	public abstract class ContentData : IDisposable
	{
		protected ContentData(string contentFilename)
		{
			if (string.IsNullOrEmpty(contentFilename))
				throw new ContentNameMissing();
		}

		public class ContentNameMissing : Exception { }

		public abstract void Dispose();
	}
}