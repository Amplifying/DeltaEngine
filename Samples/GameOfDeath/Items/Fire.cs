using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Fire lasts a long time and can help to prevent the rabbits from spreading too much.
	/// </summary>
	public class Fire : Item
	{
		public Fire(Content content)
			: base(content.Load<Image>("Fire"), null, content.Load<Sound>("FireEffect")) {}

		protected override float ImpactSize
		{
			get { return 0.06f; }
		}

		protected override float ImpactTime
		{
			get { return 2.5f; }
		}

		protected override float Damage
		{
			get { return 4; }
		}

		protected override float DoDamageEvery
		{
			get { return 0.25f; }
		}

		public override int Cost
		{
			get { return 5; }
		}
	}
}