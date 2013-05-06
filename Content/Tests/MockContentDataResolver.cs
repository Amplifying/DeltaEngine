using System;

namespace DeltaEngine.Content.Tests
{
	internal class MockContentDataResolver : ContentDataResolver
	{
		public ContentData Resolve(Type contentType, string contentName)
		{
			return Activator.CreateInstance(contentType, contentName) as ContentData;
		}
	}
}