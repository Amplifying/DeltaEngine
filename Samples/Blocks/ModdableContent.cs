using System.IO;
using DeltaEngine.Core;

namespace Blocks
{
	/// <summary>
	/// Allows the specification of a sub-directory under Content from which content can be read. 
	/// Games can therefore switch mods/skins simply by varying the sub-directory.
	/// </summary>
	public class ModdableContent : Content
	{
		public ModdableContent(Resolver resolver, string subdirectory = "")
			: base(resolver)
		{
			Subdirectory = subdirectory;
		}

		public string Subdirectory { get; set; }

		public override ContentType Load<ContentType>(string contentName)
		{
			return base.Load<ContentType>(Path.Combine(Subdirectory, contentName));
		}
	}
}