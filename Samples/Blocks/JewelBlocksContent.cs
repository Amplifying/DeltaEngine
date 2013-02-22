using DeltaEngine.Core;

namespace Blocks
{
	/// <summary>
	/// Assigns all content to load from the FruitBlocks subdirectory
	/// </summary>
	public class JewelBlocksContent : ModdableContent
	{
		public JewelBlocksContent(Resolver resolver)
			: base(resolver, "JewelBlocks") {}
	}
}