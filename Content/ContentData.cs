using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Base class for all content classes. Content is loaded and cached by the ContentLoader.
	/// Content can also be part of an entity as a component. Loading and saving content components
	/// will however only store and retrieve the content name and type, but not any internal data.
	/// </summary>
	public abstract class ContentData : IDisposable
	{
		protected ContentData(string contentName)
		{
			if (string.IsNullOrEmpty(contentName))
				throw new ContentNameMissing();
#if DEBUG
			StackFrame[] frames = new StackTrace().GetFrames();
			if (frames != null && frames.All(f => f.GetMethod().DeclaringType != typeof(ContentLoader)))
				throw new MustBeCalledFromContentLoader();
#endif
			Name = contentName;
		}

		public class ContentNameMissing : Exception {}

		public class MustBeCalledFromContentLoader : Exception {}

		public string Name { get; private set; }

		public void Dispose()
		{
			if (!IsDisposed)
				DisposeData();
			IsDisposed = true;
		}

		public bool IsDisposed { get; private set; }
		protected abstract void DisposeData();

		internal void InternalLoad(Func<ContentData, Stream> getContentDataStream)
		{
			using (var stream = getContentDataStream(this))
				LoadData(stream);
		}

		protected abstract void LoadData(Stream fileData);

		internal void FireContentChangedEvent()
		{
			if (ContentChanged != null)
				ContentChanged();
		}

		protected Action ContentChanged;

		public ContentMetaData MetaData { get; internal set; }
	}
}