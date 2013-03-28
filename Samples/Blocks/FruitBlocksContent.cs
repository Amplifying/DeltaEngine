using DeltaEngine.Core;

namespace Blocks
{
	/// <summary>
	/// Loads FruitBlocks related content and settings
	/// </summary>
	public class FruitBlocksContent : BlocksContent
	{
		public FruitBlocksContent(Resolver resolver)
			: base(resolver, "FruitBlocks_")
		{
			DoBricksSplitInHalfWhenRowFull = true;
		}
	}
}