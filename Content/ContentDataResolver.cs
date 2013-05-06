using System;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Abstract factory to provide access to create content data on demand via the active resolver
	/// </summary>
	public interface ContentDataResolver
	{
		ContentData Resolve(Type contentType, string contentName);
	}
}