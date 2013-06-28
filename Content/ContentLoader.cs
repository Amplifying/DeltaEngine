using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Abstract factory to load types derived from ContentData (images, sounds, xml files, levels,
	/// etc). Returns cached useable instances and provides quick and easy access to all cached data.
	/// </summary>
	public abstract class ContentLoader
	{
		protected ContentLoader(ContentDataResolver resolver, string contentPath)
		{
			current = this;
			this.resolver = resolver;
			ContentPath = contentPath;
		}

		/// <summary>
		/// Is only initialized when first used. Normally set in Platforms to the OnlineContentLoader.
		/// </summary>
		private static ContentLoader current;

		private readonly ContentDataResolver resolver;
		public string ContentPath { get; protected set; }

		public static Content Load<Content>(string contentName) where Content : ContentData
		{
			if (Path.HasExtension(contentName))
				throw new ContentNameShouldNotHaveExtension();

			return Load(typeof(Content), contentName) as Content;
		}

		public static bool Exists(string contentName)
		{
			return current.GetMetaData(contentName) != null;
		}

		protected abstract void LazyInitialize();

		public class ContentNameShouldNotHaveExtension : Exception {}

		internal static ContentData Load(Type contentType, string contentName)
		{
			if (!current.resources.ContainsKey(contentName))
				return current.LoadAndCacheContent(contentType, contentName);

			if (!current.resources[contentName].IsDisposed)
				return current.GetCachedResource(contentType, contentName);

			current.resources.Remove(contentName);
			return current.LoadAndCacheContent(contentType, contentName);
		}

		private readonly Dictionary<string, ContentData> resources =
			new Dictionary<string, ContentData>();

		private ContentData LoadAndCacheContent(Type contentType, string contentName)
		{
			if (GetMetaData(contentName) == null)
				throw new ContentNotFound(contentName);

			var contentData = resolver.Resolve(contentType, contentName);
			LoadMetaDataAndContent(contentData);
			resources.Add(contentName, contentData);
			return contentData;
		}

		protected abstract ContentMetaData GetMetaData(string contentName);

		public class ContentNotFound : Exception
		{
			public ContentNotFound(string contentName)
				: base(contentName) {}
		}

		private void LoadMetaDataAndContent(ContentData contentData)
		{
			contentData.MetaData = GetMetaData(contentData.Name);
			contentData.InternalLoad(GetContentDataStream);
		}

		protected abstract Stream GetContentDataStream(ContentData content);

		private ContentData GetCachedResource(Type contentType, string contentName)
		{
			var cachedResource = resources[contentName];
			if (contentType.IsInstanceOfType(cachedResource))
				return cachedResource;

			throw new CachedResourceExistsButIsOfTheWrongType("Content '" + contentName + "' of type '" +
				contentType + "' requested - but type '" + cachedResource.GetType() +
				"' found in cache\n '" + contentName +
				"' should not be in meta data files twice with different suffixes!");
		}

		public class CachedResourceExistsButIsOfTheWrongType : Exception
		{
			public CachedResourceExistsButIsOfTheWrongType(string message)
				: base(message) {}
		}

		public static void ReloadContent(string contentName)
		{
			var content = current.resources[contentName];
			current.LoadMetaDataAndContent(content);
			content.FireContentChangedEvent();
		}
	}
}