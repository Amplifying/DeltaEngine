using System;
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
			if (resolver == null)
				throw new NullReferenceException();
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public ContentType Load<ContentType>(string contentName)
			where ContentType : ContentData
		{
			if (resources.ContainsKey(contentName.GetHashCode()))
				return resources[contentName.GetHashCode()] as ContentType;

			return LoadAndCacheContent<ContentType>(contentName);
		}

		private ContentType LoadAndCacheContent<ContentType>(string contentName)
			where ContentType : ContentData
		{
			var contentData = resolver.Resolve<ContentType>(contentName);
			resources.Add(contentName.GetHashCode(), contentData);
			return contentData;
		}

		private readonly Dictionary<int, ContentData> resources = new Dictionary<int, ContentData>();
	}
}