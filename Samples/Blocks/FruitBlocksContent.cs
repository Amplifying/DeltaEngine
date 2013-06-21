using DeltaEngine.Content;

namespace Blocks
{
	/// <summary>
	/// Loads FruitBlocks related content and settings
	/// </summary>
	public class FruitBlocksContent : BlocksContent
	{
		public FruitBlocksContent(ContentLoader content)
			: base(content, "FruitBlocks_")
		{
			DoBricksSplitInHalfWhenRowFull = true;
		}
	}
}