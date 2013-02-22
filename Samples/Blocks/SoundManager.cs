using DeltaEngine.Multimedia;

namespace Blocks
{
	/// <summary>
	/// Plays sound effects when key events occur
	/// </summary>
	public class SoundManager
	{
		public SoundManager(ModdableContent content)
		{
			BlockAffixed = content.Load<Sound>("BlockAffixed");
			BlockCouldntMove = content.Load<Sound>("BlockCantMove");
			BlockMoved = content.Load<Sound>("BlockMoved");
			GameLost = content.Load<Sound>("GameLost");
			RowRemoved = content.Load<Sound>("RowRemoved");
			RowsRemoved = content.Load<Sound>("RowsRemoved");
		}

		public Sound BlockAffixed { get; private set; }
		public Sound BlockCouldntMove { get; private set; }
		public Sound BlockMoved { get; private set; }
		public Sound GameLost { get; private set; }
		public Sound RowRemoved { get; private set; }
		public Sound RowsRemoved { get; private set; }
	}
}