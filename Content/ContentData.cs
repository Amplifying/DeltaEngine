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

			StackFrame[] frames = new StackTrace().GetFrames();
			if (frames != null && frames.All(f => f.GetMethod().DeclaringType != typeof(ContentLoader)))
				throw new MustBeCalledFromContentLoader();

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

		internal void InternalLoad(string contentName, Func<string, Stream> getContentDataStream)
		{
			if (CanLoadDataFromStream)
				using (var stream = getContentDataStream(contentName))
					LoadData(stream);
			else
				LoadFromContentName(contentName);
		}

		protected virtual bool CanLoadDataFromStream
		{
			get { return true; }
		}

		/// <summary>
		/// This method needs to be implemented by derived classes to do the actual content loading.
		/// </summary>
		protected abstract void LoadData(Stream fileData);

		/// <summary>
		/// If loading content from a stream is not supported (e.g. with the XNA framework), then
		/// CanLoadDataFromStream is false and this method will be called instead of LoadData.
		/// </summary>
		protected virtual void LoadFromContentName(string contentName) {}

		internal void FireContentChangedEvent()
		{
			if (ContentChanged != null)
				ContentChanged();
		}

		protected Action ContentChanged;
	}
}