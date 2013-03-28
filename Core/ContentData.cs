using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Base class for all content classes, which will be loaded and cached by Content.Load.
	/// </summary>
	public abstract class ContentData : IDisposable
	{
		protected ContentData(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ContentNameMissing();

			Filename = filename;
		}

		public class ContentNameMissing : Exception {}

		public string Filename { get; private set; }

		public abstract void Dispose();
	}
}