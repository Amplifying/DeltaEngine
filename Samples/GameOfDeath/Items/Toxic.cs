using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Rabbits in the toxic area slowly die. Still effective due the huge range and long duration.
	/// </summary>
	public class Toxic : Item
	{
		public Toxic(Content content, Point initialPosition)
			: base(content.Load<Image>("Toxic"), content.Load<Image>("ToxicCloud"),
			content.Load<Sound>("ToxicEffect"), initialPosition) { }

		protected override float ImpactSize
		{
			get { return 0.15f; }
		}
		protected override float ImpactTime
		{
			get { return 5.0f; }
		}
		protected override float Damage
		{
			get { return 5; }
		}
		protected override float DoDamageEvery
		{
			get { return 0.1f; }
		}
		public override int Cost
		{
			get { return 20; }
		}
	}
}