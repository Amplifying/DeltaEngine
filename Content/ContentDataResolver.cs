using System;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Simple factory to provide access to create content data on demand without any resolver.
	/// Only Platforms is allowed to derive from this class.
	/// </summary>
	public class ContentDataResolver
	{
		internal ContentDataResolver() {}

		public virtual ContentData Resolve(Type contentType, string contentName)
		{
			return Activator.CreateInstance(contentType, contentName) as ContentData;
		}
	}
}