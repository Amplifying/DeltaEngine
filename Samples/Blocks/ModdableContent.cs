using DeltaEngine.Core;

namespace Blocks
{
	/// <summary>
	/// Allows a prefix to be applied to the name of each content item prior to loading.
	/// Games can therefore swap mods/skins simply by switching this prefix.
	/// </summary>
	public class ModdableContent : Content
	{
		public ModdableContent(Resolver resolver, string prefix = "")
			: base(resolver)
		{
			Prefix = prefix;
		}

		public string Prefix { get; set; }

		public override ContentType Load<ContentType>(string contentName)
		{
			return base.Load<ContentType>(Prefix + contentName);
			;
		}
	}
}