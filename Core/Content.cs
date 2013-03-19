using System.Collections.Generic;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Allows to load type derived from ContentData (images, sounds, xml files, levels, etc.) and
	/// returns a cached useable instance of the given type. Most importantly provides an easy and
	/// quick access to all cached resources, but also provides advanced content service downloading.
	/// </summary>
	public class Content
	{
		public Content(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public ContentType Load<ContentType>(string contentName)
			where ContentType : ContentData
		{
			var contentData = TryLoadFromCache(contentName);
			if (contentData != null)
				return (ContentType)contentData;

			return LoadAndCacheContent<ContentType>(contentName);
		}

		private ContentData TryLoadFromCache(string contentName)
		{
			ContentData contentData = null;
			resources.TryGetValue(contentName.GetHashCode(), out contentData);
			return contentData;
		}

		private ContentType LoadAndCacheContent<ContentType>(string contentName)
			where ContentType : ContentData
		{
			var contentData = resolver.Resolve<ContentType>(contentName);
			resources.Add(contentName.GetHashCode(), contentData);
			return contentData;
		}

		private Dictionary<int, ContentData> resources = new Dictionary<int, ContentData>();
	}
}