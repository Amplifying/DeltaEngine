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
			// Caching and contentName to filename resolving is coming soon in Milestone 3.
			return resolver.Resolve<ContentType>(contentName);
		}
	}
}
