using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DeltaEngine.Core;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Abstract factory to load types derived from ContentData (images, sounds, xml files, levels,
	/// etc). Returns cached useable instances and provides quick and easy access to all cached data.
	/// </summary>
	public abstract class ContentLoader
	{
		protected ContentLoader(ContentDataResolver resolver)
		{
			this.resolver = resolver;
			ContentLoadedInThread += data => AddLoadedContentToRessources(data);
		}

		protected readonly ContentDataResolver resolver;

		public Content Load<Content>(string contentName) where Content : ContentData
		{
			return Load(typeof(Content), contentName) as Content;
		}

		public virtual List<Content> LoadRecursively<Content>(string parentName)
			where Content : ContentData
		{
			return new List<Content>();
		}

		internal ContentData Load(Type contentType, string contentName)
		{
			if (Path.HasExtension(contentName))
				throw new ContentNameShouldNotHaveExtension();

			if (resources.ContainsKey(contentName))
				if (resources[contentName].IsDisposed)
					resources.Remove(contentName);
				else
					return GetCachedResource(contentType, contentName);

			return LoadAndCacheContent(contentType, contentName);
		}

		public class ContentNameShouldNotHaveExtension : Exception {}

		private ContentData GetCachedResource(Type contentType, string contentName)
		{
			var cachedResource = resources[contentName];
			if (contentType.IsInstanceOfType(cachedResource))
				return cachedResource;

			throw new CachedResourceExistsButIsOfTheWrongType("Content '" + contentName + "' of type '" +
				contentType + "' requested - but type '" + cachedResource.GetType() +
				"' found in cache\n '" + contentName + "' must be in meta data files twice with different suffixes!");
		}

		public class CachedResourceExistsButIsOfTheWrongType : Exception
		{
			public CachedResourceExistsButIsOfTheWrongType(string message)
				: base(message) {}
		}

		protected readonly Dictionary<string, ContentData> resources =
			new Dictionary<string, ContentData>();
		protected readonly Dictionary<string, string> contentFilenames =
			new Dictionary<string, string>();

		protected ContentData LoadAndCacheContent(Type contentType, string contentName)
		{
			var contentData = resolver.Resolve(contentType, contentName);
			LoadContent(contentData);
			resources.Add(contentName, contentData);
			return contentData;
		}

		private void LoadContent(ContentData contentData)
		{
			contentData.InternalLoad(GetContentDataStream);
			//AddContentToThreadedLoadingQueue(contentData);
			//ThreadExtensions.Start(ThreadedLoading);
		}

		private void AddContentToThreadedLoadingQueue(ContentData contentData)
		{
			lock (ContentForThreadedLoading)
			{
				ContentForThreadedLoading.Enqueue(contentData);	
			}
			contentAddedToQueue.Set();
		}

		private readonly AutoResetEvent contentAddedToQueue = new AutoResetEvent(false);

		protected readonly Queue<ContentData> ContentForThreadedLoading = new Queue<ContentData>();

		protected void ThreadedLoading()
		{
			lock (ContentForThreadedLoading)
			{
				while (ContentForThreadedLoading.Count > 0)
				{
					var contentToLoad = ContentForThreadedLoading.Dequeue();
					contentToLoad.InternalLoad(GetContentDataStream);
					ContentLoadedInThread.Invoke(contentToLoad);
				}
			}
			contentAddedToQueue.WaitOne();
		}

		protected event Action<ContentData> ContentLoadedInThread;

		protected void AddLoadedContentToRessources(ContentData loadedContent)
		{
			resources.Add(loadedContent.Name, loadedContent);
		}

		protected abstract Stream GetContentDataStream(string contentName);

		public void ReloadContent(string contentName)
		{
			var content = resources[contentName];
			LoadContent(content);
			content.FireContentChangedEvent();
		}

		public string GetFilenameFromContentName(string contentName)
		{
			string filename;
			if (contentFilenames.TryGetValue(contentName, out filename))
				return filename;

			throw new ContentNotFound(contentName);
		}

		public class ContentNotFound : Exception
		{
			public ContentNotFound(string contentName)
				: base(contentName) {}
		}
	}
}